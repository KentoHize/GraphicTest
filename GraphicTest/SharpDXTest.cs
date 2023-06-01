using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.Direct3D;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D12.Device;
using Resource = SharpDX.Direct3D12.Resource;
//using System.Numerics;
using SharpDX;

namespace GraphicTest
{
    //using SharpDX;
    //using SharpDX.Direct3D12;
    //using SharpDX.Windows;
    public partial class SharpDXTest : Form
    {

        //public void Initialize(RenderForm form)
        //{
        //LoadPipeline(form);
        //LoadAssets();
        //}

        public const string ShaderSLFile = "C:\\Programs\\GraphicTest\\GraphicTest\\shaders.hlsl";

        public Device D3D12Device { get; set; }
        public SwapChain SwapChain { get; set; }
        //public Texture2D target { get; set; }
        //public RenderTargetView targetveiw { get; set; }
        //public SharpDX.Direct3D12. Buffer { get; set; }
        //public SharpDX.Direct3D12.Buffer D3D12Buffer { get; set; }// = SharpDX.Direct3D12.Buffer;

        public SharpDXTest()
        {
            InitializeComponent();
            LoadPipeline(this);
            LoadAssets();
            Prepare();
            //SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            //d3d11Device = new SharpDX.Direct3D12.Device(DriverType.Hardware, DeviceCreationFlags.Debug);
        }

        private void SharpDXTest_Load(object sender, EventArgs e)
        {
            Render();
        }

        public void Render()
        {
            // Record all the commands we need to render the scene into the command list.
            PopulateCommandList();

            // Execute the command list.
            commandQueue.ExecuteCommandList(commandList);

            // Present the frame.
            swapChain.Present(1, 0);

            WaitForPreviousFrame();
        }

        public void Prepare()
        {
            
            
        }

        private void LoadPipeline(Form form)
        {
            int width = form.ClientSize.Width;
            int height = form.ClientSize.Height;

            viewport.Width = width;
            viewport.Height = height;
            viewport.MaxDepth = 1.0f;

            scissorRect.Right = width;
            scissorRect.Bottom = height;

#if DEBUG
            // Enable the D3D12 debug layer.
            {
                DebugInterface.Get().EnableDebugLayer();
            }
#endif
            device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            using (var factory = new Factory4())
            {
                // Describe and create the command queue.
                var queueDesc = new CommandQueueDescription(CommandListType.Direct);
                commandQueue = device.CreateCommandQueue(queueDesc);

                // Describe and create the swap chain.
                var swapChainDesc = new SwapChainDescription()
                {
                    BufferCount = FrameCount,
                    ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    Usage = Usage.RenderTargetOutput,
                    SwapEffect = SwapEffect.FlipDiscard,
                    OutputHandle = form.Handle,
                    //Flags = SwapChainFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                    IsWindowed = true
                };

                var tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
                swapChain = tempSwapChain.QueryInterface<SwapChain3>();
                tempSwapChain.Dispose();
                frameIndex = swapChain.CurrentBackBufferIndex;
            }

            // Create descriptor heaps.
            // Describe and create a render target view (RTV) descriptor heap.
            var rtvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = FrameCount,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.RenderTargetView
            };

            renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);

            rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);

            // Create frame resources.
            var rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            for (int n = 0; n < FrameCount; n++)
            {
                renderTargets[n] = swapChain.GetBackBuffer<SharpDX.Direct3D12.Resource>(n);
                device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
                rtvHandle += rtvDescriptorSize;
            }

            commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);
        }

        private void LoadAssets()
        {
            // Create an empty root signature.
            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout);
            rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());

            // Create the pipeline state, which includes compiling and loading shaders.

#if DEBUG
            var vertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(ShaderSLFile, "VSMain", "vs_5_0", SharpDX.D3DCompiler.ShaderFlags.Debug));
#else
            var vertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "VSMain", "vs_5_0"));
#endif

#if DEBUG
            var pixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(ShaderSLFile, "PSMain", "ps_5_0", SharpDX.D3DCompiler.ShaderFlags.Debug));
#else
            var pixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "PSMain", "ps_5_0"));
#endif

            // Define the vertex input layout.
            var inputElementDescs = new[]
            {
                    //new InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
                    new InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,12,0),
                    //new InputElement("COLOR",0,Format.R32G32B32A32_SInt,12,0),
            };

            // Describe and create the graphics pipeline state object (PSO).
            var psoDesc = new GraphicsPipelineStateDescription()
            {
                InputLayout = new InputLayoutDescription(inputElementDescs),
                RootSignature = rootSignature,
                VertexShader = vertexShader,
                PixelShader = pixelShader,
                RasterizerState = RasterizerStateDescription.Default(),
                BlendState = BlendStateDescription.Default(),
                DepthStencilFormat = SharpDX.DXGI.Format.D32_Float,
                DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
                SampleMask = int.MaxValue,
                PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                RenderTargetCount = 1,
                Flags = PipelineStateFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                StreamOutput = new StreamOutputDescription()
            };
            psoDesc.RenderTargetFormats[0] = SharpDX.DXGI.Format.R8G8B8A8_UNorm;

            pipelineState = device.CreateGraphicsPipelineState(psoDesc);

            // Create the command list.
            commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, pipelineState);

            // Create the vertex buffer.
            float aspectRatio = viewport.Width / viewport.Height;

            // Define the geometry for a triangle.
            ArrVertex[] triangleVertices = new ArrVertex[]
            {
                new ArrVertex() { Position = new System.Numerics.Vector3(0.0f, 0.25f, 0.0f), Color2 = new System.Numerics.Vector4(1.0f, 0.0f, 0.0f, 1.0f) },
                new ArrVertex() { Position = new System.Numerics.Vector3(0.25f, -0.25f, 0.0f), Color2 = new System.Numerics.Vector4(0.0f, 1.0f, 0.0f, 1.0f) },
                new ArrVertex() { Position = new System.Numerics.Vector3(-0.25f, -0.25f, 0.0f), Color2 = new System.Numerics.Vector4(0.0f, 0.0f, 1.0f, 1.0f) },
            };

            //ArrVertex[] triangleVertices = new ArrVertex[]
            //{
            //    new ArrVertex() { X = 0, Y = 100, Z = 0, Color2 = new System.Numerics.Vector4(1.0f, 0.0f, 0.0f, 1.0f) },
            //    new ArrVertex() { X = 100, Y = -100, Z = 0, Color2 = new System.Numerics.Vector4(0.0f, 1.0f, 0.0f, 1.0f) },
            //    new ArrVertex() { X = -100, Y = -100, Z = 0, Color2 = new System.Numerics.Vector4(0.0f, 0.0f, 1.0f, 1.0f) },
            //};

            //ArrVertex[] triangleVertices = new ArrVertex[]
            //{
            //    new ArrVertex() { Position = new System.Numerics.Vector3(0.0f, 0.25f, 0.0f), Color = System.Drawing.Color.Red },
            //    new ArrVertex() { Position = new System.Numerics.Vector3(0.25f, -0.25f, 0.0f), Color = System.Drawing.Color.Blue },
            //    new ArrVertex() { Position = new System.Numerics.Vector3(-0.25f, -0.25f, 0.0f), Color = System.Drawing.Color.Green },
            //};


            //var triangleVertices = new[]
            //{
            //    new Vertex() {Position=new Vector3(0.0f, 0.25f * aspectRatio, 0.0f ),Color=new Vector4(1.0f, 0.0f, 0.0f, 1.0f ) },
            //    new Vertex() {Position=new Vector3(0.25f, -0.25f * aspectRatio, 0.0f),Color=new Vector4(0.0f, 1.0f, 0.0f, 1.0f) },
            //    new Vertex() {Position=new Vector3(-0.25f, -0.25f * aspectRatio, 0.0f),Color=new Vector4(0.0f, 0.0f, 1.0f, 1.0f ) },
            //};

            int vertexBufferSize = Utilities.SizeOf(triangleVertices);

            // Note: using upload heaps to transfer static data like vert buffers is not 
            // recommended. Every time the GPU needs it, the upload heap will be marshalled 
            // over. Please read up on Default Heap usage. An upload heap is used here for 
            // code simplicity and because there are very few verts to actually transfer.
            vertexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize), ResourceStates.GenericRead);

            
            // Copy the triangle data to the vertex buffer.
            IntPtr pVertexDataBegin = vertexBuffer.Map(0);
            Utilities.Write(pVertexDataBegin, triangleVertices, 0, triangleVertices.Length);
            vertexBuffer.Unmap(0);

            // Initialize the vertex buffer view.
            vertexBufferView = new VertexBufferView();
            vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
            vertexBufferView.StrideInBytes = Utilities.SizeOf<ArrVertex>();
            vertexBufferView.SizeInBytes = vertexBufferSize;

            // Command lists are created in the recording state, but there is nothing
            // to record yet. The main loop expects it to be closed, so close it now.
            commandList.Close();

            // Create synchronization objects.
            fence = device.CreateFence(0, FenceFlags.None);
            fenceValue = 1;

            // Create an event handle to use for frame synchronization.
            fenceEvent = new AutoResetEvent(false);
        }

        private void PopulateCommandList()
        {
            // Command list allocators can only be reset when the associated 
            // command lists have finished execution on the GPU; apps should use 
            // fences to determine GPU execution progress.
            commandAllocator.Reset();

            // However, when ExecuteCommandList() is called on a particular command 
            // list, that command list can then be reset at any time and must be before 
            // re-recording.
            commandList.Reset(commandAllocator, pipelineState);


            // Set necessary state.
            commandList.SetGraphicsRootSignature(rootSignature);
            commandList.SetViewport(viewport);
            commandList.SetScissorRectangles(scissorRect);

            // Indicate that the back buffer will be used as a render target.
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);

            var rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            rtvHandle += frameIndex * rtvDescriptorSize;
            commandList.SetRenderTargets(rtvHandle, null);

            // Record commands.
            commandList.ClearRenderTargetView(rtvHandle, new Color4(0, 0.2F, 0.4f, 1), 0, null);

            commandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            commandList.SetVertexBuffer(0, vertexBufferView);
            commandList.DrawInstanced(3, 1, 0, 0);

            // Indicate that the back buffer will now be used to present.
            commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);

            commandList.Close();
        }


        /// <summary> 
        /// Wait the previous command list to finish executing. 
        /// </summary> 
        private void WaitForPreviousFrame()
        {
            // WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
            // This is code implemented as such for simplicity. 

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

        struct Vertex
        {
            public Vector3 Position;
            public Vector4 Color;
        };

        const int FrameCount = 2;

        private ViewportF viewport;
        private SharpDX.Rectangle scissorRect;
        // Pipeline objects.
        private SwapChain3 swapChain;
        private Device device;
        private readonly Resource[] renderTargets = new Resource[FrameCount];
        private CommandAllocator commandAllocator;
        private CommandQueue commandQueue;
        private RootSignature rootSignature;
        private DescriptorHeap renderTargetViewHeap;
        private PipelineState pipelineState;
        private GraphicsCommandList commandList;
        private int rtvDescriptorSize;

        // App resources.
        Resource vertexBuffer;
        VertexBufferView vertexBufferView;

        // Synchronization objects.
        private int frameIndex;
        private AutoResetEvent fenceEvent;

        private Fence fence;
        private int fenceValue;

        private void SharpDXTest_MouseClick(object sender, MouseEventArgs e)
        {
            Render();
        }
    }
}


//SwapChainDescription scd = new SwapChainDescription()
//{
//    BufferCount = 1,                                 //how many buffers are used for writing. it's recommended to have at least 2 buffers but this is an example
//    Flags = SwapChainFlags.None,
//    IsWindowed = true,                               //it's windowed
//    ModeDescription = new ModeDescription(
//                this.ClientSize.Width,                       //windows veiwable width
//                this.ClientSize.Height,                      //windows veiwable height
//                new Rational(60, 1),                          //refresh rate
//                Format.R8G8B8A8_UNorm),                      //pixel format, you should resreach this for your specific implementation

//    OutputHandle = this.Handle,                      //the magic 

//    SampleDescription = new SampleDescription(1, 0), //the first number is how many samples to take, anything above one is multisampling.
//    SwapEffect = SwapEffect.Discard,
//    Usage = Usage.RenderTargetOutput
//};



//Device.CreateWithSwapChain(
//    DriverType.Hardware,//hardware if you have a graphics card otherwise you can use software
//    DeviceCreationFlags.Debug,           //helps debuging don't use this for release verion
//    scd,                                 //the swapchain description made above
//            out d, out sc                        //our directx objects
//            );

//using System;

//using System.ComponentModel;//needed to overide OnClosing
////I removed useless usings
//using System.Windows.Forms;

//using SharpDX.Direct3D11;
//using SharpDX.DXGI;
//using SharpDX;


//namespace WindowsFormsApplication2
//{
//    using Device = SharpDX.Direct3D11.Device;
//    using Buffer = SharpDX.Direct3D11.Buffer;


//    public partial class Form1 : Form
//    {
//        Device d;
//        SwapChain sc;

//        Texture2D target;
//        RenderTargetView targetveiw;

//        public Form1()
//        {
//            InitializeComponent();

//            SwapChainDescription scd = new SwapChainDescription()
//            {
//                BufferCount = 1,                                 //how many buffers are used for writing. it's recommended to have at least 2 buffers but this is an example
//                Flags = SwapChainFlags.None,
//                IsWindowed = true,                               //it's windowed
//                ModeDescription = new ModeDescription(
//                    this.ClientSize.Width,                       //windows veiwable width
//                    this.ClientSize.Height,                      //windows veiwable height
//                    new Rational(60, 1),                          //refresh rate
//                    Format.R8G8B8A8_UNorm),                      //pixel format, you should resreach this for your specific implementation

//                OutputHandle = this.Handle,                      //the magic 

//                SampleDescription = new SampleDescription(1, 0), //the first number is how many samples to take, anything above one is multisampling.
//                SwapEffect = SwapEffect.Discard,
//                Usage = Usage.RenderTargetOutput
//            };

//            Device.CreateWithSwapChain(
//                SharpDX.Direct3D.DriverType.Hardware,//hardware if you have a graphics card otherwise you can use software
//                DeviceCreationFlags.Debug,           //helps debuging don't use this for release verion
//                scd,                                 //the swapchain description made above
//                out d, out sc                        //our directx objects
//                );

//            target = Texture2D.FromSwapChain<Texture2D>(sc, 0);
//            targetveiw = new RenderTargetView(d, target);

//            d.ImmediateContext.OutputMerger.SetRenderTargets(targetveiw);

//        }

//        protected override void OnClosing(CancelEventArgs e)
//        {
//            //dipose of all objects
//            d.Dispose();
//            sc.Dispose();
//            target.Dispose();
//            targetveiw.Dispose();

//            base.OnClosing(e);
//        }

//        protected override void OnPaint(PaintEventArgs e)
//        {
//            //I am rendering here for this example
//            //normally I use a seperate thread to call Draw() and Present() in a loop
//            d.ImmediateContext.ClearRenderTargetView(targetveiw, Color.CornflowerBlue);//Color to make it look like default XNA project output.
//            sc.Present(0, PresentFlags.None);

//            base.OnPaint(e);
//        }

//    }