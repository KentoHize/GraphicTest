using SharpDX;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;

namespace ConstantBuffer
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        public MainForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 1000);
        }

        private void tsiRun_Click(object sender, EventArgs e)
        {
            sde.Update();
            sde.Render();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            sde = new SharpDXEngine();
            sde.Initialize(new SharpDXSetting
            {
                Handle = pibMain.Handle,
                CullTwoFace = true,
                Viewport = new ViewportF
                {
                    Width = pibMain.ClientSize.Width,
                    Height = pibMain.ClientSize.Height,
                    MaxDepth = 20,
                }
            });

            //Vertex[] triangle = new Vertex[]
            //{
            //    new Vertex{ pos = new SharpDX.Vector3(0, 0.25f, 0), color = new SharpDX.Vector4(0, 1, 0, 1)},
            //    new Vertex{ pos = new SharpDX.Vector3(0.25f, -0.25f, 0), color = new SharpDX.Vector4(1, 0, 0, 1)},
            //    new Vertex{ pos = new SharpDX.Vector3(-0.25f, -0.25f, 0), color = new SharpDX.Vector4(0, 0, 1, 1)},
            //};

            //triangle = new Vertex[]
            //{
            //    new Vertex{ pos = new SharpDX.Vector3(0, 0, 0), color = new SharpDX.Vector4(0, 1, 0, 1)},
            //    new Vertex{ pos = new SharpDX.Vector3(0, 1, 0), color = new SharpDX.Vector4(1, 0, 0, 1)},
            //    new Vertex{ pos = new SharpDX.Vector3(1, 0, 0), color = new SharpDX.Vector4(0, 0, 1, 1)},
            //};

            Vertex[] triangle = new Vertex[]
            {
                new Vertex{ pos = new SharpDX.Vector3(1, 0, 0), color = new SharpDX.Vector4(0, 1, 0, 1)},
                new Vertex{ pos = new SharpDX.Vector3(0, 1, 0), color = new SharpDX.Vector4(1, 0, 0, 1)},
                new Vertex{ pos = new SharpDX.Vector3(0, 0, 0), color = new SharpDX.Vector4(0, 0, 1, 1)},
            };
            triangle = triangle.Reverse().ToArray();

            //triangle = new Vertex[]
            //{
            //        new Vertex() {pos=new Vector3(0.0f, 0.25f, 0.0f ),color=new Vector4(1.0f, 0.0f, 0.0f, 1.0f ) },
            //        new Vertex() {pos=new Vector3(0.25f, -0.25f, 0.0f),color=new Vector4(0.0f, 1.0f, 0.0f, 1.0f) },
            //        new Vertex() {pos=new Vector3(-0.25f, -0.25f, 0.0f),color=new Vector4(0.0f, 0.0f, 1.0f, 1.0f ) },
            //};
            //triangle.Reverse();
            sde.Load(new SharpDXData
            {
                BackgroundColor = System.Drawing.Color.Black,
                GraphicData = new SharpDXBundleData[]
                {
                    new SharpDXBundleData
                    {
                        Data = triangle,
                        PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList
                    }
                }
            });
            sde.Render();
        }

        private void t1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sde.UpdateConstantBuffer(new SharpDXEngine.CB1
            {
                Position = new Vector4(0, 0, 0, 0),
                Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
            });
        }
    }
}