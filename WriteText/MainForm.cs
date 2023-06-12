using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WriteText
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        public MainForm()
        {
            InitializeComponent();
            sde = new SharpDXEngine();
        }
    }
}
