﻿using GraphicLibrary;
using GraphicLibrary.Items;
using SharpDX;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D12.Device;
using Device11 = SharpDX.Direct3D11.Device;
using Device12 = SharpDX.Direct3D11.Device11On12;
using DeviceContext = SharpDX.Direct3D11.DeviceContext;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using Resource = SharpDX.Direct3D12.Resource;
using Resource11 = SharpDX.Direct3D11.Resource;
using Factory4 = SharpDX.DXGI.Factory4;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Imaging;
using SharpDX.DirectWrite;

namespace ResourceManagement
{
    public class SharpDXEngine
    {
        Adapter4 adapter;
        public string AdapterName => adapter.Description2.Description;
        public long SharedMemoryUsage => adapter.QueryVideoMemoryInfo(0, MemorySegmentGroup.NonLocal).CurrentUsage;
        public long DedicatedMemoryUsage => adapter.QueryVideoMemoryInfo(0, MemorySegmentGroup.Local).CurrentUsage;
        public long DedicatedVideoMemory => adapter.Description2.DedicatedVideoMemory;
        public long SaredSystemMemory => adapter.Description2.SharedSystemMemory;

        public int FrameCount { get; private set; } = 2;
        public int AdapterIndex { get; private set; } = 1;
        public const int ConstantBufferViewCount = 2;
        public const int ShaderResourceViewCount = 2;
        public const int DefaultComponentMapping = 5876;
        const string GLShaderFile = @"C:\Programs\GraphicTest\ResourceManagement\Shader\shaders.hlsl";
        internal Dictionary<ShaderType, ShaderFileInfo> ShaderFiles { get; set; }
        internal Dictionary<string, int> ModelsTable { get; set; }

        Device device;
        Device11 device11;
        Device12 device12;
        DeviceContext deviceContext;
        Resource11 resource11;

        InfoQueue infoQueue;
        SwapChain3 swapChain;
        CommandQueue commandQueue;
        PipelineState graphicPLState;
        PipelineState computePLState;

        GraphicsCommandList commandList;
        GraphicsCommandList[] bundles;
        CommandAllocator commandAllocator;
        Resource[] renderTargets;
        DescriptorHeap renderTargetViewHeap;
        DescriptorHeap shaderResourceBufferViewHeap;
        int rtvDescriptorSize;
        int cruDescriptorSize;
        CpuDescriptorHandle cruHandle;
        IntPtr ptr;

        RootSignature computeRootSignature;
        RootSignature graphicRootSignature;

        ViewportF viewport;
        int frameIndex;

        AutoResetEvent fenceEvent;
        Fence fence;
        int fenceValue;

        ArFloatVector4 backgroundColor;
        ArFloatMatrix44[] transformMatrix;

        VertexBufferView[] verticesBufferView;
        IndexBufferView[] indicesBufferView;
        Resource[] verticesBuffer;
        Resource[] indicesBuffer;
        Resource[] constantBuffer;
        Resource[] shaderResource;

        Resource texture;

        public SharpDXEngine()
        {
#if DEBUG
            DebugInterface.Get().EnableDebugLayer();
#endif
            ModelsTable = new Dictionary<string, int>();
            ShaderFiles = new Dictionary<ShaderType, ShaderFileInfo>
            {
                {ShaderType.VertexShader, new ShaderFileInfo(GLShaderFile, ShaderType.VertexShader) },
                {ShaderType.PixelShader, new ShaderFileInfo(GLShaderFile, ShaderType.PixelShader) },
            };
        }

        public void LoadSetting(SharpDXSetting setting)
        {
            viewport = setting.Viewport;
            FrameCount = setting.FrameCount;
            using (Factory4 factory = new Factory4())
            {
                adapter = new Adapter4(factory.GetAdapter1(AdapterIndex).NativePointer);
                device = new Device(adapter, SharpDX.Direct3D.FeatureLevel.Level_11_0);
                CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
                commandQueue = device.CreateCommandQueue(queueDesc);

                SwapChainDescription swapChainDesc = new SwapChainDescription
                {
                    BufferCount = 2,
                    ModeDescription = new ModeDescription((int)setting.Viewport.Width, (int)setting.Viewport.Height,
                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    Usage = Usage.RenderTargetOutput,
                    SwapEffect = setting.SwapEffect,
                    OutputHandle = setting.Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    IsWindowed = true
                };

                SwapChain tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
                swapChain = tempSwapChain.QueryInterface<SwapChain3>();
                tempSwapChain.Dispose();
            }
            frameIndex = swapChain.CurrentBackBufferIndex;
            infoQueue = device.QueryInterface<InfoQueue>();

            DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = FrameCount,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.RenderTargetView
            };
            renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);

            rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);
            var rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            renderTargets = new Resource[FrameCount];
            for (int n = 0; n < FrameCount; n++)
            {
                renderTargets[n] = swapChain.GetBackBuffer<Resource>(n);
                device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
                rtvHandle += rtvDescriptorSize;
            }
            CreatePipleLine(setting);
        }

        void CreatePipleLine(SharpDXSetting setting)
        {
            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout,
             new RootParameter[]
             {
                 new RootParameter(ShaderVisibility.All, new RootDescriptor(0, 0), RootParameterType.ConstantBufferView),
                 new RootParameter(ShaderVisibility.All, new RootDescriptor(1, 0), RootParameterType.ConstantBufferView),
                 new RootParameter(ShaderVisibility.All,
                            new DescriptorRange(DescriptorRangeType.ShaderResourceView, 1, 0))
             },
             new StaticSamplerDescription[]
             {
                    new StaticSamplerDescription(ShaderVisibility.All, 0, 0)
                    {
                         Filter = Filter.MinimumMinMagMipPoint,
                         AddressUVW = TextureAddressMode.Border,
                    }
             });
            graphicRootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());
            graphicRootSignature = device.CreateRootSignature(new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout).Serialize());

            InputElement[] inputElementDescs = new InputElement[]
             {
                    new InputElement("POSITION", 0, Format.R32G32B32_SInt,0,0),
                    new InputElement("TEXCOORD", 0, Format.R32G32B32_Float,12,0),
                    new InputElement("SHADOWCOORD", 0, Format.R32G32B32_Float,24,0),
             };

            RasterizerStateDescription rasterizerStateDesc = new RasterizerStateDescription()
            {
                CullMode = setting.CullTwoFace ? CullMode.None : CullMode.Front,
                FillMode = FillMode.Solid,
                IsDepthClipEnabled = false,
                IsFrontCounterClockwise = setting.DrawClockwise,
                IsMultisampleEnabled = false,
            };

            GraphicsPipelineStateDescription psoDesc = new GraphicsPipelineStateDescription()
            {
                InputLayout = new InputLayoutDescription(inputElementDescs),
                RootSignature = graphicRootSignature,
                VertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
                        ShaderFiles[ShaderType.VertexShader].File, ShaderFiles[ShaderType.VertexShader].EntryPoint, ShaderFiles[ShaderType.VertexShader].Profile, SharpDX.D3DCompiler.ShaderFlags.Debug,
                        SharpDX.D3DCompiler.EffectFlags.None, null, FileIncludeHandler.Default)),
                PixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
                        ShaderFiles[ShaderType.PixelShader].File, ShaderFiles[ShaderType.PixelShader].EntryPoint, ShaderFiles[ShaderType.PixelShader].Profile, SharpDX.D3DCompiler.ShaderFlags.Debug,
                        SharpDX.D3DCompiler.EffectFlags.None, null, FileIncludeHandler.Default)),
                RasterizerState = rasterizerStateDesc,
                BlendState = BlendStateDescription.Default(),
                DepthStencilFormat = Format.D32_Float,
                DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
                SampleMask = int.MaxValue,
                PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                RenderTargetCount = 1,
                Flags = PipelineStateFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                StreamOutput = new StreamOutputDescription()
            };
            psoDesc.RenderTargetFormats[0] = Format.R8G8B8A8_UNorm;
            graphicPLState = device.CreateGraphicsPipelineState(psoDesc);

            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;
            fenceEvent = new AutoResetEvent(false);
        }

        public void LoadStaticData()
        {
            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, graphicPLState);

            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);

            WaitForPreviousFrame();
        }

        long GetMemoryUsage(Device device)
        {
            var c = new PerformanceCounterCategory("GPU Adapter Memory");
            string[] instances = c.GetInstanceNames();
            string currentInstance = instances.First(m => m.Contains(device.AdapterLuid.ToString("X")));
            PerformanceCounter pc = new PerformanceCounter("GPU Adapter Memory", "Dedicated Usage", currentInstance, true);
            PerformanceCounter pc2 = new PerformanceCounter("GPU Adapter Memory", "Shared Usage", currentInstance, true);
            return pc.RawValue;
        }

        

        public void Load3DModel (ArDirect3DModel model, string name)
        {

        }

        public void LoadTextureFile(string file, string name)
        {

        }

        Resource LoadBitmapToUploadHeap(string fileName)
        {
            Bitmap bitmap = new Bitmap(fileName);
            int width = bitmap.Width;
            int height = bitmap.Height;
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var textureUploadHeap = device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, width, height), ResourceStates.CopySource);
            textureUploadHeap.WriteToSubresource(0, null, data.Scan0, 4 * width, 4 * width * height);
            bitmap.UnlockBits(data);
            bitmap.Dispose();
            return textureUploadHeap;
        }

        public void LoadTexture(SharpDXTextureData data)
        {
            Stopwatch sw = Stopwatch.StartNew();
            CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
            commandQueue = device.CreateCommandQueue(queueDesc);
            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, graphicPLState);
            shaderResource = new Resource[2];
            
            var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Width, data.Height);
            texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
            
            //texture.
            //sw.Stop();
            //MessageBox.Show($"CreateCommittedResource1:{sw.ElapsedMilliseconds} ms");
            //sw.Restart();
            var textureUploadHeap = device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, textureDesc, ResourceStates.CopySource);
            //var a = textureUploadHeap.Map(0);

            var handle = GCHandle.Alloc(data.Data, GCHandleType.Pinned);
            ptr = Marshal.UnsafeAddrOfPinnedArrayElement(data.Data, 0);            
            textureUploadHeap.WriteToSubresource(0, null, ptr, 4 * data.Width, data.Data.Length);            
            handle.Free();
            
            commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);
            commandList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);
            //commandList.ResourceBarrierTransition(textureUploadHeap, ResourceStates.CopySource, ResourceStates.Common);
            //textureUploadHeap.Unmap(0);


            //var srvDesc = new ShaderResourceViewDescription
            //    {
            //        Shader4ComponentMapping = D3DXUtilities.DefaultComponentMapping(),
            //        Format = textureDesc.Format,
            //        Dimension = ShaderResourceViewDimension.Texture2D,
            //        Texture2D = { MipLevels = 1 },
            //    };
            //    device.CreateShaderResourceView(texture, srvDesc, cruHandle);
            //    cruHandle += cruDescriptorSize;
            

            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);
            sw.Stop();

            
            //textureUploadHeap.
            //MessageBox.Show(a.CurrentUsage.ToString());
            //MessageBox.Show(adp4.QueryVideoMemoryInfo(0, MemorySegmentGroup.NonLocal).CurrentUsage.ToString());
            //MessageBox.Show($"Upload Annette:{sw.ElapsedMilliseconds} ms");
            sw.Restart();
            //MessageBox.Show(pc.RawValue.ToString());
        }

        public void LoadData()
        {
            
        }

        public void Render()
        {
            commandAllocator.Reset();
            commandList.Reset(commandAllocator, graphicPLState);
            commandList.SetGraphicsRootSignature(graphicRootSignature);

            commandList.SetViewport(viewport);
            commandList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle(0, 0, (int)viewport.Width, (int)viewport.Height));

            //commandList.SetGraphicsRootConstantBufferView(0, constantBuffer[0].GPUVirtualAddress);
            //commandList.SetGraphicsRootConstantBufferView(1, constantBuffer[1].GPUVirtualAddress);
            //commandList.SetDescriptorHeaps(new DescriptorHeap[] { shaderResourceBufferViewHeap });
            //commandList.SetGraphicsRootDescriptorTable(2, shaderResourceBufferViewHeap.GPUDescriptorHandleForHeapStart);

            CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += frameIndex * rtvDescriptorSize;
            commandList.SetRenderTargets(rtvHandle, null);
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);
            commandList.ClearRenderTargetView(rtvHandle, new Color4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W), 0, null);

            //for (int i = 0; i < bundles.Length; i++)
            //    commandList.ExecuteBundle(bundles[i]);
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);

            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);

            swapChain.Present(1, 0);

            WaitForPreviousFrame();
        }

        public void WaitForPreviousFrame()
        {
            int localFence = fenceValue;
            commandQueue.Signal(fence, localFence);
            fenceValue++;

            if (fence.CompletedValue < localFence)
            {
                fence.SetEventOnCompletion(localFence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
                fenceEvent.WaitOne();
            }

            frameIndex = swapChain.CurrentBackBufferIndex;
        }

        public void Close()
        {
            graphicRootSignature?.Dispose();
            fence?.Dispose();
            if (renderTargets != null)
                for (int i = 0; i < renderTargets.Length; i++)
                    renderTargets[i]?.Dispose();
            renderTargetViewHeap?.Dispose();
            commandQueue?.Dispose();
            swapChain?.Dispose();
            device11?.Dispose();
            device?.Dispose();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
