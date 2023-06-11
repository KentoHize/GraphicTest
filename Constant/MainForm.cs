using GraphicLibrary.Items;
using GraphicLibrary;

namespace Constant
{
    public partial class MainForm : Form
    {
        const string textureFile = @"C:\Programs\GraphicTest\Texture\Texture\AnnetteSquare.bmp";
        const string textureFile2 = @"C:\Programs\GraphicTest\Texture\Texture\ClacierSquare.bmp";
        SharpDXEngine sde;

        public MainForm()
        {
            InitializeComponent();
            sde = new SharpDXEngine();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SharpDXSetting setting = new SharpDXSetting
            {
                CullTwoFace = false,
                Handle = pibMain.Handle,
                FrameCount = 2,
                Viewport = new SharpDX.ViewportF(0, 0, pibMain.ClientSize.Width, pibMain.ClientSize.Height)
            };
            sde.LoadSetting(setting);
            sde.Close();

            sde.LoadSetting(setting);
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

            SharpDXData data = new SharpDXData
            {
                BackgroundColor = Color.Black.ToArFloatVector4(),
                TransformMartrix = Ar3DMachine.ProduceTransformMatrix(
                                        new ArIntVector3(0, 0, 0),
                                        new ArFloatVector3(0, 0, 0),
                                        new ArFloatVector3(1, 1, 1)),
                VerticesData = new SharpDXBundleData[]
                {
                    new SharpDXBundleData
                    {
                        PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                        TextureVertices = new ArTextureVertex[]
                        {
                            new ArTextureVertex(0, 0, 0, 0, 1),
                            new ArTextureVertex(512, 0, 0, 1, 1),
                            new ArTextureVertex(0, 512, 0, 0, 0),
                            new ArTextureVertex(512, 512, 0, 1, 0)
                        },
                        Indices = new int[]
                        {
                            1, 0, 2, 1, 2, 3
                        },
                        TextureIndex = 0
                    },
                    new SharpDXBundleData
                    {
                        PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                        TextureVertices = new ArTextureVertex[]
                        {
                            new ArTextureVertex(0, 0, 0, 1, 0),
                            new ArTextureVertex(-512, 0, 0, 0, 0),
                            new ArTextureVertex(0, -512, 0, 1, 1),
                            new ArTextureVertex(-512, -512, 0, 0, 1)
                        },
                        Indices = new int[]
                        {
                            1, 0, 2, 1, 2, 3
                        },
                        TextureIndex = 1
                    }
                }
            };
            sde.LoadData(data);
            
        }

        private void pibMain_Paint(object sender, PaintEventArgs e)
        {   
            sde.Render();
        }
    }
}