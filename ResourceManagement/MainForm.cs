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

        const string textureFile = @"C:\Programs\GraphicTest\ResourceManagement\Texture\AnnetteSquare.bmp";
        const string textureFile2 = @"C:\Programs\GraphicTest\ResourceManagement\Texture\ClacierSquare.bmp";
        public MainForm()
        {
            sde = new SharpDXEngine();
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            sde.LoadSetting(new SharpDXSetting
            {
                CullTwoFace = false,
                DrawClockwise = false,
                Viewport = new SharpDX.ViewportF(0, 0, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height),
                FrameCount = 2,
                SwapEffect = SharpDX.DXGI.SwapEffect.FlipDiscard,
                Handle = pictureBox1.Handle
            });

            //sde.LoadTexture(new SharpDXTextureData
            //{
            //    Data = Ar3DMachine.LoadBitmapFromFile(textureFile, out int width, out int height),
            //    Width = width,
            //    Height = height
            //});            
            sde.LoadStaticData();
           
            sde.LoadModel("TestObject", new ArDirect3DModel
            {
                Vertices = new ArDirect3DVertex[]
                {
                    new ArDirect3DVertex{ Position = new ArIntVector3(0, 0, 0), TextureCoordinate = new ArFloatVector3(1, 1, 1), ShadowCoordinate = new ArFloatVector3(1, 1, 1) },
                    new ArDirect3DVertex{ Position = new ArIntVector3(512, 0, 0), TextureCoordinate = new ArFloatVector3(1, 1, 1), ShadowCoordinate = new ArFloatVector3(1, 1, 1) },                    
                    new ArDirect3DVertex{ Position = new ArIntVector3(512, 512, 0), TextureCoordinate = new ArFloatVector3(1, 1, 1), ShadowCoordinate = new ArFloatVector3(1, 1, 1) },
                    new ArDirect3DVertex{ Position = new ArIntVector3(0, 512, 0), TextureCoordinate = new ArFloatVector3(1, 1, 1), ShadowCoordinate = new ArFloatVector3(1, 1, 1) }
                },
                Indices = new int[]
                {
                    0, 1, 2, 0, 2, 3
                }
            });
            //設置攝影機
            //設置光
            sde.SetModel("TestObject", new ArIntVector3(100, 100, 100));
            //CreateVertex
            //CountShadow
            //Draw

            unsafe
            {
                ArDirect3DModel ad3m = new ArDirect3DModel();
                ArDirect3DVertex ad3 = new ArDirect3DVertex();
                ad3.Position = new ArIntVector3(1, 1, 1);
                ad3.TextureCoordinate = new ArFloatVector3(1, 1, 1);
                ad3.ShadowCoordinate = new ArFloatVector3(1, 1, 1);
                //ArIntVector3 ex = new ArIntVector3(1, 1, 1);
                //byte* addr = (byte*)&ex;
                ad3m._vertices = new ArDirect3DVertex[] { ad3 };
                //Debug.WriteLine("Size:      {0}", Marshal.SizeOf<ArIntVector3>());
                //Debug.WriteLine("_x Offset: {0}", Marshal.OffsetOf<ArIntVector3>("_x"));
                //Debug.WriteLine("_y Offset: {0}", Marshal.OffsetOf<ArIntVector3>("_y"));
                //Debug.WriteLine("_z Offset: {0}", Marshal.OffsetOf<ArIntVector3>("_z"));

                //Debug.WriteLine("Size:      {0}", Marshal.SizeOf<ArDirect3DVertex>());
                //Debug.WriteLine("_position Offset: {0}", Marshal.OffsetOf<ArDirect3DVertex>("_position"));
                //Debug.WriteLine("_textureCoordinate Offset: {0}", Marshal.OffsetOf<ArDirect3DVertex>("_textureCoordinate"));
                //Debug.WriteLine("_shadowCoordinate Offset: {0}", Marshal.OffsetOf<ArDirect3DVertex>("_shadowCoordinate"));

                //Debug.WriteLine("Size:      {0}", Marshal.SizeOf<ArDirect3DModel>());
                //Debug.WriteLine("_vertices Offset: {0}", Marshal.OffsetOf<ArDirect3DModel>("_vertices"));
                //Debug.WriteLine("_indices Offset: {0}", Marshal.OffsetOf<ArDirect3DModel>("_indices"));
            }

            sde.Render();
            timer1.Start();
            //sde.LoadTexture(new SharpDXTextureData
            //{
            //    Data = Ar3DMachine.LoadBitmapFromFile(textureFile2, out int width2, out int height2),
            //    Width = width2,
            //    Height = height2
            //});
            //sde.LoadStaticData(new SharpDXStaticData
            //{
            //    Textures = new SharpDXTextureData[]
            //    {
            //        new SharpDXTextureData
            //        {
            //            Data = Ar3DMachine.LoadBitmapFromFile(textureFile, out int width, out int height),
            //            Width = width,
            //            Height = height
            //        }
            //    }
            //});

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
            
        }

        double GetMB(long byteCount, int reservedDigits = 2)
            => Math.Round((double)byteCount / 1024 / 1024, reservedDigits);
    }
}
