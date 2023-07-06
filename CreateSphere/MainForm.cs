using Accessibility;
using GraphicLibrary;
using GraphicLibrary.Items;
using System.Text;

namespace CreateSphere
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        SharpDXData data;
        ArFloatVector3 lightDirection;
        List<ArIntVector3> vertices;
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
            vertices = new List<ArIntVector3>();
            MainForm_KeyPress(sender, new KeyPressEventArgs(' '));

            lblDepth.Left = 0;
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'q':
                    lightDirection[2] -= 1f;
                    break;
                case 'e':
                    lightDirection[2] += 1f;
                    break;
                case 'w':
                    lightDirection[1] += 1f;
                    break;
                case 's':
                    lightDirection[1] -= 1f;
                    break;
                case 'a':
                    lightDirection[0] -= 1f;
                    break;
                case 'd':
                    lightDirection[0] += 1f;
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
                    lightDirection[2] = 5;
                    vertices.Clear();
                    Random rnd = new Random();
                    vertices.AddRange(Ar3DGeometry.GetTransformedEquilateralTriangle(200));
                    for (int i = 0; i < 8; i++)
                        vertices.AddRange(Ar3DGeometry.GetTransformedEquilateralTriangle(200, new ArIntVector3(rnd.Next(-400, 400), rnd.Next(-400, 400), rnd.Next(-400, 400)), null));
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        if (i % 3 == 0)
                            sb.Append($"T{i / 3}:");
                        sb.AppendLine($"{vertices[i]}");

                    }

                    lblDepth.Text = sb.ToString();
                    lblDepth.Top = ClientSize.Height - lblDepth.Height;
                    break;
                default:
                    break;
            }
            LoadModel(lightDirection);
        }

        bool InTriangle(ArIntVector3 trianglePA, ArIntVector3 trianglePB, ArIntVector3 trianglePC, ArIntVector3 point)
        {
            //比XY
            int minX = trianglePA[0];
            int maxX = trianglePA[0];
            int minY = trianglePA[1];
            int maxY = trianglePA[1];
            if (trianglePB[0] < minX)
                minX = trianglePB[0];
            if (trianglePB[0] > maxX)
                maxX = trianglePB[0];
            if (trianglePB[1] < minY)
                minY = trianglePB[1];
            if (trianglePB[1] > maxY)
                maxY = trianglePB[1];
            if (trianglePC[0] < minX)
                minX = trianglePC[0];
            if (trianglePC[0] > maxX)
                maxX = trianglePC[0];
            if (trianglePC[1] < minY)
                minY = trianglePC[1];
            if (trianglePC[1] > maxY)
                maxY = trianglePC[1];
            if (point.X > maxX || point.X < minX || point.Y > maxY || point.Y < minY)
                return false;
            //不在矩形內
            return true;
        }

        List<ArIntVector3> ComputeShadowTriangle(List<ArIntVector3> vertices, ArFloatVector3 lightDirection)
        {
            List<ArIntVector3> result = new List<ArIntVector3>();
            //result.AddRange(Ar3DGeometry.GetTransformedEquilateralTriangle(200));
            for(int i = 0; i < vertices.Count; i+= 3)
            {
                for(int j = 0; j < vertices.Count; j+= 3)
                {
                    if (i == j)
                        continue;
                    if (vertices[i].Z < vertices[j].Z && vertices[i + 1].Z < vertices[j + 1].Z && vertices[i + 2].Z < vertices[j + 2].Z)
                        continue;
                    //如果在Triangle內，將z值算好之後填入
                    if (InTriangle(vertices[j], vertices[j + 1], vertices[j + 2], vertices[i]) ||
                        InTriangle(vertices[j], vertices[j + 1], vertices[j + 2], vertices[i + 1]) ||
                        InTriangle(vertices[j], vertices[j + 1], vertices[j + 2], vertices[i + 2]))
                    {
                        ArFloatVector3 normal = (vertices[j] - vertices[j + 1]).CrossProduct(vertices[j] - vertices[j + 2]).Normalize();
                        double c = vertices[j][0] * normal[0] + vertices[j][1] * normal[1] + vertices[j][2] * normal[2];
                        //vertices[i][0] * normal[0] + vertices[i][1] * normal[1] + z * normal[2] = c;
                        int z = (int)((c - vertices[i][0] * normal[0] - vertices[i][1] * normal[1]) / normal[2]);
                        result.Add(new ArIntVector3(vertices[i][0], vertices[i][1], z));
                        z = (int)((c - vertices[i + 1][0] * normal[0] - vertices[i + 1][1] * normal[1]) / normal[2]);
                        result.Add(new ArIntVector3(vertices[i + 1][0], vertices[i + 1][1], z));
                        z = (int)((c - vertices[i + 2][0] * normal[0] - vertices[i + 2][1] * normal[1]) / normal[2]);
                        result.Add(new ArIntVector3(vertices[i + 2][0], vertices[i + 2][1], z));
                    }
                }                
            }
            ArFloatMatrix33 rm = GetTransformMatrixFromNormalToZ(lightDirection * -1);
            for (int i = 0; i < result.Count; i++)
                result[i] = (ArIntVector3)(rm * (ArFloatVector3)result[i]);
            return result;
        }
        void LoadModel(ArFloatVector3 lightDirectionVector)
        {
            List<ArIntVector3> verticesO = new List<ArIntVector3>(vertices);
            List<int> indices = new List<int>();
            for (int i = 0; i < vertices.Count; i++)
                indices.Add(i);

            ArFloatVector3 planeN = ArFloatVector3.UnitZ;
            ArFloatVector3 directionV = lightDirectionVector;

            List<ArTextureVertex> vertices3 = new List<ArTextureVertex>();
            ArFloatMatrix33 tm = Ar3DMachine.GetRotateMatrix(new ArFloatVector3(rx, ry, rz));

            for (int i = 0; i < vertices.Count; i++)
                verticesO[i] = (ArIntVector3)(tm * verticesO[i]);

            for (int i = 0; i < vertices.Count; i++)
                vertices3.Add(new ArTextureVertex((int)verticesO[i][0], (int)verticesO[i][1], (int)verticesO[i][2], (float)i / vertices.Count, (float)i / vertices.Count));

            ArFloatMatrix33 tm2 = GetTransformMatrixFromNormalToZ(directionV);
            for (int i = 0; i < vertices.Count; i++)
                verticesO[i] = (ArIntVector3)(tm2 * verticesO[i]);

            //計算ShadowTriangle
            List<ArIntVector3> sht = ComputeShadowTriangle(verticesO, directionV);
            List<int> shti = new List<int>();
            for (int i = 0; i < sht.Count; i++)
                shti.Add(i);

            //顯示
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < sht.Count; i++)
            {
                if (i % 3 == 0)
                    sb.Append($"T{i / 3}:");
                sb.AppendLine($"{sht[i]}");

            }
            lblResult.Text = sb.ToString();
            lblResult.Top = ClientSize.Height - lblResult.Height;
            lblResult.Left = ClientSize.Width - lblResult.Width;
            lblResult.Visible = true;

            List<ArTextureVertex> vertices2 = new List<ArTextureVertex>();
            for (int i = 0; i < sht.Count; i++)
                vertices2.Add(new ArTextureVertex(sht[i][0], sht[i][1], 0, 0, 0));

            if(vertices2.Count > 0)
            {
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
                            Indices = shti.ToArray(),
                        },
                   }
                };
            }
            else
            {
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
                   }
                };
            }
            lblDirection.Text = $"Rotation:({rx},{ry},{rz})\nLight Direction:{lightDirection}";
            sde.LoadModel(data);
            sde.Render();
        }
    }
}
