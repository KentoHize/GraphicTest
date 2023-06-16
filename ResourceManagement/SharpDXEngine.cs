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
        public const int ConstantBufferViewCount = 2;
        public const int ShaderResourceViewCount = 2;
        public const int DefaultComponentMapping = 5876;
        const string GLShaderFile = @"C:\Programs\GraphicTest\ResourceManagement\Shader\shaders.hlsl";
        internal Dictionary<ShaderType, ShaderFileInfo> ShaderFiles { get; set; }

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
            //ShaderFiles = new Dictionary<ShaderType, ShaderFileInfo>
            //{
            //    {ShaderType.VertexShader, new ShaderFileInfo(GLShaderFile, ShaderType.VertexShader) },
            //    {ShaderType.PixelShader, new ShaderFileInfo(GLShaderFile, ShaderType.PixelShader) },
            //};
        }

        public void LoadSetting(SharpDXSetting setting)
        {
            //AdapterDescription ad = new AdapterDescription()
            //{

            //}
            //int adapterCount = factory.GetAdapterCount();
            //for(int i = 0; i < adapterCount; i++)
            //    MessageBox.Show(factory.GetAdapter(i).Description.Description);
            //Adapter a = 
            //viewport = setting.Viewport;
            //FrameCount = setting.FrameCount;

            var factory = new Factory4();
            adapter = new Adapter4(factory.GetAdapter1(1).NativePointer);
            device = new Device(adapter, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            //MessageBox.Show($"Dedicated Memory Usage: {adapter.QueryVideoMemoryInfo(0, MemorySegmentGroup.Local).CurrentUsage}");
            //MessageBox.Show($"Shared Memory Usage: {adapter.QueryVideoMemoryInfo(0, MemorySegmentGroup.NonLocal).CurrentUsage}");

            //using (Factory4 factory = new Factory4())
            //{
            //    CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
            //    commandQueue = device.CreateCommandQueue(queueDesc);

            //    SwapChainDescription swapChainDesc = new SwapChainDescription
            //    {
            //        BufferCount = 2,
            //        ModeDescription = new ModeDescription((int)setting.Viewport.Width, (int)setting.Viewport.Height,
            //            new Rational(60, 1), Format.R8G8B8A8_UNorm),
            //        Usage = Usage.RenderTargetOutput,
            //        SwapEffect = setting.SwapEffect,
            //        OutputHandle = setting.Handle,
            //        SampleDescription = new SampleDescription(1, 0),
            //        IsWindowed = true
            //    };

            //    SwapChain tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
            //    swapChain = tempSwapChain.QueryInterface<SwapChain3>();
            //    tempSwapChain.Dispose();
            //}
            //frameIndex = swapChain.CurrentBackBufferIndex;
            //infoQueue = device.QueryInterface<InfoQueue>();

            //DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription()
            //{
            //    DescriptorCount = FrameCount,
            //    Flags = DescriptorHeapFlags.None,
            //    Type = DescriptorHeapType.RenderTargetView
            //};
            //renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);

            //rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);
            //var rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            //renderTargets = new Resource[FrameCount];
            //for (int n = 0; n < FrameCount; n++)
            //{
            //    renderTargets[n] = swapChain.GetBackBuffer<Resource>(n);
            //    device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
            //    rtvHandle += rtvDescriptorSize;
            //}
        }

        public void LoadStaticData()
        {

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

        public void LoadTextureFile(string file, string name)
        {

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
