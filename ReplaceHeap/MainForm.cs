using GraphicLibrary.Items;
using GraphicLibrary;
using System.Diagnostics;

namespace ReplaceHeap
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        const string textureFile = @"C:\Programs\GraphicTest\Texture\Texture\AnnetteSquare.bmp";
        const string textureFile2 = @"C:\Programs\GraphicTest\Texture\Texture\ClacierSquare.bmp";
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
            sde.Initialize();
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
                    },
                    new SharpDXTextureData
                    {
                        Data = Ar3DMachine.LoadBitmapFromFile(textureFile2, out int width2, out int height2),
                        Width = width2,
                        Height = height2
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
                        TextureVertices = new ArTextureVertex[]
                        {
                            new ArTextureVertex(0, 0, 0, 0, 0),
                            new ArTextureVertex(512, 0, 0, 1, 0),
                            new ArTextureVertex(0, 512, 0, 0, 1),
                            new ArTextureVertex(512, 512, 0, 1, 1)
                        },
                        Indices = new int[]
                        {
                            0, 1, 2, 1, 2, 3
                        },
                        TextureIndex = 0
                    }
                }
            };
            //    VerteicesData = new SharpDXBundleData[]
            //    {
            //        new SharpDXBundleData
            //        {
            //            PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
            //            ColorVertices = new ArColorVertex[]
            //            {
            //                new ArColorVertex(0, 0, 0, Color.Blue),
            //                new ArColorVertex(512, 0, 0, Color.White),
            //                new ArColorVertex(0, 512, 0, Color.White),
            //                new ArColorVertex(512, 512, 0, Color.Red)
            //            },
            //            Indices = new int[]
            //            {
            //                0, 1, 2, 1, 2, 3
            //            },
            //            TransformMartrix = Ar3DMachine.ProduceTransformMatrix(
            //                new ArIntVector3(0, 0, 0),
            //                new ArFloatVector3(0, 0, 0),
            //                //new ArFloatVector3((float)Math.PI /2, 0, 0),
            //                new ArFloatVector3(1, 1, 1))
            //        },
            //        new SharpDXBundleData
            //        {
            //            PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList,
            //            ColorVertices = new ArColorVertex[]
            //            {
            //                new ArColorVertex(-200, 0, 0, Color.Gold),
            //                new ArColorVertex(-200, 400, 0, Color.Gold),
            //                new ArColorVertex(-300, 400, 0, Color.Gold),
            //                new ArColorVertex(-300, 0, 0, Color.Gold),
            //            },
            //            Indices = new int[]
            //            {
            //                0, 1, 1, 2, 2, 3
            //            }
            //        },
            //        new SharpDXBundleData
            //        {
            //            PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
            //            MixVertices = new ArMixVertex[]
            //            {
            //                new ArMixVertex(700, -100, 0, 1, 0),
            //                new ArMixVertex(0, -700, 0, 0, 1),
            //                new ArMixVertex(700, -700, 0, 1, 1),
            //                new ArMixVertex(0, -100, 0, 0, 0),
            //            },
            //            Indices = new int[]
            //            {
            //                0, 1, 2, 0, 1, 3
            //            },
            //            TextureIndex = 0
            //        },
            //        new SharpDXBundleData
            //        {
            //            PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
            //            MixVertices = new ArMixVertex[]
            //            {
            //                new ArMixVertex(-200, -100, 0, 1, 0),
            //                new ArMixVertex(-700, -700, 0, 0, 1),
            //                new ArMixVertex(-200, -700, 0, 1, 1),
            //                new ArMixVertex(-700, -100, 0, 0, 0),
            //            },
            //            Indices = new int[]
            //            {
            //                0, 1, 2, 0, 1, 3
            //            },
            //            TextureIndex = 1
            //        }
            //    }

            //};

            //Stopwatch sw = Stopwatch.StartNew();
            sde.LoadData(data);
            //sw.Stop();
            //Debug.WriteLine($"Load Data:{sw.ElapsedMilliseconds}");
            //sw.Restart();
            //sde.Render();
            //Debug.WriteLine($"Render:{sw.ElapsedMilliseconds}");

            sde.Render();
            sde.Close();
        }
    }
}