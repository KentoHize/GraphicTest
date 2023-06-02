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
            //sde.Render();
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
                }
            });

            Vertex[] triangle = new Vertex[]
            {
                new Vertex{ pos = new SharpDX.Vector3(1, 0, 0), color = new SharpDX.Vector4(0, 1, 0, 1)},
                new Vertex{ pos = new SharpDX.Vector3(1, 1, 0), color = new SharpDX.Vector4(1, 0, 0, 1)},
                new Vertex{ pos = new SharpDX.Vector3(0, 1, 0), color = new SharpDX.Vector4(0, 0, 1, 1)},
            };
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
    }
}