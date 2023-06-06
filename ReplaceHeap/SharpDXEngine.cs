using SharpDX.Direct3D12;
using SharpDX.DXGI;
using SharpDX;
using System;
using System.Collections.Generic;
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
using GraphicLibrary;

namespace ReplaceHeap
{
    public class SharpDXEngine : IDisposable
    {
        public int FrameCount { get; private set; }
        public int ConstantBufferViewCount = 2;
        public int ShaderResourceViewCount = 2;

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


        public void Initialize()
        {
#if DEBUG
            DebugInterface.Get().EnableDebugLayer();
#endif
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
            //fence = device.CreateFence(0, FenceFlags.None);
            //fenceValue = 1;
            //fenceEvent = new AutoResetEvent(false);
        }

        

        public void LoadStaticData(SharpDXStaticData data)
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

            ptr = constantBuffer[0].Map(0);
            Utilities.Write(ptr, new ArFloatMatrix44[] { transformMatrix[0] }, 0, 1);
            constantBuffer[0].Unmap(0);
        }

        public void LoadData(SharpDXData data)
        {

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

        public void Close()
        {
            if(fence != null)
                fence.Dispose();
            for(int i = 0; i < renderTargets.Length; i++)
                renderTargets[i].Dispose();
            renderTargetViewHeap.Dispose();
            commandQueue.Dispose();            
            swapChain.Dispose();
            device.Dispose();
        }


        public void Dispose()
        {
            Close();
        }
    }
}
