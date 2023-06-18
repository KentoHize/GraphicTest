using System.Text.RegularExpressions;

namespace MathTest
{
    public partial class MainForm : Form
    {
        public Point Origin;
        public int MaxX;
        public int MaxY;
        public int GridInterval = 50;

        public Pen GridPen;
        public Pen GridBoldPen;
        public Pen P1Pen;
        public Pen P2Pen;
        public Pen P3Pen;
        public Pen PPPen;
        public Pen LinePen;

        public MainForm()
        {
            InitializeComponent();
            GridPen = new Pen(Color.Black, 1);
            GridBoldPen = new Pen(Color.Black, 2);
            lblPointA.ForeColor = Color.Red;
            lblPointB.ForeColor = Color.Blue;
            lblPointC.ForeColor = Color.Green;
            lblPointP.ForeColor = Color.White;
            P1Pen = new Pen(lblPointA.ForeColor, 1);
            P2Pen = new Pen(lblPointB.ForeColor, 1);
            P3Pen = new Pen(lblPointC.ForeColor, 1);
            PPPen = new Pen(lblPointP.ForeColor, 1);
            LinePen = new Pen(Color.DarkGray, 2);
        }


        public void DrawGrids(Graphics g)
        {
            for (int i = 1; i <= MaxX / 2; i += GridInterval)
            {
                g.DrawLine(GridPen, Origin.X + i, 0, Origin.X + i, MaxY);
                g.DrawLine(GridPen, Origin.X - i, 0, Origin.X - i, MaxY);
            }

            for (int i = 1; i <= MaxY / 2; i += GridInterval)
            {
                g.DrawLine(GridPen, 0, Origin.Y + i, MaxX, Origin.Y + i);
                g.DrawLine(GridPen, 0, Origin.Y - i, MaxX, Origin.Y - i);
            }
            g.DrawLine(GridBoldPen, 0, Origin.Y, MaxX, Origin.Y);
            g.DrawLine(GridBoldPen, Origin.X, 0, Origin.X, MaxY);
        }
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawGrids(g);
            Point A, B, C, P;
            A = GetCoordFromText(txtPointA.Text);
            B = GetCoordFromText(txtPointB.Text);
            C = GetCoordFromText(txtPointC.Text);
            P = GetCoordFromText(txtPointP.Text);
            Point dA = GetDrawPoint(A);
            Point dB = GetDrawPoint(B);
            Point dC = GetDrawPoint(C);
            Point dP = GetDrawPoint(P);
            g.FillEllipse(P1Pen, dA);
            g.FillEllipse(P2Pen, dB);
            g.FillEllipse(P3Pen, dC);
            g.FillEllipse(PPPen, dP);

            g.DrawLine(LinePen, dA, dB);
            g.DrawLine(LinePen, dA, dC);
            g.DrawLine(LinePen, dB, dC);
            //Label lblDescrition = new Label
            //{
            //    Text = $"AF"
            //}
            //Draw 2 Point
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Refresh();

        }

        public void Refresh()
        {
            Origin.X = ClientSize.Width / 2;
            Origin.Y = ClientSize.Height / 2;
            MaxX = ClientSize.Width;
            MaxY = ClientSize.Height;
            Invalidate();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!ValidateCoordText(txtPointA.Text) ||
               !ValidateCoordText(txtPointB.Text) ||
               !ValidateCoordText(txtPointC.Text) ||
               !ValidateCoordText(txtPointP.Text))
            {
                MessageBox.Show("öŒëÀ•W");
                return;
            }

            Refresh();
        }

        private void txtPoint_Validated(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!ValidateCoordText(tb.Text))
                erpMain.SetError(tb, "x,y");
            else
                erpMain.SetError(tb, "");
        }

        public bool ValidateCoordText(string text)
            => Regex.IsMatch(text.Replace(" ", ""), "^[0-9]+,[0-9]+$");

        public Point GetCoordFromText(string text)
        {
            string[] s = text.Split(',');
            return new Point(int.Parse(s[0]), int.Parse(s[1]));
        }

        public Point GetDrawPoint(Point a)
            => new Point(Origin.X + a.X, Origin.Y - a.Y);

    }

    public static class Extension
    {
        public static void FillEllipse(this Graphics g, Pen pen, Point p, int radius = 10)
            => g.FillEllipse(new SolidBrush(pen.Color), p.X - radius / 2, p.Y - radius / 2, radius, radius);
    }
}