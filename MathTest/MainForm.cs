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

        public MainForm()
        {
            InitializeComponent();

        }


        public void GetGrid()
        {

        }
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            //Grid

            for (int i = 1; i <= MaxX / 2; i += GridInterval)
            {
                e.Graphics.DrawLine(GridPen, Origin.X + i, 0, Origin.X + i, MaxY);
                e.Graphics.DrawLine(GridPen, Origin.X - i, 0, Origin.X - i, MaxY);
            }
                
            for (int i = 1; i <= MaxY / 2; i += GridInterval)
            {
                e.Graphics.DrawLine(GridPen, 0, Origin.Y + i, MaxX, Origin.Y + i);
                e.Graphics.DrawLine(GridPen, 0, Origin.Y - i, MaxX, Origin.Y - i);
            }
            e.Graphics.DrawLine(GridBoldPen, 0, Origin.Y, MaxX, Origin.Y);
            e.Graphics.DrawLine(GridBoldPen, Origin.X, 0, Origin.X, MaxY);
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
            GridPen = new Pen(Color.Black, 1);
            GridBoldPen = new Pen(Color.Black, 2);
            Invalidate();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}