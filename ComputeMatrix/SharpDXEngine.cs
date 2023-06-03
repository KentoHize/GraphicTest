using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D12.Device;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using Resource = SharpDX.Direct3D12.Resource;
using GraphicLibrary.Items;

namespace GraphicLibrary
{
    public class SharpDXEngine : IDisposable
    {
        public int FrameCount { get; private set; }
        public const int ConstantBufferViewCount = 1;

        Device device;
        SwapChain3 swapChain;
        CommandQueue commandQueue;
        PipelineState graphicPLState;
        PipelineState computePLState;
        InfoQueue infoQueue;

        

        GraphicsCommandList commandList;
        CommandAllocator commandAllocator;
        Resource[] renderTargets;
        DescriptorHeap renderTargetViewHeap;
        DescriptorHeap constantBufferViewHeap;
        int rtvDescriptorSize;
        int cruDescriptorSize;
        IntPtr ptr;

        RootSignature computeRootSignature;
        RootSignature graphicRootSignature;

        ViewportF viewport;
        int frameIndex;

        AutoResetEvent fenceEvent;
        Fence fence;
        int fenceValue;

        //ArVertex[][] verticesData;
        //int[][] indicesData;
        ArFloatVector4 backgroundColor;

        VertexBufferView[] verticesBufferView;
        IndexBufferView[] indicesBufferView;
        Resource[] verticesBuffer;
        Resource[] indicesBuffer;
        Resource[] constantBuffer;
        int indicesCount;

        internal Dictionary<ShaderType, ShaderFileInfo> ShaderFiles { get; set; }

        public SharpDXEngine()
        {
            FrameCount = 2;
            const string GLShaderFile = @"C:\Programs\GraphicTest\ComputeMatrix\shaders.hlsl";

            ShaderFiles = new Dictionary<ShaderType, ShaderFileInfo>
            {
                {ShaderType.VertexShader, new ShaderFileInfo(GLShaderFile, ShaderType.VertexShader) },
                {ShaderType.PixelShader, new ShaderFileInfo(GLShaderFile, ShaderType.PixelShader) },
            };
        }

        public void Initialize(SharpDXSetting setting)
        {
            LoadSetting(setting);
        }

        struct Matrix44
        {
            public Matrix44(float n11, float n12, float n13, float n14, float n21, float n22, float n23, float n24, float n31, float n32, float n33, float n34, float n41, float n42, float n43, float n44)
            {
                this.n11 = n11;
                this.n12 = n12;
                this.n13 = n13;
                this.n14 = n14;
                this.n21 = n21;
                this.n22 = n22;
                this.n23 = n23;
                this.n24 = n24;
                this.n31 = n31;
                this.n32 = n32;
                this.n33 = n33;
                this.n34 = n34;
                this.n41 = n41;
                this.n42 = n42;
                this.n43 = n43;
                this.n44 = n44;
            }

            public float n11 { get; set; }
            public float n12 { get; set; }
            public float n13 { get; set; }
            public float n14 { get; set; }
            public float n21 { get; set; }
            public float n22 { get; set; }
            public float n23 { get; set; }
            public float n24 { get; set; }
            public float n31 { get; set; }
            public float n32 { get; set; }
            public float n33 { get; set; }
            public float n34 { get; set; }
            public float n41 { get; set; }
            public float n42 { get; set; }
            public float n43 { get; set; }
            public float n44 { get; set; }
        }

        /// <summary>
        /// (Can't Reload)
        /// </summary>
        /// <param name="setting"></param>
        public void LoadSetting(SharpDXSetting setting)
        {
            viewport = setting.Viewport;
            FrameCount = setting.FrameCount;
#if DEBUG
            DebugInterface.Get().EnableDebugLayer();
#endif
            //Temp
            if (device != null)
                return;

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
            
            var cbvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = ConstantBufferViewCount,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            };
            constantBufferViewHeap = device.CreateDescriptorHeap(cbvHeapDesc);

            cruDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
            var cruHandle = constantBufferViewHeap.CPUDescriptorHandleForHeapStart;
            Matrix44 m44 = new Matrix44();
            m44.n11 = 2;
            m44.n22 = 2;
            m44.n33 = 2;
            m44.n44 = 1;

            constantBuffer = new Resource[ConstantBufferViewCount];
            for (int i = 0; i < ConstantBufferViewCount; i++)
            {
                constantBuffer[i] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);
                var cbvDesc = new ConstantBufferViewDescription()
                {
                    BufferLocation = constantBuffer[i].GPUVirtualAddress,
                    SizeInBytes = (Utilities.SizeOf<Matrix44>() + 255) & ~255
                };

                device.CreateConstantBufferView(cbvDesc, cruHandle);
                cruHandle += cruDescriptorSize;
                ptr = constantBuffer[i].Map(0);
                Utilities.Write(ptr, ref m44);
                constantBuffer[i].Unmap(0);
            }

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
                            DescriptorCount = ConstantBufferViewCount
                        })
            });
            graphicRootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());
            //graphicRootSignature = device.CreateRootSignature(new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout).Serialize());

            InputElement[] inputElementDescs = new InputElement[]
              {
                    new InputElement("POSITION",0,Format.R32G32B32_SInt,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,12,0)
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
#if DEBUG
                VertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
                        ShaderFiles[ShaderType.VertexShader].File, ShaderFiles[ShaderType.VertexShader].EntryPoint, ShaderFiles[ShaderType.VertexShader].Profile, SharpDX.D3DCompiler.ShaderFlags.Debug)),
                PixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
                        ShaderFiles[ShaderType.PixelShader].File, ShaderFiles[ShaderType.PixelShader].EntryPoint, ShaderFiles[ShaderType.PixelShader].Profile, SharpDX.D3DCompiler.ShaderFlags.Debug)),
#else
                VertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
                        ShaderFiles[ShaderType.VertexShader].File, ShaderFiles[ShaderType.VertexShader].EntryPoint, ShaderFiles[ShaderType.VertexShader].Profile)),
                PixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(
                        ShaderFiles[ShaderType.PixelShader].File, ShaderFiles[ShaderType.PixelShader].EntryPoint, ShaderFiles[ShaderType.PixelShader].Profile)),
#endif
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

            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, graphicPLState);
            commandList.Close();

            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;
            fenceEvent = new AutoResetEvent(false);
        }

        public void Load(SharpDXData data)
        {
            backgroundColor = data.BackgroundColor;
            verticesBufferView = new VertexBufferView[data.VerteicesData.Length];
            verticesBuffer = new Resource[data.VerteicesData.Length];
            indicesBufferView = new IndexBufferView[data.VerteicesData.Length];
            indicesBuffer = new Resource[data.VerteicesData.Length];
            for (int i = 0; i < data.VerteicesData.Length; i++)
            {
                int verticesBufferSize = Utilities.SizeOf(data.VerteicesData[i].Verteices);
                verticesBuffer[i] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(verticesBufferSize), ResourceStates.GenericRead);
                IntPtr pVertexDataBegin = verticesBuffer[i].Map(0);
                Utilities.Write(pVertexDataBegin, data.VerteicesData[i].Verteices, 0, data.VerteicesData[i].Verteices.Length);
                verticesBuffer[i].Unmap(0);

                verticesBufferView[i] = new VertexBufferView
                {
                    BufferLocation = verticesBuffer[i].GPUVirtualAddress,
                    StrideInBytes = Utilities.SizeOf<ArVertex>(),
                    SizeInBytes = verticesBufferSize
                };

                int indicesBufferSize = Utilities.SizeOf(data.VerteicesData[i].Indices);
                indicesCount = data.VerteicesData[i].Indices.Length;
                indicesBuffer[i] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(indicesBufferSize), ResourceStates.GenericRead);
                pVertexDataBegin = indicesBuffer[i].Map(0);
                Utilities.Write(pVertexDataBegin, data.VerteicesData[i].Indices, 0, data.VerteicesData[i].Indices.Length);
                indicesBuffer[i].Unmap(0);
                indicesBufferView[i] = new IndexBufferView
                {
                    BufferLocation = indicesBuffer[i].GPUVirtualAddress,
                    SizeInBytes = indicesBufferSize,
                    Format = Format.R32_UInt
                };
            }
        }

        public void Update()
        {

        }

        public void Render()
        {
            commandAllocator.Reset();
            commandList.Reset(commandAllocator, graphicPLState);
            commandList.SetGraphicsRootSignature(graphicRootSignature);

            commandList.SetViewport(viewport);
            commandList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle(0, 0, (int)viewport.Width, (int)viewport.Height));

            commandList.SetDescriptorHeaps(new DescriptorHeap[] { constantBufferViewHeap });
            commandList.SetGraphicsRootDescriptorTable(0, constantBufferViewHeap.GPUDescriptorHandleForHeapStart);

            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);
            CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += frameIndex * rtvDescriptorSize;
            commandList.SetRenderTargets(rtvHandle, null);
            commandList.ClearRenderTargetView(rtvHandle, new Color4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W), 0, null);
            commandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            commandList.SetVertexBuffer(0, verticesBufferView[0]);
            commandList.SetIndexBuffer(indicesBufferView[0]);
            commandList.DrawIndexedInstanced(indicesCount, 1, 0, 0, 0);

            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);
            commandList.Close();

            commandQueue.ExecuteCommandList(commandList);

            // Present the frame.
            swapChain.Present(1, 0);

            int localFence = fenceValue;
            commandQueue.Signal(fence, localFence);
            fenceValue++;

            // Wait until the previous frame is finished.
            if (fence.CompletedValue < localFence)
            {
                fence.SetEventOnCompletion(localFence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
                fenceEvent.WaitOne();
            }

            frameIndex = swapChain.CurrentBackBufferIndex;
        }

        public void Dispose()
        {
            device.Dispose();
        }
    }
}
