using GraphicLibrary.Items;
using GraphicLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DepthBuffer
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        const string textureFile = @"C:\Programs\GraphicTest\DepthBuffer\Textures\AnnetteSquare.bmp";
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

            var aCube = Ar3DGeometry.GetMixCube(512);

            SharpDXData data = new SharpDXData
            {
                BackgroundColor = Color.Black.ToArFloatVector4(),
                TransformMartrix = Ar3DMachine.ProduceTransformMatrix(
                                        new ArIntVector3(0, 0, 0),
                                        new ArFloatVector3(-0.2f, 0.2f, 0.7f),
                                        //new ArFloatVector3(0, 0, 0),
                                        new ArFloatVector3(1, 1, 1)),
                VerticesData = new SharpDXBundleData[]
                {
                    new SharpDXBundleData
                    {
                        PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                        MixVertices = aCube.vertices,
                        Indices = aCube.indices,
                    },
                    //new SharpDXBundleData
                    //{
                    //    PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                    //    ColorVertices = new ArColorVertex[]
                    //    {
                    //        new ArColorVertex(-500, 500, 500, 0, 1, 0, 1),
                    //        new ArColorVertex(500, 500, 500, 0, 1, 0, 1),
                    //        new ArColorVertex(500, 500, -500, 0, 1, 0, 1),
                    //        new ArColorVertex(-500, 500, -500, 0, 1, 0, 1),

                    //        new ArColorVertex(-500, -500, 500, 1, 0, 1, 1),
                    //        new ArColorVertex(500, -500, 500, 1, 0, 1, 1),
                    //        new ArColorVertex(500, -500, -500, 1, 0, 1, 1),
                    //        new ArColorVertex(-500, -500, -500, 1, 0, 1, 1),

                    //        new ArColorVertex(-500, -500, 500, 1, 0, 0, 1),
                    //        new ArColorVertex(-500, 500, 500, 1, 0, 0, 1),
                    //        new ArColorVertex(-500, 500, -500, 1, 0, 0, 1),
                    //        new ArColorVertex(-500, -500, -500, 1, 0, 0, 1),

                    //        new ArColorVertex(500, -500, 500, 1, 1, 0, 1),
                    //        new ArColorVertex(500, 500, 500, 1, 1, 0, 1),
                    //        new ArColorVertex(500, 500, -500, 1, 1, 0, 1),
                    //        new ArColorVertex(500, -500, -500, 1, 1, 0, 1),

                    //        new ArColorVertex(-500, 500, 500, 0, 1, 1, 1),
                    //        new ArColorVertex(500, 500, 500, 0, 1, 1, 1),
                    //        new ArColorVertex(500, -500, 500, 0, 1, 1, 1),
                    //        new ArColorVertex(-500, -500, 500, 0, 1, 1, 1),

                    //        new ArColorVertex(-500, 500, -500, 0, 0, 1, 1),
                    //        new ArColorVertex(500, 500, -500, 0, 0, 1, 1),
                    //        new ArColorVertex(500, -500, -500, 0, 0, 1, 1),
                    //        new ArColorVertex(-500, -500, -500, 0, 0, 1, 1),
                    //    },
                    //    Indices =  new int[]{
                    //        0,1,2,0,2,3,
                    //        4,6,5,4,7,6,
                    //        8,9,10,8,10,11,
                    //        12,14,13,12,15,14,
                    //        16,18,17,16,19,18,
                    //        20,21,22,20,22,23
                    //    }
                    //}
                }
            };
            sde.LoadData(data);
            sde.Render();
            sde.Close();
        }
    }
}
