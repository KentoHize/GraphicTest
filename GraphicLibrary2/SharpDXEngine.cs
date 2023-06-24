using GraphicLibrary2.Items;
using SharpDX;

using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Device = SharpDX.Direct3D12.Device;
using Device11 = SharpDX.Direct3D11.Device;
using Device12 = SharpDX.Direct3D11.Device11On12;
using DeviceContext = SharpDX.Direct3D11.DeviceContext;
using Factory4 = SharpDX.DXGI.Factory4;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using Resource = SharpDX.Direct3D12.Resource;
using Resource11 = SharpDX.Direct3D11.Resource;

namespace GraphicLibrary2
{
    public partial class SharpDXEngine : IDisposable
    {   
        public int BufferCount { get; protected set; }
        public string AdapterName => ReferenceEquals(adapter, null) ? "" : adapter.Description2.Description;
        public long SharedMemoryUsage => ReferenceEquals(adapter, null) ? throw new NullReferenceException() : adapter.QueryVideoMemoryInfo(0, MemorySegmentGroup.NonLocal).CurrentUsage;
        public long DedicatedMemoryUsage => ReferenceEquals(adapter, null) ? throw new NullReferenceException() : adapter.QueryVideoMemoryInfo(0, MemorySegmentGroup.Local).CurrentUsage;
        public long DedicatedVideoMemory => ReferenceEquals(adapter, null) ? throw new NullReferenceException() : adapter.Description2.DedicatedVideoMemory;
        public long SharedSystemMemory => ReferenceEquals(adapter, null) ? throw new NullReferenceException() : adapter.Description2.SharedSystemMemory;

        Device? device;
        Adapter4? adapter;
        CommandQueue? commandQueue;
        SwapChain3? swapChain;
        InfoQueue infoQueue;

        PipelineState PLStateNormal, PLStatePoint, PLStateLine;

        DescriptorHeap renderTargetViewHeap;
        int rtvDescriptorSize;

        int frameIndex;

        Resource[] renderTargets;



        GraphicsCommandList commandList;
        GraphicsCommandList[] bundles;
        CommandAllocator commandAllocator;

        AutoResetEvent fenceEvent;
        Fence fence;
        int fenceValue;

        public SharpDXEngine()
        {
#if DEBUG
            DebugInterface.Get().EnableDebugLayer();
#endif
        }

        public void SetGrahpicCardAndRenderTarget(SharpDXInitializeSetting setting)
        {
            BufferCount = setting.BufferCount;
            using (Factory4 factory = new Factory4())
            {   
                adapter = setting.AdapterIndex == -1 ? null : new Adapter4(factory.GetAdapter1(setting.AdapterIndex).NativePointer);
                device = new Device(adapter, (SharpDX.Direct3D.FeatureLevel)setting.FeatureLevel);
                CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
                commandQueue = device.CreateCommandQueue(queueDesc);
                SwapChainDescription swapChainDesc = new SwapChainDescription
                {
                    BufferCount = BufferCount,
                    ModeDescription = new ModeDescription(setting.ScreenWidth, setting.ScreenHeight,
                       new Rational(setting.RefreshRate, 1), Format.R8G8B8A8_UNorm),
                    Usage = Usage.RenderTargetOutput,
                    SwapEffect = SwapEffect.FlipDiscard,
                    OutputHandle = setting.OutputHandle,
                    SampleDescription = new SampleDescription(setting.SampleCount, setting.SampleQuality),
                    IsWindowed = setting.IsWindowed
                };
                SwapChain tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
                swapChain = tempSwapChain.QueryInterface<SwapChain3>();
                tempSwapChain.Dispose();
            }
            frameIndex = swapChain.CurrentBackBufferIndex;
            infoQueue = device.QueryInterface<InfoQueue>();

            DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = BufferCount,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.RenderTargetView
            };
            renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);

            rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);
            var rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            renderTargets = new Resource[BufferCount];
            for (int i = 0; i < BufferCount; i++)
            {
                renderTargets[i] = swapChain.GetBackBuffer<Resource>(i);
                device.CreateRenderTargetView(renderTargets[i], null, rtvHandle);
                rtvHandle += rtvDescriptorSize;
            }

            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;
            fenceEvent = new AutoResetEvent(false);

            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, null);
            commandList.Close();

        }
        public void LoadGraphicSetting(SharpDXGraphicSetting setting)
        {

        }

        public void Dispose()
            => Close();

        public void Close()
        {
            PLStateNormal?.Dispose();
            fence?.Dispose();
            if (renderTargets != null)
                for (int i = 0; i < renderTargets.Length; i++)
                    renderTargets[i]?.Dispose();
            renderTargetViewHeap?.Dispose();
            commandQueue?.Dispose();
            swapChain?.Dispose();
            //device11?.Dispose();
            device?.Dispose();            
        }
    }
}
