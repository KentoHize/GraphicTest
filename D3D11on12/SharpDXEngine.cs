using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicLibrary;
using GraphicLibrary.Items;
using SharpDX.Direct3D12;
//using SharpDX.Direct2D1;
using Device = SharpDX.Direct3D12.Device;
using Device12 = SharpDX.Direct3D11.Device11On12;
using Device11 = SharpDX.Direct3D11.Device;
using Texture2D11 = SharpDX.Direct3D11.Texture2D;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using Resource = SharpDX.Direct3D12.Resource;
using Factory4 = SharpDX.DXGI.Factory4;
using SharpDX;
using SharpDX.DXGI;
using Filter = SharpDX.Direct3D12.Filter;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
            ShaderFiles = new Dictionary<ShaderType, ShaderFileInfo>
            {
                {ShaderType.VertexShader, new ShaderFileInfo(GLShaderFile, ShaderType.VertexShader) },
                {ShaderType.PixelShader, new ShaderFileInfo(GLShaderFile, ShaderType.PixelShader) },
            };

            //Device device22 = new Device();
            //Device11 device11 = Device11.CreateFromDirect3D12(device22, SharpDX.Direct3D11.DeviceCreationFlags.None,
            //    null, null);
        }

        public void LoadSetting(SharpDXSetting setting)
        {
            FrameCount = setting.FrameCount;
            viewport = setting.Viewport;
            //var factory1 = new Factory1();
            device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            //device.CreateFromDirect3D12(device);
            //device = new Device();

            //Device11 device11 = Device11.CreateFromDirect3D12(device, SharpDX.Direct3D11.DeviceCreationFlags.None,
            //  null, null);

            using (Factory4 factory = new Factory4())
            {
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
            //infoQueue = device.QueryInterface<InfoQueue>();

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

            Device11 device11 = Device11.CreateFromDirect3D12(device, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport,
  null, null, commandQueue);

            CretatePipleLine(setting);
        }

        void CretatePipleLine(SharpDXSetting setting)
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

        public void Get2DSurface()
        {
            //DebugInterface.Get().;
            
          
        }

        /*
        public (Surface, Texture2D11) Get2DSurface(Resource D12Resource)
        {
            var d2dFactory = new SharpDX.Direct2D1.Factory1();
            //var dxgiFactory = new SharpDX.DXGI.Factory1();
            //Adapter adapter = dxgiFactory.GetAdapter(0);
            //Device device22 = new Device();            
            //Device11 device11 = Device11.CreateFromDirect3D12(device22, SharpDX.Direct3D11.DeviceCreationFlags.None,
                null, null);
            //Device11 device11 = Device11.CreateFromDirect3D12(device, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport,
            //        new[] { SharpDX.Direct3D.FeatureLevel.Level_11_1 }, null, commandQueue);
            Device11 device11 = new Device11(SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport | SharpDX.Direct3D11.DeviceCreationFlags.Debug, new[] { SharpDX.Direct3D.FeatureLevel.Level_11_1 } );
            Device12 device12 = new Device12(device.NativePointer);


            //var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, 512, 512, 1, 1, 1, 0, ResourceFlags.AllowRenderTarget, TextureLayout.Unknown, 0);            
            //texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
            //ResourceDescription.Texture2D()
            //Surface surface2 = texture.QueryInterface<Surface>();

            SharpDX.Direct3D11.D3D11ResourceFlags format = new SharpDX.Direct3D11.D3D11ResourceFlags
            {
                BindFlags = (int)SharpDX.Direct3D11.BindFlags.RenderTarget,
                CPUAccessFlags = (int)SharpDX.Direct3D11.CpuAccessFlags.None
            };
            SharpDX.Direct3D11.Resource d11Resource;
            //device12.CreateWrappedResource(renderTargets[0], format, (int)ResourceStates.RenderTarget,
            //    (int)ResourceStates.Present, typeof(SharpDX.Direct3D11.Resource).GUID,
            //    out d11Resource);

            
            Texture2D11 texture2 = new Texture2D11(device11, new SharpDX.Direct3D11.Texture2DDescription
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
            
            Surface surface = texture2.QueryInterface<Surface>();

            var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, 512, 512, 1, 1, 1, 0, ResourceFlags.None);
            var textureUploadHeap = device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, textureDesc, ResourceStates.GenericRead);
            
            //var handle = GCHandle.Alloc(data.Textures[i].Data, GCHandleType.Pinned);
            //ptr = Marshal.UnsafeAddrOfPinnedArrayElement(data.Textures[i].Data, 0);
            //textureUploadHeap.WriteToSubresource(0, null, ptr, 4 * data.Textures[i].Width, data.Textures[i].Data.Length);
            //handle.Free();
            //commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);



            var rtp = new SharpDX.Direct2D1.RenderTargetProperties
            {
                MinLevel = SharpDX.Direct2D1.FeatureLevel.Level_10,
                Type = SharpDX.Direct2D1.RenderTargetType.Hardware,
                PixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)
            };
            var renderTarget2D = new SharpDX.Direct2D1.RenderTarget(d2dFactory, surface, rtp);
            return (surface, texture2);
        }
        */

        public byte[] GetFontData()
        {
            byte[] result;

            //var factory2D = new SharpDX.Direct2D1.Factory(SharpDX.Direct2D1.FactoryType.SingleThreaded, SharpDX.Direct2D1.DebugLevel.Information);            
            //Device11 device11 = new Device11(SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport | SharpDX.Direct3D11.DeviceCreationFlags.Debug, new[] { SharpDX.Direct3D.FeatureLevel.Level_11_1 });
            //var d2dDevice = new SharpDX.Direct2D1.Device(device11.QueryInterface<SharpDX.DXGI.Device>());

            //var d2dContext = new SharpDX.Direct2D1.DeviceContext(d2dDevice, SharpDX.Direct2D1.DeviceContextOptions.None);
            //var d2PixelFormat = new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
            //var d2dBitmapProps = new SharpDX.Direct2D1.BitmapProperties1(d2PixelFormat, 96, 96, SharpDX.Direct2D1.BitmapOptions.Target | SharpDX.Direct2D1.BitmapOptions.CpuRead | SharpDX.Direct2D1.BitmapOptions.CannotDraw);
            //var d2dRenderTarget = new SharpDX.Direct2D1.Bitmap1(d2dContext, new Size2(512, 512), d2dBitmapProps);
            //d2dContext.Target = d2dRenderTarget;

            //d2dContext.BeginDraw();
            //var dwf = new SharpDX.DirectWrite.Factory();
            //var tf = new SharpDX.DirectWrite.TextFormat(dwf, "Consolas", 24);
            //d2dContext.DrawText("aaa", tf, new SharpDX.RectangleF(0, 0, 600, 600), new SharpDX.Direct2D1.SolidColorBrush(d2dContext, new Color4(1, 1, 1, 1)));
            //d2dContext.EndDraw();
            //DataRectangle map = d2dRenderTarget.Map(SharpDX.Direct2D1.MapOptions.Read);           
            //result = new byte[(int)d2dRenderTarget.Size.Width * (int)d2dRenderTarget.Size.Height * 4];            
            //Marshal.Copy(map.DataPointer, result, 0, result.Length);
            //d2dRenderTarget.Unmap();
            Bitmap bitmap = new Bitmap(512, 512);
            System.Drawing.RectangleF rectf = new System.Drawing.RectangleF(0, 0, 512, 512);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.Clear(System.Drawing.Color.White);
            string s = "現代人常說隔夜菜不健康，但腎臟科醫師江守山表示，經過研究發現，其實米飯比菜更容易壞掉，也更容易產生毒素。對此，江守山也分享自己曾經遇過的病例，一名30幾歲、腎功能不佳的男子，某天因吃到腐壞的米飯，嚴重腹痛、血壓驟降，險些洗腎。\r\n\r\n腎臟科醫師江守山在節目《健康好生活》中表示，上述病例是全家一起誤食到腐敗的米飯，均食物中毒，「大家一起中獎」。而該男子本身腎功能差，雖然未達洗腎程度，但當天因食物中毒、嚴重腹痛，迷走神經太過興奮，導致血壓驟降，全身冒冷汗，出現所謂的「休克型低血壓」，當時收縮壓甚至來到62、舒張壓近40毫米汞柱。\r\n\r\n他表示，腎臟功能差者，若血壓急遽至該男子的數值，可以直接造成腎小管壞死、重創腎功能，嚴重恐要終身洗腎，「所以不要忽視拉肚子這個小病。」\r\n\r\n江守山補充，「國外也發生過很多次，就是都認為飯不會壞，尤其很多廚師都會教說，冰過或放過的老飯去炒，才會粒粒分明和好吃」，但米飯若腐敗其實很容易長出仙人掌桿菌，並產生「可抗熱」的內毒素，因此即便煮熟都還是無法消滅此菌，吃下去照常食物中毒、上吐下瀉，甚至腹部不適、血壓驟降、嚴重傷害腎臟血管，造成腎臟永久性傷害。\r\n\r\n所以江守山也再次提醒，飯絕對不是不會壞，「只是飯看起來比較乾燥，所以看不出來它已經壞掉」，民眾千萬別輕忽「炒飯症候群」食物中毒的風險，嚴重可能會造成腎臟等器官敗壞，慘淪終身洗腎。";
            g.DrawString(s, new Font("Consolas", 14), Brushes.Black, rectf);
            g.Flush();
            Get2DSurface();
            //bitmap.Save(@"C:\Programs\TestArea\yourText.bmp");

            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            result = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(data.Scan0, result, 0, result.Length);
            bitmap.UnlockBits(data);
            return result;
        }

        public void LoadStaticData(SharpDXStaticData data)
        {
            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, graphicPLState);

            DescriptorHeapDescription shaderResourceBufferViewHeapDesc = new DescriptorHeapDescription
            {
                DescriptorCount = 2,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            };
            shaderResourceBufferViewHeap = device.CreateDescriptorHeap(shaderResourceBufferViewHeapDesc);
            cruHandle = shaderResourceBufferViewHeap.CPUDescriptorHandleForHeapStart;

            Stopwatch sw = Stopwatch.StartNew();
            byte[] fontTexture = GetFontData();
            sw.Stop();
            Debug.Print($"Get Font:{sw.ElapsedMilliseconds}");
            
            //var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Textures[0].Width, data.Textures[0].Height, 1, 1, 1, 0, ResourceFlags.None);
            var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, 512, 512, 1, 1, 1, 0, ResourceFlags.None);
            texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
            var textureUploadHeap = device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, textureDesc, ResourceStates.GenericRead);
            var handle = GCHandle.Alloc(fontTexture, GCHandleType.Pinned);
            ptr = Marshal.UnsafeAddrOfPinnedArrayElement(fontTexture, 0);
            textureUploadHeap.WriteToSubresource(0, null, ptr, 4 * 512, fontTexture.Length);
            handle.Free();
            commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);
            commandList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);
            var srvDesc = new ShaderResourceViewDescription
            {
                Shader4ComponentMapping = Ar3DMachine.DefaultComponentMapping,
                Format = textureDesc.Format,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = { MipLevels = 1 },
            };
            device.CreateShaderResourceView(texture, srvDesc, cruHandle);
            cruHandle += cruDescriptorSize;
            for (int i = 0; i < data.Textures.Length; i++)
            { 
                //ArraySize = 1,
                //BindFlags = SharpDX.Direct3D11.BindFlags.RenderTarget | SharpDX.Direct3D11.BindFlags.ShaderResource,
                //CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                //Format = Format.B8G8R8A8_UNorm,
                //Height = 512,
                //Width = 512,
                //MipLevels = 1,
                //OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.SharedKeyedmutex,
                //SampleDescription = new SampleDescription(1, 0),
                //Usage = SharpDX.Direct3D11.ResourceUsage.Default

                //textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, 512, 512, 1, 1, 1, 0, ResourceFlags.None);
                //textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Textures[i].Width, data.Textures[i].Height);                
                //texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
                //var textureUploadHeap = device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, textureDesc, ResourceStates.GenericRead);
                //var handle = GCHandle.Alloc(data.Textures[i].Data, GCHandleType.Pinned);
                //ptr = Marshal.UnsafeAddrOfPinnedArrayElement(data.Textures[i].Data, 0);
                //textureUploadHeap.WriteToSubresource(0, null, ptr, 4 * data.Textures[i].Width, data.Textures[i].Data.Length);
                //handle.Free();
                //commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);
                //commandList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);
              
            }

            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);

            WaitForPreviousFrame();
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

            constantBuffer = new Resource[2];
            constantBuffer[0] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);
            ptr = constantBuffer[0].Map(0);
            Utilities.Write(ptr, new ArFloatMatrix44[] { data.TransformMartrix }, 0, 1);            
            constantBuffer[0].Unmap(0);

            constantBuffer[1] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);
            ptr = constantBuffer[1].Map(0);
            Utilities.Write(ptr, new int[] { 1 }, 0, 1);
            constantBuffer[1].Unmap(0);

            
        }

        public void Render()
        {
            commandAllocator.Reset();
            commandList.Reset(commandAllocator, graphicPLState);
            commandList.SetGraphicsRootSignature(graphicRootSignature);

            commandList.SetViewport(viewport);
            commandList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle(0, 0, (int)viewport.Width, (int)viewport.Height));

            commandList.SetGraphicsRootConstantBufferView(0, constantBuffer[0].GPUVirtualAddress);
            commandList.SetGraphicsRootConstantBufferView(1, constantBuffer[1].GPUVirtualAddress);
            commandList.SetDescriptorHeaps(new DescriptorHeap[] { shaderResourceBufferViewHeap });
            commandList.SetGraphicsRootDescriptorTable(2, shaderResourceBufferViewHeap.GPUDescriptorHandleForHeapStart);
            //commandList.SetGraphicsRootShaderResourceView(2, texture.GPUVirtualAddress);

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

        public void Update()
        {

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
            if(renderTargets != null)            
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
