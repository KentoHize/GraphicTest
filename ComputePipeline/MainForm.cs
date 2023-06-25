using GraphicLibrary2;
using GraphicLibrary2.Items;

namespace ComputePipeline
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        public MainForm()
        {
            InitializeComponent();
            sde = new SharpDXEngine();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            sde.SetGrahpicCardAndRenderTarget()
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetDesktopBounds().C
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            GC.Collect();
            lblMemoryInfo.Text = $"{sde.AdapterName} Shared Memory Use: {GetMB(sde.SharedMemoryUsage)}/{GetMB(sde.SharedSystemMemory)} Mb. Dedicated Memory Use: {GetMB(sde.DedicatedMemoryUsage)}/{GetMB(sde.DedicatedVideoMemory)} Mb";
        }
        double GetMB(long byteCount, int reservedDigits = 2)
           => Math.Round((double)byteCount / 1024 / 1024, reservedDigits);
    }
}
