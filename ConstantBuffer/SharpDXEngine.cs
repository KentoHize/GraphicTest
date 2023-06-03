using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Device = SharpDX.Direct3D12.Device;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using Resource = SharpDX.Direct3D12.Resource;
using ShaderBytecode = SharpDX.Direct3D12.ShaderBytecode;

namespace ConstantBuffer
{
    public class SharpDXEngine
    {
        public const string shaderFile = @"C:\Programs\GraphicTest\ConstantBuffer\shaders.hlsl";

        Device device;
        CommandQueue commandQueue;
        SwapChain3 swapChain;
        PipelineState pipelineState;
        InfoQueue iq;

        readonly Resource[] renderTargets = new Resource[2];
        GraphicsCommandList commandList;
        CommandAllocator commandAllocator;

        RootSignature rootSignature;
        DescriptorHeap renderTargetViewHeap;
        DescriptorHeap constantBufferViewHeap;
        VertexBufferView vertexBufferView;
        //BufferView constantBufferView;
        Resource vertexBuffer;
        Resource[] constantBuffer = new Resource[3];
        IntPtr constantBufferPointer;
        int rtvDescriptorSize;
        int cruDescriptorSize;

        ViewportF viewport;
        Color4 backgroundColor;
        Vertex[] gd;

        int frameIndex;
        AutoResetEvent fenceEvent;

        Fence fence;
        int fenceValue;
        public void Initialize(SharpDXSetting setting)
        {
            viewport = setting.Viewport;
            
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
            }
            iq = device.QueryInterface<InfoQueue>();
            frameIndex = swapChain.CurrentBackBufferIndex;

            var rtvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = 2,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.RenderTargetView
            };
            renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);

            rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);
            var rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            for (int n = 0; n < 2; n++)
            {
                renderTargets[n] = swapChain.GetBackBuffer<Resource>(n);
                device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
                rtvHandle += rtvDescriptorSize;
            }
            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);

            //Init Constant Buffer Heap
            var cbvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = 3,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            };
            constantBufferViewHeap = device.CreateDescriptorHeap(cbvHeapDesc);

            cruDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
            var cruHandle = constantBufferViewHeap.CPUDescriptorHandleForHeapStart;
            CB1[] cbDataArray =
                new CB1[]
                {
                    new CB1
                    {
                        Position = new Vector4(0.25f, 0, 0, 0),
                        Color = new Vector4(1.0f, 1.0f, 0, 1.0f),
                    },
                    new CB1
                    {
                        Position = new Vector4(0.25f, 0, 0, 0),
                        Color = new Vector4(1.0f, 0.0f, 0, 1.0f),
                    },
                    new CB1
                    {
                        Position = new Vector4(0.25f, 0, 0, 0),
                        Color = new Vector4(0, 0.0f,1.0f, 1.0f),
                    }
             };

            for (int i = 0; i < 3; i++)
            {
                constantBuffer[i] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);
                var cbvDesc = new ConstantBufferViewDescription()
                {
                    BufferLocation = constantBuffer[i].GPUVirtualAddress,
                    SizeInBytes = (Utilities.SizeOf<CB1>() + 255) & ~255
                };

                device.CreateConstantBufferView(cbvDesc, cruHandle);
                cruHandle += cruDescriptorSize;
                constantBufferPointer = constantBuffer[i].Map(0);
                Utilities.Write(constantBufferPointer, ref cbDataArray[i]);
                //constantBuffer[i].Unmap(0);
            }
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
                            RegisterSpace = 0,
                            OffsetInDescriptorsFromTableStart = 0,
                            DescriptorCount = 3
                        })
               });
            rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());
        }

        public struct CB1
        {
            public Vector4 Position { get; set; }
            public Vector4 Color { get; set; }
        };

        public void Load(SharpDXData data)
        {
            backgroundColor = new Color4(new Vector4(data.BackgroundColor.R,
                data.BackgroundColor.G, data.BackgroundColor.B,
                data.BackgroundColor.A));

#if DEBUG
            var vertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(shaderFile, "VSMain", "vs_5_1", SharpDX.D3DCompiler.ShaderFlags.Debug));
#else
            var vertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "VSMain", "vs_5_1"));
#endif

#if DEBUG
            var pixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(shaderFile, "PSMain", "ps_5_1", SharpDX.D3DCompiler.ShaderFlags.Debug));
#else
            var pixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "PSMain", "ps_5_1"));
#endif

            var inputElementDescs = new[]
           {
                    new InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,12,0)
            };

            var psoDesc = new GraphicsPipelineStateDescription()
            {
                InputLayout = new InputLayoutDescription(inputElementDescs),
                RootSignature = rootSignature,
                VertexShader = vertexShader,
                PixelShader = pixelShader,
                RasterizerState = RasterizerStateDescription.Default(),
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
            pipelineState = device.CreateGraphicsPipelineState(psoDesc);

            gd = data.GraphicData[0].Data;
            int vertexBufferSize = Utilities.SizeOf(gd);
            vertexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize), ResourceStates.GenericRead);
            IntPtr pVertexDataBegin = vertexBuffer.Map(0);
            Utilities.Write(pVertexDataBegin, gd, 0, gd.Length);
            vertexBuffer.Unmap(0);
            vertexBufferView = new VertexBufferView();
            vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
            vertexBufferView.StrideInBytes = Utilities.SizeOf<Vertex>();
            vertexBufferView.SizeInBytes = vertexBufferSize;

            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, pipelineState);
            commandList.Close();

            // Create synchronization objects.
            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;

            // Create an event handle to use for frame synchronization.
            fenceEvent = new AutoResetEvent(false);
        }
        public void Update()
        {
            //constantBufferPointer = constantBuffer[i].Map(0);
            //Utilities.Write(constantBufferPointer, ref cbDataArray[i]);
        }

        public void UpdateConstantBuffer(CB1 cb)
        {
            constantBufferPointer = constantBuffer[2].Map(0);
            Utilities.Write(constantBufferPointer, ref cb);
            Render();
        }

        public void Render()
        {
            commandAllocator.Reset();
            commandList.Reset(commandAllocator, pipelineState);
            commandList.SetGraphicsRootSignature(rootSignature);

            commandList.SetViewport(viewport);
            commandList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle(0, 0, (int)viewport.Width, (int)viewport.Height));

            commandList.SetDescriptorHeaps(new DescriptorHeap[] { constantBufferViewHeap });
            commandList.SetGraphicsRootDescriptorTable(0, constantBufferViewHeap.GPUDescriptorHandleForHeapStart);
            //GpuDescriptorHandle gdh = constantBufferViewHeap.GPUDescriptorHandleForHeapStart;
            //commandList.SetGraphicsRootDescriptorTable(0, gdh);
            //gdh += cruDescriptorSize;
            //commandList.SetGraphicsRootDescriptorTable(1, gdh);
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);

            CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += frameIndex * rtvDescriptorSize;
            commandList.SetRenderTargets(rtvHandle, null);

            commandList.ClearRenderTargetView(rtvHandle, backgroundColor, 0, null);
            commandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            commandList.SetVertexBuffer(0, vertexBufferView);
            commandList.DrawInstanced(3, 1, 0, 0);
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);
            commandList.Close();

            commandQueue.ExecuteCommandList(commandList);

            // Present the frame.
            swapChain.Present(1, 0);

            int localFence = fenceValue;
            commandQueue.Signal(this.fence, localFence);
            fenceValue++;

            // Wait until the previous frame is finished.
            if (this.fence.CompletedValue < localFence)
            {
                this.fence.SetEventOnCompletion(localFence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
                fenceEvent.WaitOne();
            }

            frameIndex = swapChain.CurrentBackBufferIndex;
        }
    }
}
