using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D12.Device;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using Resource = SharpDX.Direct3D12.Resource;

namespace GraphicLibrary
{
    public class SharpDXEngine : IDisposable
    {
        public int FrameCount { get; private set; }

        Device device;
        SwapChain3 swapChain;
        CommandQueue commandQueue;
        PipelineState pipelineState;
        InfoQueue infoQueue;

        GraphicsCommandList commandList;
        CommandAllocator commandAllocator;
        Resource[] renderTargets;
        DescriptorHeap renderTargetViewHeap;
        int rtvDescriptorSize;

        RootSignature rootSignature;

        ViewportF viewport;
        int frameIndex;

        public SharpDXEngine()
        {
            ShaderFiles = new List<ShaderFileInfo>();
            FrameCount = 2;
        }

        internal List<ShaderFileInfo> ShaderFiles { get; set; }
        public void Initiailize(SharpDXSetting setting)
        {   
            viewport = setting.Viewport;
            FrameCount = setting.FrameCount;
#if DEBUG
            DebugInterface.Get().EnableDebugLayer();
#endif
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
            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);

            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout,
              new RootParameter[]
              {
                    new RootParameter(ShaderVisibility.All,
                        new DescriptorRange()
                        {
                            RangeType = DescriptorRangeType.ConstantBufferView,
                            BaseShaderRegister = 0,
                            RegisterSpace = 0,
                            OffsetInDescriptorsFromTableStart = 0,
                            DescriptorCount = 1
                        })
              });
            rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());
        }

        public void Load(SharpDXData data)
        {

        }

        public void Update()
        {

        }

        public void Render()
        {

        }

        public void Dispose()
        {
            device.Dispose();
        }
    }
}
