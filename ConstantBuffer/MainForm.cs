using SharpDX;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;

namespace ConstantBuffer
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        bool Run;
        float posx = 0;
        float colorr = 0;
        float colorg = 0;
        float colorb = 1.0f;
        public MainForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 1000);
        }

        private void tsiRun_Click(object sender, EventArgs e)
        {
            Run = true;
            timMain.Enabled = true;
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

            Vertex[] triangle = new Vertex[]
            {
                new Vertex{ pos = new SharpDX.Vector3(1, 0, 0), color = new SharpDX.Vector4(0, 1, 0, 1)},
                new Vertex{ pos = new SharpDX.Vector3(0, 1, 0), color = new SharpDX.Vector4(1, 0, 0, 1)},
                new Vertex{ pos = new SharpDX.Vector3(0, 0, 0), color = new SharpDX.Vector4(0, 0, 1, 1)},
            };
            triangle = triangle.Reverse().ToArray();
        
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
            colorr = 1 - colorr;
            UpdateConstantBuffer();
        }

        private void pibMain_Click(object sender, EventArgs e)
        {

        }

        private void timMain_Tick(object sender, EventArgs e)
        {
            if (Run)
            {
                posx += 0.02f;
                if (posx > 1.5f)
                    posx = -2f;
                UpdateConstantBuffer();
            }
        }

        public void UpdateConstantBuffer()
        {
            sde.UpdateConstantBuffer(new SharpDXEngine.CB1
            {
                Position = new Vector4(posx, 0, 0, 0),
                Color = new Vector4(colorr, colorg, colorb, 1.0f)
            });
        }

        private void tmiStop_Click(object sender, EventArgs e)
        {
            Run = false;
            timMain.Enabled = false;
        }

        private void t2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorg = 1 - colorg;
            UpdateConstantBuffer();
        }

        private void t3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorb = 1 - colorb;
            UpdateConstantBuffer();
        }
    }
}