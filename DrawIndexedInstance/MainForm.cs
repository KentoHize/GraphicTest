using GraphicLibrary;
using GraphicLibrary.Items;
using System.Xml.Schema;

namespace DrawIndexedInstance
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde { get; set; }
        public MainForm()
        {
            InitializeComponent();
            sde = new SharpDXEngine();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Exit = true;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (sde != null)
                sde.Render();
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


            SharpDXData data = new SharpDXData
            {
                BackgroundColor =  Color.Wheat.ToArFloatVector4()
            };
            //
            sde.Load(data);
            sde.Render();
        }
    }
}