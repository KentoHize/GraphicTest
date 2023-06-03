using GraphicLibrary;
using GraphicLibrary.Items;
using System.Diagnostics;

namespace ComputeMatrix
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde { get; set; }
        private Ar3DArea GetArea()
        {
            Ar3DArea area = new Ar3DArea();
            //Ar3DModelGroup modelGroup = new Ar3DModelGroup();
            area.ModelGroups = new Ar3DModelGroup[] { new Ar3DModelGroup() };
            area.ModelGroups[0].Models = new Ar3DModel[] { new Ar3DModel() };
            area.ModelGroups[0].Models[0].Planes = new ArPlane[] {
                new ArPlane(new ArVertex[]
                {
                    new ArVertex(1000, 1000, 0),
                    new ArVertex(1000, 0, 0),
                    new ArVertex(0, 1000, 0),
                })
            };
            return area;
        }

        public MainForm()
        {
            InitializeComponent();
            sde = new SharpDXEngine();
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
                BackgroundColor = Color.Black.ToArFloatVector4(),
                VerteicesData = new SharpDXBundleData[]
                {
                    new SharpDXBundleData
                    {
                        PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
                        Verteices = new ArVertex[]
                        {
                            new ArVertex(0, 0, 0, Color.Blue),
                            new ArVertex(512, 0, 0, Color.White),
                            new ArVertex(0, 512, 0, Color.White),
                            new ArVertex(512, 512, 0, Color.Red)
                        },
                        Indices = new int[]
                        {
                            0, 1, 2, 1, 2, 3
                        },
                        TransformMartrix = Ar3DMachine.ProduceTransformMatrix(
                            new ArIntVector3(-200, 0, 0),
                            new ArFloatVector3(0, 0, 0),
                            new ArFloatVector3(1, 1, 1))
                    }
                }
                
            };
            Stopwatch sw = Stopwatch.StartNew();
            sde.Load(data);
            sw.Stop();
            Debug.WriteLine($"Load Data:{sw.ElapsedMilliseconds}");
            sw.Restart();
            sde.Render();
            Debug.WriteLine($"Render:{sw.ElapsedMilliseconds}");
        }
    }
}