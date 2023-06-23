using GraphicLibrary.Items;
using GraphicLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Schema;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ResourceManagement
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        SharpDXData data;
        //float rx = 0, ry = 0, rz = 0;

        ArIntVector3 p1, p2, p3;

        const string textureFolder = @"C:\Programs\GraphicTest\ResourceManagement\Texture\";
        public MainForm()
        {
            sde = new SharpDXEngine();
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            sde.LoadSetting(new SharpDXSetting
            {
                CullTwoFace = false,
                DrawClockwise = false,
                Viewport = new SharpDX.ViewportF(0, 0, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height),
                FrameCount = 2,
                SwapEffect = SharpDX.DXGI.SwapEffect.FlipDiscard,
                Handle = pictureBox1.Handle
            });

            sw.Stop();
            sw.Restart();
            sde.LoadTextureFromFile(0, Path.Combine(textureFolder, "AnnetteSquare.bmp"));
            sde.LoadTextureFromFile(1, Path.Combine(textureFolder, "Ayane.bmp"));
            sde.LoadTextureFromFile(2, Path.Combine(textureFolder, "ClacierSquare.bmp"));
            sde.LoadTextureFromFile(3, Path.Combine(textureFolder, "Kanade.bmp"));
            sde.LoadTextureFromFile(4, Path.Combine(textureFolder, "Sento.bmp"));
            sde.LoadTextureFromFile(5, Path.Combine(textureFolder, "Sonia.bmp"));
            sde.LoadTextureFromFile(6, Path.Combine(textureFolder, "Sophia.bmp"));
            sde.LoadTextureFromFile(7, Path.Combine(textureFolder, "Yuri.bmp"));
            sw.Stop();
            Debug.WriteLine($"Load 8 Picture: {sw.ElapsedMilliseconds}");

            sde.LoadMaterial(0, new ArMaterial
            {
                TextureIndex = 0
            });

            sde.LoadMaterial(1, new ArMaterial
            {
                TextureIndex = 1
            });

            sde.PrepareLoadModel();

            sde.LoadModel("TestObject", new ArDirect3DModel
            {
                Vertices = new ArDirect3DVertex[]
                {
                    new ArDirect3DVertex{ Position = new ArIntVector3(0, 0, 0), MaterialIndex = 0, TextureCoordinate = new ArFloatVector2(0, 1), ShadowCoordinate = new ArFloatVector3(1, 1, 1) },
                    new ArDirect3DVertex{ Position = new ArIntVector3(512, 0, 0), MaterialIndex = 0, TextureCoordinate = new ArFloatVector2(1, 1), ShadowCoordinate = new ArFloatVector3(1, 1, 1) },
                    new ArDirect3DVertex{ Position = new ArIntVector3(512, 512, 0), MaterialIndex = 0, TextureCoordinate = new ArFloatVector2(1, 0), ShadowCoordinate = new ArFloatVector3(1, 1, 1) },
                    new ArDirect3DVertex{ Position = new ArIntVector3(0, 512, 0), MaterialIndex = 0, TextureCoordinate = new ArFloatVector2(0, 0), ShadowCoordinate = new ArFloatVector3(1, 1, 1) }
                },
                Indices = new int[]
                {
                    0, 1, 2, 0, 2, 3
                },                
            });

            sde.LoadModel("TestObject2", new ArDirect3DModel
            {
                Vertices = new ArDirect3DVertex[]
                {
                    new ArDirect3DVertex{ Position = new ArIntVector3(0, 0, 0), MaterialIndex = 2, TextureCoordinate = new ArFloatVector2(1, 0), ShadowCoordinate = new ArFloatVector3(1, 1, 1) },
                    new ArDirect3DVertex{ Position = new ArIntVector3(-512, 0, 0), MaterialIndex = 2, TextureCoordinate = new ArFloatVector2(0, 0), ShadowCoordinate = new ArFloatVector3(1, 1, 1) },
                    new ArDirect3DVertex{ Position = new ArIntVector3(-512, -512, 0), MaterialIndex = 2, TextureCoordinate = new ArFloatVector2(0, 1), ShadowCoordinate = new ArFloatVector3(1, 1, 1) },
                    new ArDirect3DVertex{ Position = new ArIntVector3(0, -512, 0), MaterialIndex = 2, TextureCoordinate = new ArFloatVector2(1, 1), ShadowCoordinate = new ArFloatVector3(1, 1, 1) }
                },
                Indices = new int[]
                {
                    0, 1, 2, 0, 2, 3
                },                
            });

            sde.PrepareCreateInstance();
            //設置攝影機
            //sde.SetPerspectiveCamera()
            //設置光
            p1 = new ArIntVector3(100, 100, 0);
            sde.CreateInstance("TestObject", 0, p1);            
            p2 = new ArIntVector3(0, 0, 0);
            sde.CreateInstance("TestObject2", 1, p2);
            p3 = new ArIntVector3(300, 200, -10);
            sde.CreateInstance("TestObject2", 2, p3);
            //CreateVertex
            //CountShadow
            //Draw
            //結合模組/Texture
            sde.PrepareRender();

            sde.Render();
            timer1.Start();

            //var aCube = Ar3DGeometry.GetTextureCube(512);

            //data = new SharpDXData
            //{
            //    BackgroundColor = Color.Black.ToArFloatVector4(),
            //    TransformMartrix = Ar3DMachine.ProduceTransformMatrix(
            //                            new ArIntVector3(0, 0, 0),
            //                            new ArFloatVector3(1.7f, -0.2f, 0.2f),
            //                            new ArFloatVector3(1, 1, 1)),
            //    VerticesData = new SharpDXBundleData[]
            //    {
            //        new SharpDXBundleData
            //        {
            //            PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
            //            TextureVertices = aCube.vertices,
            //            Indices = aCube.indices,
            //        }
            //        //new SharpDXBundleData
            //        //{
            //        //    PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
            //        //    TextureVertices = new ArTextureVertex[]
            //        //    {
            //        //        new ArTextureVertex(0, 0, 0, 0, 1),
            //        //        new ArTextureVertex(512, 0, 0, 1, 1),
            //        //        new ArTextureVertex(0, 512, 0, 0, 0),
            //        //        new ArTextureVertex(512, 512, 0, 1, 0)
            //        //    },
            //        //    Indices = new int[]
            //        //    {
            //        //        1, 0, 2, 1, 2, 3
            //        //    },
            //        //    TextureIndex = 0
            //        //},
            //        //new SharpDXBundleData
            //        //{
            //        //    PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList,
            //        //    TextureVertices = new ArTextureVertex[]
            //        //    {
            //        //        new ArTextureVertex(0, 0, 0, 1, 0),
            //        //        new ArTextureVertex(-512, 0, 0, 0, 0),
            //        //        new ArTextureVertex(0, -512, 0, 1, 1),
            //        //        new ArTextureVertex(-512, -512, 0, 0, 1)
            //        //    },
            //        //    Indices = new int[]
            //        //    {
            //        //        1, 0, 2, 1, 2, 3
            //        //    },
            //        //    TextureIndex = 1
            //        //}
            //    }
            //};
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblCPUMemory.Text = $"{sde.AdapterName} Shared Memory Use: {GetMB(sde.SharedMemoryUsage)}/{GetMB(sde.SaredSystemMemory)} Mb. Dedicated Memory Use: {GetMB(sde.DedicatedMemoryUsage)}/{GetMB(sde.DedicatedVideoMemory)} Mb";
            GC.Collect();

            p3[0] += 10;
            if (p3[0] > pictureBox1.ClientSize.Width)
                p3[0] = 0;

            //if (p3[0] >= 300)

            sde.SetInstance(2, p3);
            sde.Render();
        }

        double GetMB(long byteCount, int reservedDigits = 2)
            => Math.Round((double)byteCount / 1024 / 1024, reservedDigits);
    }

    //unsafe
    //{
    //    ArDirect3DModel ad3m = new ArDirect3DModel();
    //    ArDirect3DVertex ad3 = new ArDirect3DVertex();
    //    ad3.Position = new ArIntVector3(1, 1, 1);
    //    ad3.TextureCoordinate = new ArFloatVector3(1, 1, 1);
    //    ad3.ShadowCoordinate = new ArFloatVector3(1, 1, 1);
    //    //ArIntVector3 ex = new ArIntVector3(1, 1, 1);
    //    //byte* addr = (byte*)&ex;
    //    ad3m._vertices = new ArDirect3DVertex[] { ad3 };
    //    //Debug.WriteLine("Size:      {0}", Marshal.SizeOf<ArIntVector3>());
    //    //Debug.WriteLine("_x Offset: {0}", Marshal.OffsetOf<ArIntVector3>("_x"));
    //    //Debug.WriteLine("_y Offset: {0}", Marshal.OffsetOf<ArIntVector3>("_y"));
    //    //Debug.WriteLine("_z Offset: {0}", Marshal.OffsetOf<ArIntVector3>("_z"));

    //    //Debug.WriteLine("Size:      {0}", Marshal.SizeOf<ArDirect3DVertex>());
    //    //Debug.WriteLine("_position Offset: {0}", Marshal.OffsetOf<ArDirect3DVertex>("_position"));
    //    //Debug.WriteLine("_textureCoordinate Offset: {0}", Marshal.OffsetOf<ArDirect3DVertex>("_textureCoordinate"));
    //    //Debug.WriteLine("_shadowCoordinate Offset: {0}", Marshal.OffsetOf<ArDirect3DVertex>("_shadowCoordinate"));

    //    //Debug.WriteLine("Size:      {0}", Marshal.SizeOf<ArDirect3DModel>());
    //    //Debug.WriteLine("_vertices Offset: {0}", Marshal.OffsetOf<ArDirect3DModel>("_vertices"));
    //    //Debug.WriteLine("_indices Offset: {0}", Marshal.OffsetOf<ArDirect3DModel>("_indices"));
    //}

}
