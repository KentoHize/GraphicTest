using GraphicLibrary;
using GraphicLibrary.Items;


namespace CreateSphere
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        SharpDXData data;
        float rx = 0, ry = 0, rz = 0;
        const string textureFile = @"C:\Programs\GraphicTest\CreateSphere\Textures\AnnetteSquare.bmp";
        public MainForm()
        {
            InitializeComponent();
            sde = new SharpDXEngine();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            sde.LoadSetting(new SharpDXSetting
            {
                CullTwoFace = false,
                DrawClockwise = false,
                Viewport = new SharpDX.ViewportF(0, 0, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height),
                FrameCount = 2,
                SwapEffect = SharpDX.DXGI.SwapEffect.FlipDiscard,
                Handle = pictureBox1.Handle
            });
            sde.LoadTextureFile(textureFile, "Annette");


            var aCube = Ar3DGeometry.GetTextureCube(512);

            data = new SharpDXData
            {
                BackgroundColor = Color.Black.ToArFloatVector4(),
                TransformMartrix = Ar3DMachine.ProduceTransformMatrix(
                                        new ArIntVector3(0, 0, 0),
                                        new ArFloatVector3(1.7f, -0.2f, 0.2f),
                                        new ArFloatVector3(1, 1, 1)),
                VerticesData = new SharpDXBundleData[]
                {
                    new SharpDXBundleData
                    {
                        PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                        TextureVertices = aCube.vertices,
                        Indices = aCube.indices,
                    },
                    //new SharpDXBundleData
                    //{
                        
                    //}
                }

            };
            sde.LoadModel(data);
            sde.Render();
        }
    }
}
