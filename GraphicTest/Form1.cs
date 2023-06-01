using System.Drawing.Drawing2D;
using SharpDX.Direct3D12;


namespace GraphicTest
{
    public partial class Form1 : Form
    {
        public int transformX = 0;
        public int transformY = 0;

        public Form1()
        {
            InitializeComponent();
        }

        Point TranslateTransform(Point p, int transformX, int transformY)
        {
            return new Point(p.X + transformX, p.Y + transformY);
        }

        Point RotateTransform(Point p, double multipierY, double multipierZ1, double multipierZ2)
        {
            return new Point((int)(p.X * multipierZ1 - p.Y * multipierZ2), (int)((p.Y * multipierZ1 + p.X * multipierZ2) * multipierY));
        }

        Point AmplificationTransform(Point p, int amplificationFactor)
        {
            return new Point(p.X * amplificationFactor, p.Y * amplificationFactor);
        }

        public void DrawBlock(Graphics g)
        {
            Point[] p = new Point[4];
            Pen pen = new Pen(Color.Black, 1);
            for (int i = 0; i < 300; i++)
            {
                for (int j = 0; j < 300; j++)
                {
                    p[0] = new Point(i, j);
                    p[1] = new Point(i + 1, j);
                    p[2] = new Point(i + 1, j + 1);
                    p[3] = new Point(i, j + 1);
                    for (int k = 0; k < 4; k++)
                    {
                        p[k] = AmplificationTransform(p[k], 20);
                        p[k] = TranslateTransform(p[k], transformX, transformY);
                    }
                        
                    g.DrawPolygon(pen, p);
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            DrawBlock(e.Graphics);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch(e.KeyChar)
            {
                case 'e':
                    transformX += 10;
                    break;
                case 'q':
                    transformX -= 10;
                    break;
                default:
                    return;
            }
            pibMain.Invalidate();
        }
    }
}