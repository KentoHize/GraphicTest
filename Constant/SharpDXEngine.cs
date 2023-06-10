using GraphicLibrary;
using GraphicLibrary.Items;
using SharpDX;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Runtime.InteropServices;
using Device = SharpDX.Direct3D12.Device;
using Factory4 = SharpDX.DXGI.Factory4;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using Resource = SharpDX.Direct3D12.Resource;
using InputElement = SharpDX.Direct3D12.InputElement;
using FillMode = SharpDX.Direct3D12.FillMode;
using Color = SharpDX.Color;
using Filter = SharpDX.Direct3D12.Filter;
//using ShaderBytecode = SharpDX.D3DCompiler.ShaderBytecode;

namespace Constant
{
    public class SharpDXEngine : IDisposable
    {
        public int FrameCount { get; private set; } = 2;
        public const int ConstantBufferViewCount = 2;
        public const int ShaderResourceViewCount = 2;
        public const int DefaultComponentMapping = 5876;
        const string GLShaderFile = @"C:\Programs\GraphicTest\Constant\Shaders\shaders.hlsl";
        internal Dictionary<ShaderType, ShaderFileInfo> ShaderFiles { get; set; }

        Device device;
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
        DescriptorHeap constantBufferViewHeap;
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
            ShaderFiles = new Dictionary<ShaderType, ShaderFileInfo>
            {
                {ShaderType.VertexShader, new ShaderFileInfo(GLShaderFile, ShaderType.VertexShader) },
                {ShaderType.PixelShader, new ShaderFileInfo(GLShaderFile, ShaderType.PixelShader) },
            };
        }

        protected Resource CreateUploadResource()
        {
            return null;
        }

        public void LoadSetting(SharpDXSetting setting)
        {
            FrameCount = setting.FrameCount;
            viewport = setting.Viewport;
            device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            

            using (Factory4 factory = new Factory4())
            {
                CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
                commandQueue = device.CreateCommandQueue(queueDesc);

                SwapChainDescription swapChainDesc = new SwapChainDescription()
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
            CretatePipleLine(setting);
        }

        void CretatePipleLine(SharpDXSetting setting)
        {
            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout,
             new RootParameter[]
             {   
                 new RootParameter(ShaderVisibility.All, new RootConstants(0, 0, 2)),                 
                 new RootParameter(ShaderVisibility.All,
                            new RootDescriptor(1, 0), RootParameterType.ConstantBufferView),
                 new RootParameter(ShaderVisibility.All,
                            new DescriptorRange(DescriptorRangeType.ShaderResourceView, 1, 0))
                 //new RootParameter(ShaderVisibility.All,
                 //           new RootDescriptor(0, 0), RootParameterType.ShaderResourceView)
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

            InputElement[] inputElementDescs = new InputElement[]
             {
                    new InputElement("POSITION", 0, Format.R32G32B32_SInt,0,0),                    
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float,12,0),
             };

            RasterizerStateDescription rasterizerStateDesc = new RasterizerStateDescription()
            {
                CullMode = setting.CullTwoFace ? CullMode.None : CullMode.Front,
                FillMode = FillMode.Solid,
                IsDepthClipEnabled = false,
                IsFrontCounterClockwise = true,
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

        public void LoadStaticData(SharpDXStaticData data)
        {
            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, graphicPLState);

            DescriptorHeapDescription constantBufferViewHeapDesc = new DescriptorHeapDescription
            {
                DescriptorCount = 1,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            };
            constantBufferViewHeap = device.CreateDescriptorHeap(constantBufferViewHeapDesc);
            cruHandle = constantBufferViewHeap.CPUDescriptorHandleForHeapStart;

            for (int i = 0; i < data.Textures.Length; i++)
            {
                var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Textures[i].Width, data.Textures[i].Height);
                texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);                
                var textureUploadHeap = device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Textures[i].Width, data.Textures[i].Height), ResourceStates.GenericRead);
                var handle = GCHandle.Alloc(data.Textures[i].Data, GCHandleType.Pinned);
                ptr = Marshal.UnsafeAddrOfPinnedArrayElement(data.Textures[i].Data, 0);
                textureUploadHeap.WriteToSubresource(0, null, ptr, 4 * data.Textures[i].Width, data.Textures[i].Data.Length);
                handle.Free();
                commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);
                commandList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);
                var srvDesc = new ShaderResourceViewDescription
                {
                    Shader4ComponentMapping =  Ar3DMachine.DefaultComponentMapping,
                    Format = textureDesc.Format,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = { MipLevels = 1 },
                };
                device.CreateShaderResourceView(texture, srvDesc, cruHandle);
                cruHandle += cruDescriptorSize;
            }

            constantBuffer = new Resource[1];
            constantBuffer[0] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);

            ptr = constantBuffer[0].Map(0);
            Utilities.Write(ptr, new AnotherConstant[] { new AnotherConstant { c = 122, d = 200 } }, 0, 1);
            constantBuffer[0].Unmap(0);

            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);

            WaitForPreviousFrame();
        }

        private void WaitForPreviousFrame()
        {
            // WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
            // This is code implemented as such for simplicity. 

            int localFence = fenceValue;
            commandQueue.Signal(this.fence, localFence);
            fenceValue++;

            // Wait until the previous frame is finished.
            if (this.fence.CompletedValue < localFence)
            {
                this.fence.SetEventOnCompletion(localFence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
                fenceEvent.WaitOne();
            }

            frameIndex = swapChain.CurrentBackBufferIndex;
        }

        struct AnotherConstant
        {
            public int c;
            public int d;
        }

        public void LoadData(SharpDXData data)
        {
            backgroundColor = data.BackgroundColor;
            verticesBufferView = new VertexBufferView[data.VerticesData.Length];
            verticesBuffer = new Resource[data.VerticesData.Length];
            indicesBufferView = new IndexBufferView[data.VerticesData.Length];
            indicesBuffer = new Resource[data.VerticesData.Length];
            transformMatrix = new ArFloatMatrix44[data.VerticesData.Length];
            bundles = new GraphicsCommandList[data.VerticesData.Length];

            for (int i = 0; i < data.VerticesData.Length; i++)
            {
                int dataSize;
                if (data.VerticesData[i].ColorVertices != null)
                    dataSize = ArColorVertex.ByteSize;
                else if (data.VerticesData[i].TextureVertices != null)
                    dataSize = ArTextureVertex.ByteSize;
                else
                    dataSize = ArMixVertex.ByteSize;

                transformMatrix[i] = data.VerticesData[i].TransformMartrix;
                int verticesBufferSize;
                if (data.VerticesData[i].ColorVertices != null)
                    verticesBufferSize = Utilities.SizeOf(data.VerticesData[i].ColorVertices);
                else if (data.VerticesData[i].TextureVertices != null)
                    verticesBufferSize = Utilities.SizeOf(data.VerticesData[i].TextureVertices);
                else
                    verticesBufferSize = Utilities.SizeOf(data.VerticesData[i].MixVertices);
                verticesBuffer[i] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(verticesBufferSize), ResourceStates.GenericRead);
                IntPtr pVertexDataBegin = verticesBuffer[i].Map(0);
                if (data.VerticesData[i].ColorVertices != null)
                    Utilities.Write(pVertexDataBegin, data.VerticesData[i].ColorVertices, 0, data.VerticesData[i].ColorVertices.Length);
                else if (data.VerticesData[i].TextureVertices != null)
                    Utilities.Write(pVertexDataBegin, data.VerticesData[i].TextureVertices, 0, data.VerticesData[i].TextureVertices.Length);
                else
                    Utilities.Write(pVertexDataBegin, data.VerticesData[i].MixVertices, 0, data.VerticesData[i].MixVertices.Length);

                verticesBuffer[i].Unmap(0);
                verticesBufferView[i] = new VertexBufferView
                {
                    BufferLocation = verticesBuffer[i].GPUVirtualAddress,
                    StrideInBytes = dataSize,
                    SizeInBytes = verticesBufferSize
                };

                int indicesBufferSize = Utilities.SizeOf(data.VerticesData[i].Indices);
                indicesBuffer[i] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(indicesBufferSize), ResourceStates.GenericRead);
                pVertexDataBegin = indicesBuffer[i].Map(0);
                Utilities.Write(pVertexDataBegin, data.VerticesData[i].Indices, 0, data.VerticesData[i].Indices.Length);
                indicesBuffer[i].Unmap(0);
                indicesBufferView[i] = new IndexBufferView
                {
                    BufferLocation = indicesBuffer[i].GPUVirtualAddress,
                    SizeInBytes = indicesBufferSize,
                    Format = Format.R32_UInt
                };

                CommandAllocator bundleAllocator = device.CreateCommandAllocator(CommandListType.Bundle);
                bundles[i] = device.CreateCommandList(0, CommandListType.Bundle, bundleAllocator, graphicPLState);
                bundles[i].PrimitiveTopology = data.VerticesData[i].PrimitiveTopology;
                bundles[i].SetVertexBuffer(0, verticesBufferView[i]);
                bundles[i].SetIndexBuffer(indicesBufferView[i]);
                bundles[i].DrawIndexedInstanced(data.VerticesData[i].Indices.Length, 1, 0, 0, 0);
                bundles[i].Close();
            }
        }

        public void Render()
        {
            //commandList.ResourceBarrierTransition(renderTarget)
            commandAllocator.Reset();
            commandList.Reset(commandAllocator, graphicPLState);
            commandList.SetGraphicsRootSignature(graphicRootSignature);

            commandList.SetViewport(viewport);
            commandList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle(0, 0, (int)viewport.Width, (int)viewport.Height));

            commandList.SetDescriptorHeaps(new DescriptorHeap[] { constantBufferViewHeap });
            //commandList.SetGraphicsRootDescriptorTable(0, constantBufferViewHeap.GPUDescriptorHandleForHeapStart);            
            commandList.SetGraphicsRoot32BitConstant(0, 255, 0);
            commandList.SetGraphicsRoot32BitConstant(0, 125, 1);
            
            commandList.SetGraphicsRootConstantBufferView(1, constantBuffer[0].GPUVirtualAddress);
            commandList.SetGraphicsRootDescriptorTable(2, constantBufferViewHeap.GPUDescriptorHandleForHeapStart);
            //commandList.SetGraphicsRootShaderResourceView(2, texture.GPUVirtualAddress);

            CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += frameIndex * rtvDescriptorSize;
            commandList.SetRenderTargets(rtvHandle, null);
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);
            commandList.ClearRenderTargetView(rtvHandle, new Color4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W), 0, null);

            for (int i = 0; i < bundles.Length; i++)
            {             
                commandList.ExecuteBundle(bundles[i]);
            }
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);

            //MessageBox.Show(device.DeviceRemovedReason.ToString());
            
            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);

            swapChain.Present(1, 0);

            WaitForPreviousFrame();
        }

        public void Update()
        {

        }

        public void Close()
        {
            graphicRootSignature?.Dispose();
            fence?.Dispose();
            for (int i = 0; i < renderTargets.Length; i++)
                renderTargets[i]?.Dispose();
            renderTargetViewHeap?.Dispose();
            commandQueue?.Dispose();
            swapChain?.Dispose();
            device?.Dispose();
        }

        public void Dispose()
        {
            Close();
        }
    }
}