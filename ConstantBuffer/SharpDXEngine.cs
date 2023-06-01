using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Device = SharpDX.Direct3D12.Device;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;

namespace ConstantBuffer
{
    public class SharpDXEngine
    {
        Device device;
        CommandQueue commandQueue;
        SwapChain3 swapChain;
        InfoQueue iq;
        int frameIndex;


        RootSignature rootSignature;
        DescriptorHeap constantBufferViewHeap;
        public void Initialize(SharpDXSetting setting)
        {
#if DEBUG   
            DebugInterface.Get().EnableDebugLayer();
#endif
            device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            using (var factory = new Factory4())
            {
                var queueDesc = new CommandQueueDescription(CommandListType.Direct);
                commandQueue = device.CreateCommandQueue(queueDesc);

                var swapChainDesc = new SwapChainDescription()
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

                var tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
                swapChain = tempSwapChain.QueryInterface<SwapChain3>();
                tempSwapChain.Dispose();
                iq = device.QueryInterface<InfoQueue>();
                frameIndex = swapChain.CurrentBackBufferIndex;
            }

            //Init Constant Buffer Heap
            var cbvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = 2,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            };
            constantBufferViewHeap = device.CreateDescriptorHeap(cbvHeapDesc);
            LoadAssets();
        }

        protected void LoadAssets()
        {
            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout,
               new[]
               {
                    new RootParameter(ShaderVisibility.All,
                        new DescriptorRange()
                        {
                            RangeType = DescriptorRangeType.ConstantBufferView,
                            BaseShaderRegister = 0,
                            OffsetInDescriptorsFromTableStart = -1,
                            DescriptorCount = 1
                        },
                         new DescriptorRange()
                        {
                            RangeType = DescriptorRangeType.ConstantBufferView,
                            BaseShaderRegister = 1,
                            OffsetInDescriptorsFromTableStart = -1,
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
    }
}
