using GraphicLibrary2;
using GraphicLibrary2.Items;

namespace Camera
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
            sde.SetGrahpicCardAndRenderTarget(new SharpDXInitializeSetting(
                pictureBox1.Handle, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height, true, 1));
            //Load Texture
            //Load Material
            //Load Model
            //sde.LoadGraphicSetting(new SharpDXGraphicSetting 
            //    );
            //PrepareCreateInstance
            //CreateInstance
            //SetInsatnce
            //PrepareRender
            //Render
            sde.Close();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
