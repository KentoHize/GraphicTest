﻿using SharpDX.Direct3D12;
using SharpDX.DXGI;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Device = SharpDX.Direct3D12.Device;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using Resource = SharpDX.Direct3D12.Resource;
using GraphicLibrary.Items;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;
using System.Diagnostics;
using GraphicLibrary;
//using SharpDX.D3DCompiler;

namespace ReplaceHeap
{
    public class SharpDXEngine : IDisposable
    {
        public int FrameCount { get; private set; }
        public int ConstantBufferViewCount = 2;
        public int ShaderResourceViewCount = 0;

        Device device;
        SwapChain3 swapChain;
        CommandQueue commandQueue;
        PipelineState graphicPLState;
        PipelineState computePLState;
        InfoQueue infoQueue;

        GraphicsCommandList commandList;
        GraphicsCommandList[] bundles;
        CommandAllocator commandAllocator;
        Resource[] renderTargets;
        DescriptorHeap renderTargetViewHeap;
        DescriptorHeap constantBufferViewHeap;
        DescriptorHeap constantBufferViewHeap2;
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
        Resource[] constantBuffer2;
        Resource[] shaderResource;
        
        internal Dictionary<ShaderType, ShaderFileInfo> ShaderFiles { get; set; }

        public void Initialize()
        {
#if DEBUG
            DebugInterface.Get().EnableDebugLayer();
#endif
            FrameCount = 2;
            const string GLShaderFile = @"C:\Programs\GraphicTest\ReplaceHeap\shaders.hlsl";

            ShaderFiles = new Dictionary<ShaderType, ShaderFileInfo>
            {
                {ShaderType.VertexShader, new ShaderFileInfo(GLShaderFile, ShaderType.VertexShader) },
                {ShaderType.PixelShader, new ShaderFileInfo(GLShaderFile, ShaderType.PixelShader) },
            };
            
        }

        public void LoadSetting(SharpDXSetting setting)
        {
            viewport = setting.Viewport;
            FrameCount = setting.FrameCount;
            device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            using (Factory4 factory = new Factory4())
            {
                CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
                commandQueue = device.CreateCommandQueue(queueDesc);

                SwapChainDescription swapChainDesc = new SwapChainDescription()
                {
                    BufferCount = FrameCount,
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
                                DescriptorCount =  ConstantBufferViewCount
                            }
                            //new DescriptorRange
                            //{
                            //    RangeType = DescriptorRangeType.ShaderResourceView,
                            //    BaseShaderRegister = 0,
                            //    RegisterSpace = 0,
                            //    OffsetInDescriptorsFromTableStart = ConstantBufferViewCount,
                            //    DescriptorCount = ShaderResourceViewCount
                            //},
                ) },
              //new RootParameter[] { },
              new StaticSamplerDescription[]
              {
                    new StaticSamplerDescription(ShaderVisibility.Pixel, 0, 0)
                    {
                         Filter = Filter.MinimumMinMagMipPoint,
                         AddressUVW = TextureAddressMode.Border,
                    }
              });
            graphicRootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());
            //graphicRootSignature = device.CreateRootSignature(new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout).Serialize());

            

            InputElement[] inputElementDescs = new InputElement[]
            {
                    new InputElement("POSITION", 0, Format.R32G32B32_SInt,0,0),
                    new InputElement("NORMAL", 0, Format.R32G32B32_Float,12,0),
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
                StreamOutput = new StreamOutputDescription(),
            };
            
            psoDesc.RenderTargetFormats[0] = Format.R8G8B8A8_UNorm;            
            graphicPLState = device.CreateGraphicsPipelineState(psoDesc);
            //Debug.WriteLine(infoQueue.GetMessage(0).ToString());

            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;
            fenceEvent = new AutoResetEvent(false);
        }


        protected void CreateConstantBuffer()
        {
            var cbvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = ConstantBufferViewCount + ShaderResourceViewCount,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            };
            constantBufferViewHeap = device.CreateDescriptorHeap(cbvHeapDesc);
            cruDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
            cruHandle = constantBufferViewHeap.CPUDescriptorHandleForHeapStart;

            constantBuffer = new Resource[ConstantBufferViewCount];
            constantBuffer[0] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);
            var cbvDesc = new ConstantBufferViewDescription()
            {
                BufferLocation = constantBuffer[0].GPUVirtualAddress,
                SizeInBytes = (Utilities.SizeOf<ArFloatMatrix44>() + 255) & ~255
            };
            device.CreateConstantBufferView(cbvDesc, cruHandle);
            cruHandle += cruDescriptorSize;

            constantBuffer[1] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);
            cbvDesc = new ConstantBufferViewDescription()
            {
                BufferLocation = constantBuffer[1].GPUVirtualAddress,
                SizeInBytes = (Utilities.SizeOf<int>() + 255) & ~255
            };
            device.CreateConstantBufferView(cbvDesc, cruHandle);
            cruHandle += cruDescriptorSize;
           
            constantBufferViewHeap2 = device.CreateDescriptorHeap(cbvHeapDesc);
            cruHandle = constantBufferViewHeap2.CPUDescriptorHandleForHeapStart;
            constantBuffer2 = new Resource[ConstantBufferViewCount];

            constantBuffer2[0] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);
            constantBuffer2[1] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);            

            cbvDesc = new ConstantBufferViewDescription()
            {
                BufferLocation = constantBuffer2[0].GPUVirtualAddress,
                SizeInBytes = (Utilities.SizeOf<ArFloatMatrix44>() + 255) & ~255
            };
            device.CreateConstantBufferView(cbvDesc, cruHandle);
            cruHandle += cruDescriptorSize;

            device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);
            cbvDesc = new ConstantBufferViewDescription()
            {
                BufferLocation = constantBuffer2[1].GPUVirtualAddress,
                SizeInBytes = (Utilities.SizeOf<int>() + 255) & ~255
            };
            device.CreateConstantBufferView(cbvDesc, cruHandle);
            cruHandle += cruDescriptorSize;
        }

        public void LoadStaticData(SharpDXStaticData data)
        {
            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, graphicPLState);

            CreateConstantBuffer();// Can Improve

            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);
        }

        public void LoadData(SharpDXData data)
        {
            backgroundColor = data.BackgroundColor;
            verticesBufferView = new VertexBufferView[data.VerteicesData.Length];
            verticesBuffer = new Resource[data.VerteicesData.Length];
            indicesBufferView = new IndexBufferView[data.VerteicesData.Length];
            indicesBuffer = new Resource[data.VerteicesData.Length];
            transformMatrix = new ArFloatMatrix44[data.VerteicesData.Length];
            bundles = new GraphicsCommandList[data.VerteicesData.Length];
            for (int i = 0; i < data.VerteicesData.Length; i++)
            {
                int dataSize;
                if (data.VerteicesData[i].ColorVertices != null)
                    dataSize = ArColorVertex.ByteSize;
                else if (data.VerteicesData[i].TextureVertices != null)
                    dataSize = ArTextureVertex.ByteSize;
                else
                    dataSize = ArMixVertex.ByteSize;

                transformMatrix[i] = data.VerteicesData[i].TransformMartrix;
                int verticesBufferSize;
                if (data.VerteicesData[i].ColorVertices != null)
                    verticesBufferSize = Utilities.SizeOf(data.VerteicesData[i].ColorVertices);
                else if (data.VerteicesData[i].TextureVertices != null)
                    verticesBufferSize = Utilities.SizeOf(data.VerteicesData[i].TextureVertices);
                else
                    verticesBufferSize = Utilities.SizeOf(data.VerteicesData[i].MixVertices);
                verticesBuffer[i] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(verticesBufferSize), ResourceStates.GenericRead);
                IntPtr pVertexDataBegin = verticesBuffer[i].Map(0);
                if (data.VerteicesData[i].ColorVertices != null)
                    Utilities.Write(pVertexDataBegin, data.VerteicesData[i].ColorVertices, 0, data.VerteicesData[i].ColorVertices.Length);
                else if (data.VerteicesData[i].TextureVertices != null)
                    Utilities.Write(pVertexDataBegin, data.VerteicesData[i].TextureVertices, 0, data.VerteicesData[i].TextureVertices.Length);
                else
                    Utilities.Write(pVertexDataBegin, data.VerteicesData[i].MixVertices, 0, data.VerteicesData[i].MixVertices.Length);

                verticesBuffer[i].Unmap(0);
                verticesBufferView[i] = new VertexBufferView
                {
                    BufferLocation = verticesBuffer[i].GPUVirtualAddress,
                    StrideInBytes = dataSize,
                    SizeInBytes = verticesBufferSize
                };

                int indicesBufferSize = Utilities.SizeOf(data.VerteicesData[i].Indices);
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

                CommandAllocator bundleAllocator = device.CreateCommandAllocator(CommandListType.Bundle);

                bundles[i] = device.CreateCommandList(0, CommandListType.Bundle, bundleAllocator, graphicPLState);
                bundles[i].PrimitiveTopology = data.VerteicesData[i].PrimitiveTopology;
                bundles[i].SetVertexBuffer(0, verticesBufferView[i]);
                bundles[i].SetIndexBuffer(indicesBufferView[i]);
                bundles[i].DrawIndexedInstanced(data.VerteicesData[i].Indices.Length, 1, 0, 0, 0);
                
                bundles[i].Close();                
            }

            ptr = constantBuffer[0].Map(0);
            Utilities.Write(ptr, new ArFloatMatrix44[] { transformMatrix[0] }, 0, 1);
            //constantBuffer[0].Unmap(0);

            ptr = constantBuffer[1].Map(0);
            Utilities.Write(ptr, new int[] { data.VerteicesData[0].TextureIndex }, 0, 1);
            //constantBuffer[1].Unmap(0);

            ptr = constantBuffer2[0].Map(0);
            Utilities.Write(ptr, new ArFloatMatrix44[] { transformMatrix[1] }, 0, 1);
            //constantBuffer2[0].Unmap(0);

            ptr = constantBuffer2[1].Map(0);
            Utilities.Write(ptr, new int[] { data.VerteicesData[1].TextureIndex }, 0, 1);
            //constantBuffer2[1].Unmap(0);
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
           
            commandList.SetGraphicsRootDescriptorTable(0, constantBufferViewHeap.GPUDescriptorHandleForHeapStart);
            commandList.SetDescriptorHeaps(new DescriptorHeap[] { constantBufferViewHeap });
            CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += frameIndex * rtvDescriptorSize;
            commandList.SetRenderTargets(rtvHandle, null);
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);
            commandList.ClearRenderTargetView(rtvHandle, new Color4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W), 0, null);


            for (int i = 0; i < bundles.Length; i++)
            {   
                if(i == 1)
                    commandList.SetDescriptorHeaps(new DescriptorHeap[] { constantBufferViewHeap2 });
                commandList.ExecuteBundle(bundles[i]);
            }

            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);
            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);
            // GetDeviceRemovedReason
            swapChain.Present(1, 0);

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