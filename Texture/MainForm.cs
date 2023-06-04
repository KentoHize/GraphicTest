using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using GraphicLibrary;
using GraphicLibrary.Items;

namespace Texture
{ 

    public partial class MainForm : Form
    {
        const string textureFile = @"C:\Programs\GraphicTest\Texture\Texture\158452020235.jpg";

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
                CullTwoFace = true,
                Handle = pibMain.Handle,
                FrameCount = 2,
                Viewport = new SharpDX.ViewportF(0, 0, pibMain.ClientSize.Width, pibMain.ClientSize.Height)
            };
            sde.Initialize(setting);

            sde.LoadStaticData(new SharpDXStaticData
            {
                Textures = new SharpDXTextureData[]
                {
                    new SharpDXTextureData
                    {
                        Data = new FileStream(textureFile, FileMode.Open)
                    }
                }
            });

            SharpDXData data = new SharpDXData
            {
                BackgroundColor = Color.Black.ToArFloatVector4(),
                VerteicesData = new SharpDXBundleData[]
                {
                    new SharpDXBundleData
                    {
                        PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                        ColorVertices = new ArColorVertex[]
                        {
                            new ArColorVertex(0, 0, 0, Color.Blue),
                            new ArColorVertex(512, 0, 0, Color.White),
                            new ArColorVertex(0, 512, 0, Color.White),
                            new ArColorVertex(512, 512, 0, Color.Red)
                        },
                        Indices = new int[]
                        {
                            0, 1, 2, 1, 2, 3
                        },
                        TransformMartrix = Ar3DMachine.ProduceTransformMatrix(
                            new ArIntVector3(-200, 0, 0),
                            new ArFloatVector3(0, 0, 0),
                            //new ArFloatVector3((float)Math.PI /2, 0, 0),
                            new ArFloatVector3(1, 1, 1))
                    },
                    new SharpDXBundleData
                    {
                        PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList,
                        ColorVertices = new ArColorVertex[]
                        {
                            new ArColorVertex(-200, -200, 0, Color.Gold),
                            new ArColorVertex(-200, 200, 0, Color.Gold),
                            new ArColorVertex(-300, 200, 0, Color.Gold),
                            new ArColorVertex(-300, -200, 0, Color.Gold),
                        },
                        Indices = new int[]
                        {
                            0, 1, 1, 2, 2, 3
                        }
                    },
                    new SharpDXBundleData
                    {
                        PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                        TextureVertices = new ArTextureVertex[]
                        {
                            new ArTextureVertex(700, -100, 0, 1, 0),
                            new ArTextureVertex(0, -700, 0, 0, 0),
                            new ArTextureVertex(700, -700, 0, 1, 1),
                            new ArTextureVertex(0, -100, 0, 1, 0),
                        },
                        Indices = new int[]
                        {
                            0, 1, 2, 0, 1, 3
                        }
                    }
                }

            };
            Stopwatch sw = Stopwatch.StartNew();
            sde.Load(data);
            sw.Stop();
            Debug.WriteLine($"Load Data:{sw.ElapsedMilliseconds}");
            sw.Restart();
            sde.Render();
            Debug.WriteLine($"Render:{sw.ElapsedMilliseconds}");
        }
    }
}