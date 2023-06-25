using GraphicLibrary2;
using GraphicLibrary2.Items;
using System.Runtime.InteropServices;

namespace ComputePipeline
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    struct ComputeResult
    {
        public ArFloatVector4 a { get; set; }
        public ArFloatVector4 b { get; set; }
        public ArFloatVector4 c { get; set; }
    }

    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        const string hlslFile = @"C:\Programs\GraphicTest\ComputePipeline\Shader\Compute.hlsl";
        public MainForm()
        {
            InitializeComponent();
            sde = new SharpDXEngine();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            sde.SetGrahpicCardAndRenderTarget(new SharpDXInitializeSetting(
                pictureBox1.Handle, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height, true, 1));

            sde.CreateComputeShader(hlslFile);
            sde.UploadComputeData(new float[] { 3, 5, 7 });
            ComputeResult[] crs = sde.Compute<ComputeResult>(3);
            MessageBox.Show(crs[2].a.ToString());
            timer1.Start();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            sde.Close();
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            GC.Collect();
            lblMemory.Text = $"{sde.AdapterName} Shared Memory Use: {GetMB(sde.SharedMemoryUsage)}/{GetMB(sde.SharedSystemMemory)} Mb. Dedicated Memory Use: {GetMB(sde.DedicatedMemoryUsage)}/{GetMB(sde.DedicatedVideoMemory)} Mb";
        }
        double GetMB(long byteCount, int reservedDigits = 2)
           => Math.Round((double)byteCount / 1024 / 1024, reservedDigits);      
    }
}
