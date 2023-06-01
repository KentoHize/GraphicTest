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

            sde.Render();
        }
    }
}