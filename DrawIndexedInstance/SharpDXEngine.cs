﻿using System;
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
using Color = SharpDX.Color;
using GraphicLibrary.Items;


namespace GraphicLibrary
{
    public class SharpDXEngine : IDisposable
    {
        public int FrameCount { get; private set; }

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
        int rtvDescriptorSize;

        RootSignature computeRootSignature;
        RootSignature graphicRootSignature;

        ViewportF viewport;
        int frameIndex;

        AutoResetEvent fenceEvent;
        Fence fence;
        int fenceValue;

        ArFloatVector4 backgroundColor;

        VertexBufferView[] verticesBufferView;
        IndexBufferView[] indicesBufferView;
        Resource[] verticesBuffer;
        Resource[] indicesBuffer;
        int indicesCount;

        internal Dictionary<ShaderType, ShaderFileInfo> ShaderFiles { get; set; }

        public SharpDXEngine()
        {
            FrameCount = 2;
            const string GLShaderFile = @"C:\Programs\GraphicTest\DrawIndexedInstance\Shaders.hlsl";

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
            //graphicRootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());
            graphicRootSignature = device.CreateRootSignature(new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout).Serialize());

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

            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, graphicPLState);
            commandList.Close();
        }

        public void Load(SharpDXData data)
        {
            backgroundColor = data.BackgroundColor;
            verticesBufferView = new VertexBufferView[data.VerticesData.Length];
            verticesBuffer = new Resource[data.VerticesData.Length];
            indicesBufferView = new IndexBufferView[data.VerticesData.Length];
            indicesBuffer = new Resource[data.VerticesData.Length];
            for (int i = 0; i < data.VerticesData.Length; i++)
            {
                int verticesBufferSize = Utilities.SizeOf(data.VerticesData[i].ColorVertices);
                verticesBuffer[i] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(verticesBufferSize), ResourceStates.GenericRead);
                IntPtr pVertexDataBegin = verticesBuffer[i].Map(0);
                Utilities.Write(pVertexDataBegin, data.VerticesData[i].ColorVertices, 0, data.VerticesData[i].ColorVertices.Length);
                verticesBuffer[i].Unmap(0);

                verticesBufferView[i] = new VertexBufferView
                {
                    BufferLocation = verticesBuffer[i].GPUVirtualAddress,
                    StrideInBytes = Utilities.SizeOf<ArColorVertex>(),
                    SizeInBytes = verticesBufferSize
                };

                int indicesBufferSize = Utilities.SizeOf(data.VerticesData[i].Indices);
                indicesCount = data.VerticesData[i].Indices.Length;
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
            }

            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;
            fenceEvent = new AutoResetEvent(false);
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

            //commandList.SetDescriptorHeaps(new DescriptorHeap[] { constantBufferViewHeap });
            //commandList.SetGraphicsRootDescriptorTable(0, constantBufferViewHeap.GPUDescriptorHandleForHeapStart);

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
