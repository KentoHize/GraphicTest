using GraphicLibrary;
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.CompilerServices;

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
        //internal Dictionary<string, Resource> ModelsTable { get; set; }

        internal Dictionary<string, DirectX12Model> ModelsTable { get; set; }

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
        IntPtr ptr, ptr2;

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
            ModelsTable = new Dictionary<string, DirectX12Model>();
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

        

        /// <summary>
        /// 必須是單層陣列，內部為簡單結構
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="structArray"></param>
        /// <param name="ptr"></param>
        void CopyStructArrayToPtr<T>(T[] structArray, IntPtr ptr)
        {
            int structSize = Marshal.SizeOf(typeof(T));
            Task[] task = new Task[structArray.Length];
            for(int i = 0; i < structArray.Length; i++)
            {
                T localstruct = structArray[i];                                
                task[i] = Task.Factory.StartNew((index) => Marshal.StructureToPtr(localstruct, ptr + (int)index * structSize, true), i);
            }

            while (!Task.WhenAll(task).IsCompleted)
                ;
        }

        public void Load3DModel(string name, ArDirect3DModel model)
        {
            DirectX12Model d12model = new DirectX12Model();
            d12model.IndicesCount = model.Indices.Length;
            d12model.VertexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None,
                ResourceDescription.Buffer(model.Vertices.Length * Marshal.SizeOf(typeof(ArDirect3DVertex))), ResourceStates.VertexAndConstantBuffer);

            //var handle = GCHandle.Alloc(model.Vertices, GCHandleType.Pinned);
            //ptr = Marshal.UnsafeAddrOfPinnedArrayElement(model.Vertices, 0);
            //ptr2 = d12model.VertexBuffer.Map(0);
            //Marshal.StructureToPtr(model.Vertices)
            //Marshal.Str
            //MemoryStream ms = new MemoryStream();            
            //Utilities.CopyMemory(ptr2, ptr, model.Vertices.Length * Marshal.SizeOf(typeof(ArDirect3DVertex)));
            //d12model.VertexBuffer.Unmap(0);
            //handle.Free();
            

            ptr = d12model.VertexBuffer.Map(0);
            Stopwatch sw = Stopwatch.StartNew();
            //Utilities.Write(ptr, model.Vertices, 0, model.Vertices.Length);
            CopyStructArrayToPtr(model.Vertices, ptr);
            sw.Stop();
            Debug.WriteLine($"CSAP: {sw.ElapsedTicks}");
            d12model.VertexBuffer.Unmap(0);
            //unsafe
            //{
            //    //Marshal.StructureArrayToPtr
            //    Marshal.StructureToPtr(model.Vertices[0], ptr, true);
            //    Marshal.StructureToPtr(model.Vertices[1], ptr + 36, true);
            //    Marshal.StructureToPtr(model.Vertices[2], ptr + 72, true);
            //    //Utilities.CopyMemory(ptr, );
            //    //Utilities.CopyMemory(ptr, model.Vertices[0], 36);
            //}

            //Utilities.Write(ptr, ref model.Vertices[0]);
            //Utilities.Write(ptr + 36, ref model.Vertices[1]);
            //Utilities.Write(ptr + 72, ref model.Vertices[2]);
            //Utilities.Write(ptr, model.Vertices[0], 0, model.Vertices.Length);
            //unsafe
            //{
            //    fixed (ArDirect3DVertex* ptr = &model.Vertices[0])
            //    {
            //        _003F val = ptr;
            //        int num = sizeof(T) * count;
            //        // IL cpblk instruction
            //        Unsafe.CopyBlockUnaligned(pDest, val, num);
            //        return num + (byte*)pDest;
            //    }
            //    //Interop.Write((void*)ptr, model.Vertices, 0, model.Vertices.Length);
            //}



            //d12model.VertexBuffer
            //d12model.VertexBuffer.WriteToSubresource(0, null, ptr, Marshal.SizeOf(typeof(ArDirect3DVertex)), model.Vertices.Length);


            //d12model.VertexBuffer.WriteToSubresource()

            //Buffer.MemoryCopy()
            d12model.VertexBufferView = new VertexBufferView
            {
                BufferLocation = d12model.VertexBuffer.GPUVirtualAddress,
                StrideInBytes = Marshal.SizeOf(typeof(ArDirect3DVertex)),
                SizeInBytes = model.Vertices.Length * Marshal.SizeOf(typeof(ArDirect3DVertex))
            };
            d12model.IndexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, 
                ResourceDescription.Buffer(model.Indices.Length * sizeof(int)), ResourceStates.IndexBuffer);

            ptr = d12model.IndexBuffer.Map(0);
            Utilities.Write(ptr, model.Indices, 0, model.Indices.Length);
            d12model.IndexBuffer.Unmap(0);

            d12model.IndexBufferView = new IndexBufferView
            {
                BufferLocation = d12model.IndexBuffer.GPUVirtualAddress,
                SizeInBytes = model.Indices.Length * sizeof(int),
                Format = Format.R32_UInt
            };


            bundles = new GraphicsCommandList[1];
            CommandAllocator bundleAllocator = device.CreateCommandAllocator(CommandListType.Bundle);
            bundles[0] = device.CreateCommandList(0, CommandListType.Bundle, bundleAllocator, graphicPLState);
            bundles[0].PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            bundles[0].SetVertexBuffer(0, d12model.VertexBufferView);
            bundles[0].SetIndexBuffer(d12model.IndexBufferView);
            bundles[0].DrawIndexedInstanced(d12model.IndicesCount, 1, 0, 0, 0);
            bundles[0].Close();

            //backgroundColor = data.BackgroundColor;
            //ModelsTable.Add(name, model);
            //var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Width, data.Height);
            //texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
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

            for (int i = 0; i < bundles.Length; i++)
                commandList.ExecuteBundle(bundles[i]);
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
