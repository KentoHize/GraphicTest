using GraphicLibrary;
using GraphicLibrary.Items;

namespace WriteText
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        const string textureFile = @"C:\Programs\GraphicTest\Texture\Texture\AnnetteSquare.bmp";
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
                Viewport = new SharpDX.ViewportF(0, 0, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height),
                FrameCount = 2,
                SwapEffect = SharpDX.DXGI.SwapEffect.FlipDiscard,
                Handle = pictureBox1.Handle
            });
            sde.LoadStaticData(new SharpDXStaticData
            {
                Textures = new SharpDXTextureData[]
                {
                    new SharpDXTextureData
                    {
                        Data = Ar3DMachine.LoadBitmapFromFile(textureFile, out int width, out int height),
                        Width = width,
                        Height = height
                    }
                }
            });

            var aCube = Ar3DGeometry.GetCube(512);
            
            SharpDXData data = new SharpDXData
            {
                BackgroundColor = Color.Black.ToArFloatVector4(),
                TransformMartrix = Ar3DMachine.ProduceTransformMatrix(
                                        new ArIntVector3(0, 0, 0),
                                        new ArFloatVector3(-0.2f, -0.2f, 0.7f),
                                        new ArFloatVector3(1, 1, 1)),
                VerticesData = new SharpDXBundleData[]
                {
                    new SharpDXBundleData
                    {
                        PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                        TextureVertices = aCube.vertices,
                        Indices = aCube.indices,
                        TextureIndex = 1
                    }
                    //new SharpDXBundleData
                    //{
                    //    PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                    //    TextureVertices = new ArTextureVertex[]
                    //    {
                    //        new ArTextureVertex(0, 0, 0, 0, 1),
                    //        new ArTextureVertex(512, 0, 0, 1, 1),
                    //        new ArTextureVertex(0, 512, 0, 0, 0),
                    //        new ArTextureVertex(512, 512, 0, 1, 0)
                    //    },
                    //    Indices = new int[]
                    //    {
                    //        1, 0, 2, 1, 2, 3
                    //    },
                    //    TextureIndex = 0
                    //},
                    //new SharpDXBundleData
                    //{
                    //    PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                    //    TextureVertices = new ArTextureVertex[]
                    //    {
                    //        new ArTextureVertex(0, 0, 0, 1, 0),
                    //        new ArTextureVertex(-512, 0, 0, 0, 0),
                    //        new ArTextureVertex(0, -512, 0, 1, 1),
                    //        new ArTextureVertex(-512, -512, 0, 0, 1)
                    //    },
                    //    Indices = new int[]
                    //    {
                    //        1, 0, 2, 1, 2, 3
                    //    },
                    //    TextureIndex = 1
                    //}
                }
            };
            sde.LoadData(data);
            sde.Render();
            sde.Close();
        }
    }
}
