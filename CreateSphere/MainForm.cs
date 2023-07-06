using Accessibility;
using GraphicLibrary;
using GraphicLibrary.Items;


namespace CreateSphere
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        SharpDXData data;
        ArFloatVector3 lightDirection;
        float rx = 0, ry = 0, rz = 0;
        const string textureFile = @"C:\Programs\GraphicTest\CreateSphere\Textures\AnnetteSquare.bmp";
        public MainForm()
        {
            InitializeComponent();
            sde = new SharpDXEngine();
        }

        ArFloatMatrix33 GetTransformMatrixFromNormalToZ(ArFloatVector3 Normal)
        {
            ArFloatVector3 XNormal = new ArFloatVector3(Normal.X, 0, Normal.Z);
            ArFloatVector3 YNormal = new ArFloatVector3(0, Normal.Y, Normal.Z);
            double cosX = XNormal.DotProduct(Normal) / (XNormal.GetLength() * Normal.GetLength());
            double cosY = YNormal.DotProduct(Normal) / (YNormal.GetLength() * Normal.GetLength());
            if (cosX > 1)
                cosX = 1;
            else if (cosX < -1)
                cosX = -1;
            if (cosY > 1)
                cosY = 1;
            else if (cosY < -1)
                cosY = -1;
            double AngleY = Math.Acos(cosX);
            double AngleX = Math.Acos(cosY);
            if (Normal[0] < 0)
                AngleX *= -1;
            if (Normal[1] < 0)
                AngleY *= -1;
            //if (dotX < 0)
            //    AngleY *= -1;
            //if (dotY < 0)
            //    AngleX *= -1;
            return (ArFloatMatrix33)Ar3DMachine.ProduceTransformMatrix(ArIntVector3.Zero, new ArFloatVector3(0, (float)AngleX, (float)AngleY), ArFloatVector3.One, 1);
            //return Ar3DMachine.GetRotateMatrix(new ArFloatVector3(0, (float)AngleX, (float)AngleY));
            //return Ar3DMachine.GetRotateMatrix(new ArFloatVector3((float)AngleX, 0, (float)AngleY));
            //return Ar3DMachine.ProduceTransformMatrix(ArIntVector3.Zero, new ArFloatVector3((float)AngleX, (float)AngleY, 0), ArFloatVector3.One);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Create Box
            sde.LoadSetting(new SharpDXSetting
            {
                CullTwoFace = true,
                DrawClockwise = false,
                Viewport = new SharpDX.ViewportF(0, 0, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height),
                FrameCount = 2,
                SwapEffect = SharpDX.DXGI.SwapEffect.FlipDiscard,
                Handle = pictureBox1.Handle
            });

            //Create Box
            sde.LoadTextureFile(textureFile, "Annette");
            //var aCube = Ar3DGeometry.GetTextureCube(512);

            //data = new SharpDXData
            //{
            //    BackgroundColor = Color.Black.ToArFloatVector4(),
            //    TransformMartrix = Ar3DMachine.ProduceTransformMatrix(
            //                            new ArIntVector3(0, 0, 0),
            //                            new ArFloatVector3(1.7f, -0.2f, 0.2f),
            //                            new ArFloatVector3(1, 1, 1)),
            //    VerticesData = new SharpDXBundleData[]
            //    {
            //        new SharpDXBundleData
            //        {
            //            PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
            //            TextureVertices = aCube.vertices,
            //            Indices = aCube.indices,
            //        },
            //        //new SharpDXBundleData
            //        //{

            //        //}
            //    }

            //};
            //sde.LoadModel(data);
            //sde.Render();
            lightDirection = new ArFloatVector3(0, -1, 1);
            LoadModel(lightDirection);

        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'q':
                    lightDirection[0] -= 1f;
                    break;
                case 'e':
                    lightDirection[0] += 1f;
                    break;
                case 'w':
                    lightDirection[1] += 1f;
                    break;
                case 's':
                    lightDirection[1] -= 1f;
                    break;
                case 'a':
                    lightDirection[2] -= 1f;
                    break;
                case 'd':
                    lightDirection[2] += 1f;
                    break;

                case 'u':
                    rx -= 0.1f;
                    break;
                case 'o':
                    rx += 0.1f;
                    break;
                case 'i':
                    ry += 0.1f;
                    break;
                case 'k':
                    ry -= 0.1f;
                    break;
                case 'j':
                    rz -= 0.1f;
                    break;
                case 'l':
                    rz += 0.1f;
                    break;
                case ' ':
                    rx = 0;
                    ry = 0;
                    rz = 0;
                    lightDirection[0] = 0;
                    lightDirection[1] = 0;
                    lightDirection[2] = 1;
                    break;
                default:
                    break;
            }
            LoadModel(lightDirection);
        }

        void LoadModel(ArFloatVector3 lightDirectionVector)
        {
            List<ArIntVector3> vertices = new List<ArIntVector3>();
            vertices.AddRange(Ar3DGeometry.GetTransformedEquilateralTriangle(200));
            vertices.AddRange(Ar3DGeometry.GetTransformedEquilateralTriangle(200, new ArIntVector3(400, 0, 0), null));
            vertices.AddRange(Ar3DGeometry.GetTransformedEquilateralTriangle(200, new ArIntVector3(400, 400, 0), null));
            vertices.AddRange(Ar3DGeometry.GetTransformedEquilateralTriangle(200, new ArIntVector3(400, -400, 0), null));
            vertices.AddRange(Ar3DGeometry.GetTransformedEquilateralTriangle(200, new ArIntVector3(-400, -400, 0), null));
            vertices.AddRange(Ar3DGeometry.GetTransformedEquilateralTriangle(200, new ArIntVector3(0, -400, 0), null));
            //vertices.AddRange(Ar3DGeometry.GetTransformedEquilateralTriangle(200, new ArIntVector3(400, 0, 0), new ArFloatVector3(0.5f, 0.5f, 0)));
            List<int> indices = new List<int>();
            for(int i = 0; i < vertices.Count; i++)
                indices.Add(i);

            ArFloatVector3 planeN = ArFloatVector3.UnitZ;
            ArFloatVector3 directionV = lightDirectionVector;

            List<ArTextureVertex> vertices3 = new List<ArTextureVertex>();
            ArFloatMatrix33 tm = Ar3DMachine.GetRotateMatrix(new ArFloatVector3(rx, ry, rz));

            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = (ArIntVector3)(tm * vertices[i]);

            for (int i = 0; i < vertices.Count; i++)
                vertices3.Add(new ArTextureVertex((int)vertices[i][0], (int)vertices[i][1], (int)vertices[i][2], 1, 1));

            ArFloatMatrix33 tm2 = GetTransformMatrixFromNormalToZ(directionV);
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = (ArIntVector3)(tm2 * vertices[i]);

            List<ArTextureVertex> vertices2 = new List<ArTextureVertex>();
            for (int i = 0; i < vertices.Count; i++)
                vertices2.Add(new ArTextureVertex(vertices[i][0], vertices[i][1], 0));
           
            //ArFloatMatrix44 f44 = new ArFloatMatrix44();

            data = new SharpDXData
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
                            TextureVertices = vertices3.ToArray(),
                            Indices = indices.ToArray(),
                        },
                        new SharpDXBundleData
                        {
                            PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                            TextureVertices = vertices2.ToArray(),
                            Indices = indices.ToArray(),
                        },
                        
                    }
            };
            lblDirection.Text = $"Rotation:({rx},{ry},{rz})\nLight Direction:{lightDirection}";
            sde.LoadModel(data);
            sde.Render();
        }
    }
}
