using GraphicLibrary2;
using GraphicLibrary2.Items;
using System.Diagnostics;

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

        void MainForm_Load(object sender, EventArgs e)
        {
            sde.SetGrahpicCardAndRenderTarget(new SharpDXInitializeSetting(
                pictureBox1.Handle, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height, true, 1));

            for (int i = 0; i < 8; i++)
                LoadTexture(i);


            sde.CreateComputeShader();
            sde.Compute();
            //Debug.WriteLine($"Load 8 Picture: {sw.ElapsedMilliseconds}");

            //sde.LoadMaterial(0, new ArMaterial
            //{
            //    TextureIndex = 0
            //});
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
            //sde.Close();
            timer1.Enabled = true;
        }

        void MainForm_Paint(object sender, PaintEventArgs e)
        {

        }

        void timer1_Tick(object sender, EventArgs e)
        {
            GC.Collect();
            lblMemoryInfo.Text = $"{sde.AdapterName} Shared Memory Use: {GetMB(sde.SharedMemoryUsage)}/{GetMB(sde.SharedSystemMemory)} Mb. Dedicated Memory Use: {GetMB(sde.DedicatedMemoryUsage)}/{GetMB(sde.DedicatedVideoMemory)} Mb";

        }
        double GetMB(long byteCount, int reservedDigits = 2)
           => Math.Round((double)byteCount / 1024 / 1024, reservedDigits);

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch(e.KeyChar)
            {
                case '1':
                    if (sde.TextureExist(1))
                        sde.DeleteTexture(1);
                    else
                        LoadTexture(1);                        
                    break;
                case '2':
                    if (sde.TextureExist(2))
                        sde.DeleteTexture(2);
                    else
                        LoadTexture(2);
                    break;
                case '3':
                    if (sde.TextureExist(3))
                        sde.DeleteTexture(3);
                    else
                        LoadTexture(3);
                    break;
            }
        }

        void LoadTexture(int index)
        {
            switch(index)
            {
                case 0:
                    sde.LoadTextureFromBitmapFile(0, Path.Combine(Constant.TextureFolder, "AnnetteSquare.bmp"));
                    break;
                case 1:
                    sde.LoadTextureFromBitmapFile(1, Path.Combine(Constant.TextureFolder, "Ayane.bmp"));
                    break;
                case 2:
                    sde.LoadTextureFromBitmapFile(2, Path.Combine(Constant.TextureFolder, "ClacierSquare.bmp"));
                    break;
                case 3:
                    sde.LoadTextureFromBitmapFile(3, Path.Combine(Constant.TextureFolder, "Kanade.bmp"));
                    break;
                case 4:
                    sde.LoadTextureFromBitmapFile(4, Path.Combine(Constant.TextureFolder, "Sento.bmp"));
                    break;
                case 5:
                    sde.LoadTextureFromBitmapFile(5, Path.Combine(Constant.TextureFolder, "Sonia.bmp"));
                    break;
                case 6:
                    sde.LoadTextureFromBitmapFile(6, Path.Combine(Constant.TextureFolder, "Sophia.bmp"));
                    break;
                case 7:
                    sde.LoadTextureFromBitmapFile(7, Path.Combine(Constant.TextureFolder, "Yuri.bmp"));
                    break;
            }
        }
    }
}
