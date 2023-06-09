﻿using GraphicLibrary2.Items;
using SharpDX;

using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
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

namespace GraphicLibrary2
{
    public partial class SharpDXEngine : IDisposable
    {
        public int BufferCount { get; protected set; }

        Device? device;
        Adapter4? adapter;
        CommandQueue? commandQueue;
        SwapChain3? swapChain;
        InfoQueue? infoQueue;

        PipelineState? PLStateBase, PLStateNormal, PLStatePoint, PLStateLine, PLStateCompute;

        DescriptorHeap? renderTargetViewHeap;
        int rtvDescriptorSize;
        DescriptorHeap? unorderedAccessViewHeap;
        int csuDescriptorSize;

        int frameIndex;

        Resource[]? renderTargets;
        Resource? tempResource;
        Resource? loadResource;
        Resource? debugBuffer;        
        Resource? constantBuffer;

        GraphicsCommandList? commandList, commandList2;
        GraphicsCommandList[]? bundles;
        CommandAllocator? commandAllocator, commandAllocator2; 
        RootSignature? computeRS;

        AutoResetEvent? fenceEvent;
        Fence? fence;
        int fenceValue;

        IntPtr ptr;

        //internal Dictionary<ShaderType, ShaderFileInfo> ShaderFiles { get; set; }
        internal Dictionary<int, Resource> TextureTable { get; set; }
        internal Dictionary<int, ArMaterial> MaterialTable { get; set; }
        internal Dictionary<string, ArCamera> CameraList { get; set; }
        internal Dictionary<int, ArLight> LightList { get; set; }
        //internal Dictionary<string, DirectX12Model> ModelTable { get; set; }        
        internal Dictionary<int, Resource> InstanceFrameVariables { get; set; }

        public SharpDXEngine()
        {
#if DEBUG
            DebugInterface.Get().EnableDebugLayer();            
#endif
            TextureTable = new Dictionary<int, Resource>();
            MaterialTable = new Dictionary<int, ArMaterial>();
            CameraList = new Dictionary<string, ArCamera>();
            LightList = new Dictionary<int, ArLight>();
            InstanceFrameVariables = new Dictionary<int, Resource>();
        }

        public void SetGrahpicCardAndRenderTarget(SharpDXInitializeSetting setting)
        {
            BufferCount = setting.BufferCount;
            using (Factory4 factory = new Factory4())
            {
                adapter = setting.AdapterIndex == -1 ? null : new Adapter4(factory.GetAdapter1(setting.AdapterIndex).NativePointer);
                device = new Device(adapter, (SharpDX.Direct3D.FeatureLevel)setting.FeatureLevel);
                CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
                commandQueue = device.CreateCommandQueue(queueDesc);
                SwapChainDescription swapChainDesc = new SwapChainDescription
                {
                    BufferCount = BufferCount,
                    ModeDescription = new ModeDescription(setting.ScreenWidth, setting.ScreenHeight,
                       new Rational(setting.RefreshRate, 1), Format.R8G8B8A8_UNorm),
                    Usage = Usage.RenderTargetOutput,
                    SwapEffect = SwapEffect.FlipDiscard,
                    OutputHandle = setting.OutputHandle,
                    SampleDescription = new SampleDescription(setting.SampleCount, setting.SampleQuality),
                    IsWindowed = setting.IsWindowed
                };
                SwapChain tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
                swapChain = tempSwapChain.QueryInterface<SwapChain3>();
                tempSwapChain.Dispose();
            }
            frameIndex = swapChain.CurrentBackBufferIndex;
            infoQueue = device.QueryInterface<InfoQueue>();

            DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = BufferCount,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.RenderTargetView
            };
            renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);

            rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);
            var rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            renderTargets = new Resource[BufferCount];
            for (int i = 0; i < BufferCount; i++)
            {
                renderTargets[i] = swapChain.GetBackBuffer<Resource>(i);
                device.CreateRenderTargetView(renderTargets[i], null, rtvHandle);
                rtvHandle += rtvDescriptorSize;
            }

            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;
            fenceEvent = new AutoResetEvent(false);

            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, null);
            commandList.Close();

            commandAllocator2 = device.CreateCommandAllocator(CommandListType.Direct);
            commandList2 = device.CreateCommandList(CommandListType.Direct, commandAllocator2, null);
            commandList2.Close();

        }

        public void LoadTextureFromBitmapFile(int index, string file)
        {
            Resource uploadHeap = LoadBitmapToUploadHeap(file);
            commandAllocator.Reset();
            commandList.Reset(commandAllocator, null);
            ResourceDescription textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, uploadHeap.Description.Width, uploadHeap.Description.Height);
            Resource texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
            commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(uploadHeap, 0), null);
            commandList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);
            commandList.Close();
            commandQueue.ExecuteCommandList(commandList);
            WaitForPreviousFrame(); // Can Fix
            uploadHeap.Dispose();
            TextureTable[index] = texture;
        }       

        //public void LoadTexture(int index, byte[] data, int width, int height)
        //{
        //    commandAllocator.Reset();
        //    commandList.Reset(commandAllocator, null);

        //    var textureDesc = ResourceDescription.Texture2D(Format.B8G8R8A8_UNorm, data.Width, data.Height);
        //    texture = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);

        //    var textureUploadHeap = device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, textureDesc, ResourceStates.CopySource);

        //    var handle = GCHandle.Alloc(data.Data, GCHandleType.Pinned);
        //    ptr = Marshal.UnsafeAddrOfPinnedArrayElement(data.Data, 0);
        //    textureUploadHeap.WriteToSubresource(0, null, ptr, 4 * data.Width, data.Data.Length);
        //    handle.Free();

        //    commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);
        //    commandList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);
        //    commandList.DiscardResource(textureUploadHeap, null);

        //    commandList.Close();
        //    commandQueue.ExecuteCommandList(commandList);
        //    WaitForPreviousFrame();
        //    TextureTable[index] = texture;
        //}

        public void PrepareLoadModel()
        {
            
        }

        public void LoadModel(string name)
        {

        }

        public void PrepareCreateInstance()
        {
            //CreatePipleLine();
            
        }

        public void PrepareRender()
        {

        }

        public void Render(string CameraName = null)
        {
            //commandAllocator.Reset();
            //commandList.Reset(commandAllocator, graphicPLState);
            //commandList.SetGraphicsRootSignature(graphicRootSignature);

            //commandList.SetViewport(viewport);
            //commandList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle(0, 0, (int)viewport.Width, (int)viewport.Height));

            //CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            //rtvHandle += frameIndex * rtvDescriptorSize;
            //commandList.SetRenderTargets(rtvHandle, null);

            //commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);
            //commandList.ClearRenderTargetView(rtvHandle, new Color4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W), 0, null);
            //commandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            //for (int i = 0; i < bundles.Length; i++)
            //{
            //    commandList.ExecuteBundle(bundles[i]);
            //}
            //commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);
            //commandList.CopyResource(debugGetBuffer, debug2Buffer);
            //commandList.Close();
            //commandQueue.ExecuteCommandList(commandList);

            //swapChain.Present(1, 0);
        }

        public void LoadGraphicSetting(SharpDXGraphicSetting setting)
        {

        }

        public void CreateComputeShader(string hlslFile)
        {
            var cmRootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.None,
                new RootParameter[]
                {
                    //new RootParameter(ShaderVisibility.All, new DescriptorRange(DescriptorRangeType.UnorderedAccessView, 1, 0))
                    new RootParameter(ShaderVisibility.All, new RootDescriptor(0, 0), RootParameterType.ConstantBufferView),
                    new RootParameter(ShaderVisibility.All, new RootDescriptor(0, 0), RootParameterType.UnorderedAccessView),
                    new RootParameter(ShaderVisibility.All, new RootDescriptor(1, 0), RootParameterType.UnorderedAccessView)
                }
            );
            computeRS = device.CreateRootSignature(cmRootSignatureDesc.Serialize());
            ComputePipelineStateDescription cpsDesc = new ComputePipelineStateDescription
            {
                ComputeShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(hlslFile, "CS", "cs_5_1", SharpDX.D3DCompiler.ShaderFlags.Debug,
                SharpDX.D3DCompiler.EffectFlags.None, null, null)),
                RootSignaturePointer = computeRS
            };

            PLStateCompute = device.CreateComputePipelineState(cpsDesc);

            //DescriptorHeapDescription uavHeapDesc = new DescriptorHeapDescription()
            //{
            //    DescriptorCount = 1,
            //    Flags = DescriptorHeapFlags.ShaderVisible,
            //    Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            //};
            //unorderedAccessViewHeap = device.CreateDescriptorHeap(uavHeapDesc);
            //csuDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

           
            //UnorderedAccessViewDescription uavdesc = new UnorderedAccessViewDescription
            //{
            //    Format = Format.Unknown,                
            //    Dimension = UnorderedAccessViewDimension.Buffer,                
            //};
            //uavdesc.Buffer.ElementCount = 1;
            //uavdesc.Buffer.CounterOffsetInBytes = 0;
            //uavdesc.Buffer.StructureByteStride = 16;

            //device.CreateUnorderedAccessView(tempResource, null, uavdesc, unorderedAccessViewHeap.CPUDescriptorHandleForHeapStart);
        }

        public void UploadComputeData<T>(T[] data) where T : struct
        {
            tempResource = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(1024, ResourceFlags.AllowUnorderedAccess), ResourceStates.Common);
            loadResource = device.CreateCommittedResource(new HeapProperties(HeapType.Readback), HeapFlags.None, ResourceDescription.Buffer(1024), ResourceStates.CopyDestination);
            debugBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(1024, ResourceFlags.AllowUnorderedAccess), ResourceStates.Common);
            constantBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(256), ResourceStates.Common);
            ptr = constantBuffer.Map(0);
            Utilities.Write(ptr, data, 0, data.Length);
            constantBuffer.Unmap(0);
        }

        public T[] Compute<T>(int count, int threadGroupCountX = 1, int threadGroupCountY = 1, int threadGroupCountZ = 1) where T: struct
        {
            commandAllocator2.Reset();
            commandList2.Reset(commandAllocator2, PLStateCompute);
            commandList2.SetComputeRootSignature(computeRS);
            //commandList2.SetDescriptorHeaps(new DescriptorHeap[] { unorderedAccessViewHeap });
            //commandList2.SetComputeRootDescriptorTable(0, unorderedAccessViewHeap.GPUDescriptorHandleForHeapStart);
            commandList2.ResourceBarrierTransition(tempResource, ResourceStates.Common, ResourceStates.UnorderedAccess);
            commandList2.SetComputeRootConstantBufferView(0, constantBuffer.GPUVirtualAddress);
            commandList2.SetComputeRootUnorderedAccessView(1, tempResource.GPUVirtualAddress);
            commandList2.SetComputeRootUnorderedAccessView(2, debugBuffer.GPUVirtualAddress);
            commandList2.Dispatch(threadGroupCountX, threadGroupCountY, threadGroupCountZ);
            commandList2.ResourceBarrierTransition(tempResource, ResourceStates.UnorderedAccess, ResourceStates.CopySource);
            //commandList2.CopyResource(loadResource, tempResource);
            commandList2.CopyResource(loadResource, debugBuffer);
            commandList2.Close();
            commandQueue.ExecuteCommandList(commandList2);
            WaitForPreviousFrame();

            ptr = loadResource.Map(0);
            T[] result = new T[count];
            Utilities.Read(ptr, result, 0, count);
            loadResource.Unmap(0);
            return result;
        }

       
    }
}
