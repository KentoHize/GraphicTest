using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathTest
{
    public partial class MatrixSimplification : Form
    {
        string[,] result = new string[4, 4];

        public MatrixSimplification()
        {
            InitializeComponent();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    TextBox txtMatrixItem = new TextBox
                    {
                        Name = $"txt_{i}_{j}",
                        Left = i * 50,
                        Top = j * 50,
                        Width = 50,
                    };
                    Controls.Add(txtMatrixItem);
                }
            }


        }

        private void txtMatrix_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    //result[i, j].
                }
            }   
        }
    }
}
