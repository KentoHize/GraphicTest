using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Device = SharpDX.Direct3D12.Device;
using Device11 = SharpDX.Direct3D11.Device;
using Device12 = SharpDX.Direct3D11.Device11On12;
using DeviceContext = SharpDX.Direct3D11.DeviceContext;
using Factory4 = SharpDX.DXGI.Factory4;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using Resource = SharpDX.Direct3D12.Resource;
using Resource11 = SharpDX.Direct3D11.Resource;

namespace ShaderParameterManager
{
    public partial class SharpDXEngine
    {
        //平台問題可改善
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

        void CreatePipleLine()
        {
            HLSLShaderRPM rpm = new HLSLShaderRPM(device);
            rpm.SetParameter("a", TextureTable, RootParameterType.ShaderResourceView);
            rpm.SetStaticSampler("normal_sampler", new StaticSamplerDescription { ShaderVisibility = ShaderVisibility.All, 
                ShaderRegister = 0, RegisterSpace = 0, Filter = Filter.MinimumMinMagMipPoint, AddressUVW = TextureAddressMode.Border });
            
            //var rsd = rpm.GetRootSignatureDescription();
            rpm.CreateRootParameterShaderFile(Path.Combine(ShaderFolderLocation, "Param.hlsl"));

            grahpicRS = device.CreateRootSignature(rpm.GetRootSignatureDescription().Serialize());
            InputElement[] inputElementDescs = new InputElement[]
            {
                new InputElement("INDEX", 0, Format.R32_UInt, 0, 0)
            };
        }

        RasterizerStateDescription rasterizerStateDesc = new RasterizerStateDescription()
        {
            CullMode = setting.CullTwoFace ? CullMode.None : CullMode.Front,
            FillMode = FillMode.Solid,
            IsDepthClipEnabled = false,
            IsFrontCounterClockwise = setting.DrawClockwise,
            IsMultisampleEnabled = false,
        };

        //GraphicsPipelineStateDescription psoDesc = new GraphicsPipelineStateDescription()
        //{
        //    InputLayout = new InputLayoutDescription(inputElementDescs),
        //    RootSignature = graphicRootSignature,
        //    VertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
        //            ShaderFiles[ShaderType.VertexShader].File, ShaderFiles[ShaderType.VertexShader].EntryPoint, ShaderFiles[ShaderType.VertexShader].Profile, SharpDX.D3DCompiler.ShaderFlags.Debug,
        //            SharpDX.D3DCompiler.EffectFlags.None, null, FileIncludeHandler.Default)),
        //    //SharpDX.D3DCompiler.ShaderBytecode.Compile()
        //    PixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
        //            ShaderFiles[ShaderType.PixelShader].File, ShaderFiles[ShaderType.PixelShader].EntryPoint, ShaderFiles[ShaderType.PixelShader].Profile, SharpDX.D3DCompiler.ShaderFlags.Debug,
        //            SharpDX.D3DCompiler.EffectFlags.None, null, FileIncludeHandler.Default)),
        //    RasterizerState = rasterizerStateDesc,
        //    BlendState = BlendStateDescription.Default(),
        //    DepthStencilFormat = Format.D32_Float,
        //    DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
        //    SampleMask = int.MaxValue,
        //    PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
        //    RenderTargetCount = 1,
        //    Flags = PipelineStateFlags.None,
        //    SampleDescription = new SampleDescription(1, 0),
        //    StreamOutput = new StreamOutputDescription()
        //};
        //psoDesc.RenderTargetFormats[0] = Format.R8G8B8A8_UNorm;
        //    graphicPLState = device.CreateGraphicsPipelineState(psoDesc);

        //    GraphicsPipelineStateDescription gpsDesc = new GraphicsPipelineStateDescription
        //    {
        //        InputLayout = new InputLayoutDescription(inputElementDescs),
        //        RootSignature = graphicRootSignature,
        //        VertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
        //                ShaderFiles[ShaderType.VertexShader].File, ShaderFiles[ShaderType.VertexShader].EntryPoint, ShaderFiles[ShaderType.VertexShader].Profile, SharpDX.D3DCompiler.ShaderFlags.Debug,
        //                SharpDX.D3DCompiler.EffectFlags.None, null, FileIncludeHandler.Default)),
        //        //SharpDX.D3DCompiler.ShaderBytecode.Compile()
        //        PixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
        //                ShaderFiles[ShaderType.PixelShader].File, ShaderFiles[ShaderType.PixelShader].EntryPoint, ShaderFiles[ShaderType.PixelShader].Profile, SharpDX.D3DCompiler.ShaderFlags.Debug,
        //                SharpDX.D3DCompiler.EffectFlags.None, null, FileIncludeHandler.Default)),
        //        RasterizerState = rasterizerStateDesc,
        //        BlendState = BlendStateDescription.Default(),
        //        DepthStencilFormat = Format.D32_Float,
        //        DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
        //        SampleMask = int.MaxValue,
        //        PrimitiveTopologyType = PrimitiveTopologyType.Point,
        //        RenderTargetCount = 1,
        //        Flags = PipelineStateFlags.None,
        //        SampleDescription = new SampleDescription(1, 0),
        //        StreamOutput = new StreamOutputDescription()
        //    };
        //gpsDesc.RenderTargetFormats[0] = Format.R8G8B8A8_UNorm;
        //    graphicPLStatePoint = device.CreateGraphicsPipelineState(gpsDesc);

        //    GraphicsPipelineStateDescription gpsDesc3 = new GraphicsPipelineStateDescription
        //    {
        //        InputLayout = new InputLayoutDescription(inputElementDescs),
        //        RootSignature = graphicRootSignature,
        //        VertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
        //                ShaderFiles[ShaderType.VertexShader].File, ShaderFiles[ShaderType.VertexShader].EntryPoint, ShaderFiles[ShaderType.VertexShader].Profile, SharpDX.D3DCompiler.ShaderFlags.Debug,
        //                SharpDX.D3DCompiler.EffectFlags.None, null, FileIncludeHandler.Default)),
        //        //SharpDX.D3DCompiler.ShaderBytecode.Compile()
        //        PixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
        //                ShaderFiles[ShaderType.PixelShader].File, ShaderFiles[ShaderType.PixelShader].EntryPoint, ShaderFiles[ShaderType.PixelShader].Profile, SharpDX.D3DCompiler.ShaderFlags.Debug,
        //                SharpDX.D3DCompiler.EffectFlags.None, null, FileIncludeHandler.Default)),
        //        RasterizerState = rasterizerStateDesc,
        //        BlendState = BlendStateDescription.Default(),
        //        DepthStencilFormat = Format.D32_Float,
        //        DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
        //        SampleMask = int.MaxValue,
        //        PrimitiveTopologyType = PrimitiveTopologyType.Line,
        //        RenderTargetCount = 1,
        //        Flags = PipelineStateFlags.None,
        //        SampleDescription = new SampleDescription(1, 0),
        //        StreamOutput = new StreamOutputDescription()
        //    };
        //gpsDesc3.RenderTargetFormats[0] = Format.R8G8B8A8_UNorm;
        //    graphicPLStateLine = device.CreateGraphicsPipelineState(gpsDesc3);

        void WaitForPreviousFrame()
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
    }
}
