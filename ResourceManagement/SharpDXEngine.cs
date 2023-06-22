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
using System.Drawing.Imaging;

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
        internal Dictionary<string, DirectX12Model> ModelTable { get; set; }
        internal Dictionary<int, Resource> TextureTable { get; set; }
        internal Dictionary<int, DirectX12FrameVariables> InstanceFrameVariables { get; set; }

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
        DescriptorHeap shaderResourceViewHeap;
        int rtvDescriptorSize;
        int cruDescriptorSize;
        CpuDescriptorHandle cruHandle;
        IntPtr ptr, ptr2;

        SharpDXSetting setting;
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
        List<Resource> constantBuffer;        
        Resource[] shaderResource;

        Resource texture;

        public SharpDXEngine()
        {
#if DEBUG
            DebugInterface.Get().EnableDebugLayer();
#endif
            ModelTable = new Dictionary<string, DirectX12Model>();
            TextureTable = new Dictionary<int, Resource>();
            InstanceFrameVariables = new Dictionary<int, DirectX12FrameVariables>();
            ShaderFiles = new Dictionary<ShaderType, ShaderFileInfo>
            {
                {ShaderType.VertexShader, new ShaderFileInfo(GLShaderFile, ShaderType.VertexShader) },
                {ShaderType.PixelShader, new ShaderFileInfo(GLShaderFile, ShaderType.PixelShader) },
            };
        }

        public void LoadSetting(SharpDXSetting setting)
        {
            this.setting = setting;
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

            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;
            fenceEvent = new AutoResetEvent(false);

            CreatePipleLine();

            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, graphicPLState);
            commandList.Close();
        }

        void CreatePipleLine()
        {
            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout,
             new RootParameter[]
             {   
                 new RootParameter(ShaderVisibility.All, new RootDescriptor(0, 0), RootParameterType.ConstantBufferView),
                 new RootParameter(ShaderVisibility.All,
                            new DescriptorRange(DescriptorRangeType.ShaderResourceView, 8, 0))
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
            //graphicRootSignature = device.CreateRootSignature(new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout).Serialize());

            InputElement[] inputElementDescs = new InputElement[]
             {
                    new InputElement("POSITION", 0, Format.R32G32B32_SInt,0,0),
                    new InputElement("TEXINDEX", 0, Format.R32_UInt, 12, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float,16,0),
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
        }

        public void PrepareLoadModel()
        {
            bundles = new GraphicsCommandList[1];
            CommandAllocator bundleAllocator = device.CreateCommandAllocator(CommandListType.Bundle);            
            bundles[0] = device.CreateCommandList(0, CommandListType.Bundle, bundleAllocator, graphicPLState);
            bundles[0].SetGraphicsRootSignature(graphicRootSignature);
            bundles[0].PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
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
                Thread.Sleep(1);
        }

        public void LoadModels(IDictionary<string, ArDirect3DModel> modelDictionary)
        {
            foreach (KeyValuePair<string, ArDirect3DModel> kvp in modelDictionary)
                LoadModel(kvp.Key, kvp.Value);
        }

        public void LoadModel(string name, ArDirect3DModel model)
        {
            if (ModelTable.ContainsKey(name))
                throw new ArgumentException(nameof(name));
            DirectX12Model d12model = new DirectX12Model();            
            
            d12model.IndicesCount = model.Indices.Length;
            d12model.VertexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None,
                ResourceDescription.Buffer(model.Vertices.Length * Marshal.SizeOf(typeof(ArDirect3DVertex))), ResourceStates.VertexAndConstantBuffer);

            ptr = d12model.VertexBuffer.Map(0);
            Stopwatch sw = Stopwatch.StartNew();
            Utilities.Write(ptr, model.Vertices, 0, model.Vertices.Length);
            //CopyStructArrayToPtr(model.Vertices, ptr);
            sw.Stop();
            Debug.WriteLine($"CSAP: {sw.ElapsedTicks}");
            d12model.VertexBuffer.Unmap(0);
            d12model.VertexBufferView = new VertexBufferView
            {
                BufferLocation = d12model.VertexBuffer.GPUVirtualAddress,
                StrideInBytes = Marshal.SizeOf(typeof(ArDirect3DVertex)),
                SizeInBytes = model.Vertices.Length * Marshal.SizeOf(typeof(ArDirect3DVertex)),                
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

            //d12model.MaterialIndices = model.MaterialIndices;

            ModelTable.Add(name, d12model);

            //backgroundColor = data.BackgroundColor;
            //ModelsTable.Add(name, model);
            //var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Width, data.Height);
            //texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
        }

        /// <summary>
        /// Bitmap only
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        public void LoadTextureFromFile(int index, string file)
        {
            Resource uploadHeap = LoadBitmapToUploadHeap(file);
            commandAllocator.Reset();
            commandList.Reset(commandAllocator, graphicPLState);

            ResourceDescription textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, uploadHeap.Description.Width, uploadHeap.Description.Height);
            texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
            
            commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(uploadHeap, 0), null);
            commandList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);
            commandList.DiscardResource(uploadHeap, null);
            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);
            WaitForPreviousFrame();
            TextureTable.Add(index, texture);
        }

        public void DeleteTexture(int index)
        {
            if (!TextureTable.ContainsKey(index))
                throw new ArgumentException(nameof(index));
            //砍Resource
            TextureTable.Remove(index);
        }

        public void ClearTextures()
        {
            //砍Resource
            TextureTable.Clear();
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

        public void LoadMaterial(int index, ArMaterial material)
        {
            
        }

        public void LoadTexture(int index, SharpDXTextureData data)
        {   
            commandAllocator.Reset();
            commandList.Reset(commandAllocator, graphicPLState);
            //shaderResource = new Resource[2];
            
            var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Width, data.Height);
            texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
         
            var textureUploadHeap = device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, textureDesc, ResourceStates.CopySource);

            var handle = GCHandle.Alloc(data.Data, GCHandleType.Pinned);
            ptr = Marshal.UnsafeAddrOfPinnedArrayElement(data.Data, 0);            
            textureUploadHeap.WriteToSubresource(0, null, ptr, 4 * data.Width, data.Data.Length);            
            handle.Free();
            
            commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);
            commandList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);
            commandList.DiscardResource(textureUploadHeap, null);

            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);
            TextureTable.Add(index, texture);
        }

        public int GetNewInstanceIndex()
        {
            for(int i = 0; true;  i++)
            {
                if (!InstanceFrameVariables.ContainsKey(i))
                    return i;
                //May Improved
            }
        }

        public int CreateInstance(string name, int index = -1, ArIntVector3? position = null, ArFloatVector3? rotation = null, ArFloatVector3? scaling = null, Dictionary<int, int> replaceMaterialIndices = null)
        {   
            if (!ModelTable.ContainsKey(name))
                throw new ArgumentException(nameof(name));
            if (InstanceFrameVariables.ContainsKey(index))
                throw new ArgumentException(nameof(index));
            if (index == -1)
                index = GetNewInstanceIndex();           

            DirectX12FrameVariables d12fv = new DirectX12FrameVariables();
            d12fv.ReplaceMaterialIndices = replaceMaterialIndices;
            d12fv.TransformMatrix = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.Common);
            ptr = d12fv.TransformMatrix.Map(0);
            Utilities.Write(ptr, new ArFloatMatrix44[] { Ar3DMachine.ProduceTransformMatrix(position ?? ArIntVector3.Zero,
                rotation ?? ArFloatVector3.Zero, scaling ?? ArFloatVector3.One) }, 0, 1);
            d12fv.TransformMatrix.Unmap(0);

            bundles[0].SetGraphicsRootConstantBufferView(0, d12fv.TransformMatrix.GPUVirtualAddress);
            bundles[0].SetVertexBuffer(0, ModelTable[name].VertexBufferView);
            bundles[0].SetIndexBuffer(ModelTable[name].IndexBufferView);
            bundles[0].DrawIndexedInstanced(ModelTable[name].IndicesCount, 1, 0, 0, 0);
            InstanceFrameVariables.Add(index, d12fv);
            return index;
        }
     
        public void SetInstance(int index, ArIntVector3? position = null, ArFloatVector3? rotation = null, ArFloatVector3? scaling = null, Dictionary<int, int> replaceMaterialIndices = null)
        {
            if (!InstanceFrameVariables.ContainsKey(index))
                throw new ArgumentException(nameof(index));
            ptr = InstanceFrameVariables[index].TransformMatrix.Map(0);
            Utilities.Write(ptr, new ArFloatMatrix44[] { Ar3DMachine.ProduceTransformMatrix(position ?? ArIntVector3.Zero,
                rotation ?? ArFloatVector3.Zero, scaling ?? ArFloatVector3.One) }, 0, 1);
            InstanceFrameVariables[index].TransformMatrix.Unmap(0);
        }

        public void DeleteInstance(int index)
        {
            if (!InstanceFrameVariables.ContainsKey(index))
                throw new ArgumentException(nameof(index));
            //砍Instance
        }

        public void ClearInstance()
        {
            InstanceFrameVariables.Clear();
            //砍Instance
        }

        public void DeleteModel(string name)
        {
            if (!ModelTable.ContainsKey(name))
                throw new ArgumentException(nameof(name));
            
            //砍Model
            ModelTable.Remove(name);
        }

        public void ClearModels()
        {
            //砍Model
            ModelTable.Clear();
        }
            

        public void PrepareCreateInstance()
        {
            commandAllocator.Reset();
            commandList.Reset(commandAllocator, graphicPLState);

            //for (int i = 0; i < constantBuffer.Count; i++)
            //    commandList.DiscardResource(constantBuffer[i], null);

            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);
            constantBuffer = new List<Resource>();
        }

        public void PrepareRender()
        {
           
            DescriptorHeapDescription csuHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = 8,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            };
            shaderResourceViewHeap = device.CreateDescriptorHeap(csuHeapDesc);
            cruDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
            cruHandle = shaderResourceViewHeap.CPUDescriptorHandleForHeapStart;

            for (int i = 0; i < TextureTable.Count; i++)
            {
                var srvDesc = new ShaderResourceViewDescription
                {
                    Shader4ComponentMapping = D3DXUtilities.DefaultComponentMapping(),
                    Format = TextureTable[i].Description.Format,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = { MipLevels = 1 },                    
                };
                device.CreateShaderResourceView(TextureTable[i], srvDesc, cruHandle);
                cruHandle += cruDescriptorSize;
            }

            bundles[0].Close();
            WaitForPreviousFrame();
        }

        public void SetOrthographicCamera(string name, int width, int height, long depth, ArIntVector3? position = null, ArFloatVector3? rotation = null)
        {

        }

        public void SetPerspectiveCamera(string name, int width, int height, long depth, ArIntVector3? position = null, ArFloatVector3? rotation = null)
        {

        }

        public void SetAmbientLight()
        { }

        public void SetDirectionalLight()
        { }

        public void SetPointLight()
        { }

        public void SetSpotLight()
        { }

        public void WriteTextOnScreen(string text)
        {

        }

        public void Render()
        {            
            commandAllocator.Reset();
            commandList.Reset(commandAllocator, graphicPLState);
            commandList.SetGraphicsRootSignature(graphicRootSignature);

            commandList.SetViewport(viewport);
            commandList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle(0, 0, (int)viewport.Width, (int)viewport.Height));

            //Resource cb = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);
            //ptr = cb.Map(0);
            //Utilities.Write(ptr, new ArFloatMatrix44[] { ArFloatMatrix44.One }, 0, 1);
            //cb.Unmap(0);
            //commandList.SetGraphicsRootConstantBufferView(0, cb.GPUVirtualAddress);
            //commandList.SetGraphicsRootConstantBufferView(1, constantBuffer[1].GPUVirtualAddress);
            //commandList.SetGraphicsRootConstantBufferView(0, InstanceFrameVariables[0].TransformMatrix.GPUVirtualAddress);
            commandList.SetDescriptorHeaps(new DescriptorHeap[] { shaderResourceViewHeap });
            commandList.SetGraphicsRootDescriptorTable(1, shaderResourceViewHeap.GPUDescriptorHandleForHeapStart);
            //commandList.SetGraphicsRootDescriptorTable(2, shaderResourceViewHeap.GPUDescriptorHandleForHeapStart);

            CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += frameIndex * rtvDescriptorSize;
            commandList.SetRenderTargets(rtvHandle, null);
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);
            commandList.ClearRenderTargetView(rtvHandle, new Color4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W), 0, null);
            commandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            for (int i = 0; i < bundles.Length; i++)
            {
                commandList.ExecuteBundle(bundles[i]);
            }   
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
