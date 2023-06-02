using System.Security.Cryptography.X509Certificates;

namespace ConstantBuffer
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 1000);

        }

        private void tsiRun_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SharpDXEngine sde = new SharpDXEngine();
            sde.Initialize(new SharpDXSetting
            {
                Handle = pibMain.Handle,
                CullTwoFace = true,
                Viewport = new SharpDX.Direct3D12.Viewport
                {
                    Width = pibMain.ClientSize.Width,
                    Height = pibMain.ClientSize.Height,
                }
            });

            Vertex[] triangle = new Vertex[]
            {
                new Vertex{ pos = new SharpDX.Vector3(1, 1, 0), color = new SharpDX.Vector4(1, 0, 0, 1)},
                new Vertex{ pos = new SharpDX.Vector3(1, 0, 0), color = new SharpDX.Vector4(0, 1, 0, 1)},
                new Vertex{ pos = new SharpDX.Vector3(0, 1, 0), color = new SharpDX.Vector4(0, 0, 1, 1)},
            };
            sde.Load(new SharpDXData
            {
                BackgroundColor = Color.White,
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