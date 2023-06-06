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
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GraphicLibrary
{
    public class SharpDXEngine : IDisposable
    {
        public int FrameCount { get; private set; }
        public const int ConstantBufferViewCount = 2;
        public const int ShaderResourceViewCount = 2;

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

        internal Dictionary<ShaderType, ShaderFileInfo> ShaderFiles { get; set; }

        public SharpDXEngine()
        {
            FrameCount = 2;
            const string GLShaderFile = @"C:\Programs\GraphicTest\Texture\shaders.hlsl";

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
            for (int i = 0; i < ConstantBufferViewCount; i++)
            {
                constantBuffer[i] = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.GenericRead);
                var cbvDesc = new ConstantBufferViewDescription()
                {
                    BufferLocation = constantBuffer[i].GPUVirtualAddress,
                    SizeInBytes = (Utilities.SizeOf<ArFloatMatrix44>() + 255) & ~255
                };

                device.CreateConstantBufferView(cbvDesc, cruHandle);
                cruHandle += cruDescriptorSize;
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
                        },
                        new DescriptorRange
                        {
                            RangeType = DescriptorRangeType.ShaderResourceView,
                            BaseShaderRegister = 0,
                            RegisterSpace = 0,
                            OffsetInDescriptorsFromTableStart = ConstantBufferViewCount,
                            DescriptorCount = ShaderResourceViewCount
                        }),
                        
            },
            new StaticSamplerDescription[]
            {
                new StaticSamplerDescription(ShaderVisibility.Pixel, 0, 0)
                {
                     Filter = Filter.MinimumMinMagMipPoint,
                     AddressUVW = TextureAddressMode.Border,
                }
            });
            graphicRootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());

            InputElement[] inputElementDescs = new InputElement[]
              {
                    new InputElement("POSITION", 0, Format.R32G32B32_SInt,0,0),
                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float,12,0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float,28,0),                    
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

            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;
            fenceEvent = new AutoResetEvent(false);
        }

        public long GetRequiredIntermediateSize(Resource destinationResource, int firstSubresource, int subresourcesCount)
        {
            ResourceDescription desc = destinationResource.Description;
            device.GetCopyableFootprints(ref desc, firstSubresource, subresourcesCount, 0, null, null, null, out long requiredSize);
            return requiredSize;
        }

        public void LoadStaticData(SharpDXStaticData data)
        {
            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, graphicPLState);
            shaderResource = new Resource[ShaderResourceViewCount];
            for(int i = 0; i < data.Textures.Length; i++)
            {
                var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Textures[i].Width, data.Textures[i].Height);
                texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
                long uploadBufferSize = GetRequiredIntermediateSize(texture, 0, 1);
                var textureUploadHeap = device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Textures[i].Width, data.Textures[i].Height), ResourceStates.GenericRead);
                var handle = GCHandle.Alloc(data.Textures[i].Data, GCHandleType.Pinned);
                ptr = Marshal.UnsafeAddrOfPinnedArrayElement(data.Textures[i].Data, 0);
                textureUploadHeap.WriteToSubresource(0, null, ptr, 4 * data.Textures[i].Width, data.Textures[i].Data.Length);
                handle.Free();
                commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);
                commandList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);                
                var srvDesc = new ShaderResourceViewDescription
                {   
                    Shader4ComponentMapping = D3DXUtilities.DefaultComponentMapping(),
                    Format = textureDesc.Format,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = { MipLevels = 1 },                    
                };
                device.CreateShaderResourceView(texture, srvDesc, cruHandle);
                cruHandle += cruDescriptorSize;
            }
           

            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);
        }

        public void Load(SharpDXData data)
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
                //bundles[i].SetGraphicsRootSignature(graphicRootSignature);
                bundles[i].PrimitiveTopology = data.VerteicesData[i].PrimitiveTopology;
                bundles[i].SetVertexBuffer(0, verticesBufferView[i]);
                bundles[i].SetIndexBuffer(indicesBufferView[i]);
                bundles[i].DrawIndexedInstanced(data.VerteicesData[i].Indices.Length, 1, 0, 0, 0);
                bundles[i].Close();
            }

            ptr = constantBuffer[0].Map(0);
            Utilities.Write(ptr, new ArFloatMatrix44[] { transformMatrix[0] }, 0, 1);
            constantBuffer[0].Unmap(0);
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

            CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += frameIndex * rtvDescriptorSize;
            commandList.SetRenderTargets(rtvHandle, null);
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);
            commandList.ClearRenderTargetView(rtvHandle, new Color4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W), 0, null);
                        
            for (int i = 0; i < bundles.Length; i++)
            {
                //ptr = constantBuffer[0].Map(0);
                //Utilities.Write(ptr, new ArFloatMatrix44[] { transformMatrix[0] }, 0, 1);
                //constantBuffer[0].Unmap(0);
                //commandAllocator.Reset();
                commandList.ExecuteBundle(bundles[i]);
                //commandList.SetGraphicsRootConstantBufferView()
            
            }
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);
            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);

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

        public void Dispose()
        {
            device.Dispose();
        }
    }

    public class D3DXUtilities
    {

        public const int ComponentMappingMask = 0x7;
        public const int ComponentMappingShift = 3;
        public const int ComponentMappingAlwaysSetBitAvoidingZeromemMistakes = (1 << (ComponentMappingShift * 4));
        public static int ComponentMapping(int src0, int src1, int src2, int src3)
        {
            return ((((src0) & ComponentMappingMask) |
            (((src1) & ComponentMappingMask) << ComponentMappingShift) |
                                                                (((src2) & ComponentMappingMask) << (ComponentMappingShift * 2)) |
                                                                (((src3) & ComponentMappingMask) << (ComponentMappingShift * 3)) |
                                                                ComponentMappingAlwaysSetBitAvoidingZeromemMistakes));
        }

        public static int DefaultComponentMapping()
        {
            return ComponentMapping(0, 1, 2, 3);
        }

        public static int ComponentMapping(int ComponentToExtract, int Mapping)
        {
            return ((Mapping >> (ComponentMappingShift * ComponentToExtract) & ComponentMappingMask));
        }
    }
}
