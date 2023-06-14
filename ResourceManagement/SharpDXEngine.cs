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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace ResourceManagement
{
    public class SharpDXEngine
    {
        public int FrameCount { get; private set; } = 2;
        public const int ConstantBufferViewCount = 2;
        public const int ShaderResourceViewCount = 2;
        public const int DefaultComponentMapping = 5876;
        const string GLShaderFile = @"C:\Programs\GraphicTest\WriteText\Shaders\shaders.hlsl";
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
            var factory = new Factory1();
            //int adapterCount = factory.GetAdapterCount();
            //for(int i = 0; i < adapterCount; i++)
            //    MessageBox.Show(factory.GetAdapter(i).Description.Description);
            //Adapter a = 
            //viewport = setting.Viewport;
            //FrameCount = setting.FrameCount;
            device = new Device(factory.GetAdapter(1), SharpDX.Direct3D.FeatureLevel.Level_11_0);
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

        public void LoadTexture(SharpDXTextureData data)
        {
            var factory = new Factory1();
            //MessageBox.Show(factory.GetAdapter1(1).Description.Description);
            Adapter1 adp = factory.GetAdapter1(1);
            //long dvm = adp.Description1.DedicatedVideoMemory;
            //MessageBox.Show($"Video Memory: {(double)dvm / 1024 / 1024}MB");
            //long dsm = adp.Description1.DedicatedSystemMemory;
            //MessageBox.Show($"Dedicated System Memory: {(double)dsm / 1024 / 1024}MB");
            //long ssm = adp.Description1.SharedSystemMemory;
            //MessageBox.Show($"Shared System Memory: {(double)ssm / 1024 / 1024}MB");
            
            //var b = PerformanceCounterCategory.GetCategories();
            //var b = PerformanceCounterCategory.GetCategories("KHLEGION");
            //b = b.OrderBy(m => m.CategoryName).ToArray();
           

            //var c = new PerformanceCounterCategory("GPU Engine");
             
            //string s = "";
            ////s += b.Length;
            //for (int i = 0; i < b.Length; i++)
            //{
            //    s += b[i] + "\t";
            //    if (i % 5 == 0)
            //        s += "\n";
            //}

            //MessageBox.Show(s);

            //var a = PerformanceCounterCategory.GetCategories("GPU Adapter Memory");
            //new PerformanceCounter("GPU Adapter Memory", "")
            //MessageBox.Show(pc.RawValue.ToString());

            //device = new Device(factory.GetAdapter1(1), SharpDX.Direct3D.FeatureLevel.Level_11_1);
            //device.CreateCommittedResource()
            Stopwatch sw = Stopwatch.StartNew();
            CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
            commandQueue = device.CreateCommandQueue(queueDesc);
            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, graphicPLState);
            shaderResource = new Resource[2];
            var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Width, data.Height);
            texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
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
            MessageBox.Show($"Upload Annette:{sw.ElapsedMilliseconds} ms");
            sw.Restart();
            //MessageBox.Show(pc.RawValue.ToString());
        }

        public void LoadData()
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
