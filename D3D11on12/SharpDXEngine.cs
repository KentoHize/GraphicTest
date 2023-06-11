using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicLibrary;
using GraphicLibrary.Items;
using SharpDX.Direct3D12;
using SharpDX.Direct2D1;
using Device = SharpDX.Direct3D12.Device;
using Device12 = SharpDX.Direct3D11.Device11On12;
using Device11 = SharpDX.Direct3D11.Device;
using Texture2D11 = SharpDX.Direct3D11.Texture2D;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using Resource = SharpDX.Direct3D12.Resource;
//using Texture2DDescription11 = SharpDX.Direct3D11.Texture2DDescription;
//using SharpDX.DXGI;
using SharpDX;
using SharpDX.DXGI;

namespace D3D11on12
{
    public class SharpDXEngine
    {
        public int FrameCount { get; private set; } = 2;
        public const int ConstantBufferViewCount = 2;
        public const int ShaderResourceViewCount = 2;
        public const int DefaultComponentMapping = 5876;
        const string GLShaderFile = @"C:\Programs\GraphicTest\D3D11on12\Shaders\shaders.hlsl";
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
            //DebugInterface.Get().EnableDebugLayer();
#endif      
            ShaderFiles = new Dictionary<ShaderType, ShaderFileInfo>
            {
                {ShaderType.VertexShader, new ShaderFileInfo(GLShaderFile, ShaderType.VertexShader) },
                {ShaderType.PixelShader, new ShaderFileInfo(GLShaderFile, ShaderType.PixelShader) },
            };
        }

        public Surface Get2DSurface()
        { 
            Device11 device11 = new Device11(SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport | SharpDX.Direct3D11.DeviceCreationFlags.Debug);
            Texture2D11 texture = new Texture2D11(device11, new SharpDX.Direct3D11.Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = SharpDX.Direct3D11.BindFlags.RenderTarget | SharpDX.Direct3D11.BindFlags.ShaderResource,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = 512,
                Width = 512,
                MipLevels = 1,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.SharedKeyedmutex,
                SampleDescription = new SampleDescription(1, 0),
                Usage = SharpDX.Direct3D11.ResourceUsage.Default
            });
            SharpDX.DXGI.Surface surface = texture.QueryInterface<Surface>();
            var d2dFactory = new SharpDX.Direct2D1.Factory1();
            var rtp = new RenderTargetProperties
            {
                MinLevel = FeatureLevel.Level_10,
                Type = RenderTargetType.Hardware,
                PixelFormat = new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)
            };
            var renderTarget2D = new RenderTarget(d2dFactory, surface, rtp);
            return surface;
        }

        public void LoadSetting(SharpDXSetting setting)
        {
            device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);

            Get2DSurface();

        }

        public void LoadStaticData(SharpDXStaticData data)
        {

        }

        public void LoadData(SharpDXData data)
        {

        }

        public void Render()
        {

        }

        public void Update()
        {

        }

        public void WaitForPreviousFrame()
        {
            //int localFence = fenceValue;
            //commandQueue.Signal(this.fence, localFence);
            //fenceValue++;
           
            //if (this.fence.CompletedValue < localFence)
            //{
            //    this.fence.SetEventOnCompletion(localFence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
            //    fenceEvent.WaitOne();
            //}

            //frameIndex = swapChain.CurrentBackBufferIndex;
        }

        public void Close()
        {
            //graphicRootSignature?.Dispose();
            //fence?.Dispose();
            //for (int i = 0; i < renderTargets.Length; i++)
            //    renderTargets[i]?.Dispose();
            //renderTargetViewHeap?.Dispose();
            //commandQueue?.Dispose();
            //swapChain?.Dispose();
            //device?.Dispose();
        }

        public void Dispose()
        {
            Close();
        }

    }
}
