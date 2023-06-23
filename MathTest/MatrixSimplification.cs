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
        TextBox[,] m1, m2, m3;
        public MatrixSimplification()
        {
            InitializeComponent();
            m1 = new TextBox[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    TextBox txtMatrixItem = new TextBox
                    {
                        Name = $"txt1_{i}_{j}",
                        Left = 20 + i * 50,
                        Top = 20 + j * 50,
                        Width = 50,
                        Text = i == j ? "1" : "0",
                    };
                    m1[i, j] = txtMatrixItem;
                    Controls.Add(txtMatrixItem);
                }
            }

            m2 = new TextBox[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    TextBox txtMatrixItem = new TextBox
                    {
                        Name = $"txt2_{i}_{j}",
                        Left = 270 + i * 50,
                        Top = 20 + j * 50,
                        Width = 50,
                        Text = i == j ? "1" : "0"
                    };
                    m2[i, j] = txtMatrixItem;
                    Controls.Add(txtMatrixItem);
                }
            }

            m3 = new TextBox[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    TextBox txtMatrixItem = new TextBox
                    {
                        Name = $"txt3_{i}_{j}",
                        Left = 520 + i * 50,
                        Top = 20 + j * 50,
                        Width = 50,
                        Text = ""
                    };
                    m3[i, j] = txtMatrixItem;
                    Controls.Add(txtMatrixItem);
                }
            }
        }

        private void txtMatrix_TextChanged(object sender, EventArgs e)
        {

        }

        string GetMultipleString(string a, string b)
        {
            if (a.Trim() == "0" || b.Trim() == "0")
                return "";
            if (a.Trim() == "1")
                return $"+ {b}";
            if (b.Trim() == "1")
                return $"+ {a}";
            return $"+ {a} * {b}";
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    m3[j, i].Text = "";
                    for (int k = 0; k < 4; k++)
                    {
                        m3[j, i].Text += $"{GetMultipleString(m1[k, i].Text, m2[j, k].Text)}";

                    }
                    if (m3[j, i].Text.StartsWith("+ "))
                        m3[j, i].Text = m3[j, i].Text.Substring(2);
                    if (m3[j, i].Text == "")
                        m3[j, i].Text = "0";
                }
            }

            //for(int i = 0; i < 4; i++)
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        //result[i, j].
            //    }
            //}   
        }

        private void MatrixSimplification_Load(object sender, EventArgs e)
        {

        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            m1[0, 0].Text = "CosA";
            m1[1, 0].Text = "-SinA";
            m1[2, 0].Text = "0";
            m1[3, 0].Text = "0";
            m1[0, 1].Text = "SinA";
            m1[1, 1].Text = "CosA";
            m1[2, 1].Text = "0";
            m1[3, 1].Text = "0";
            m1[0, 2].Text = "0";
            m1[1, 2].Text = "0";
            m1[2, 2].Text = "1";
            m1[3, 2].Text = "0";
            m1[0, 3].Text = "0";
            m1[1, 3].Text = "0";
            m1[2, 3].Text = "0";
            m1[3, 3].Text = "1";

            m2[0, 0].Text = "CosB";
            m2[1, 0].Text = "0";
            m2[2, 0].Text = "-SinB";
            m2[3, 0].Text = "0";
            m2[0, 1].Text = "0";
            m2[1, 1].Text = "1";
            m2[2, 1].Text = "0";
            m2[3, 1].Text = "0";
            m2[0, 2].Text = "SinB";
            m2[1, 2].Text = "0";
            m2[2, 2].Text = "CosB";
            m2[3, 2].Text = "0";
            m2[0, 3].Text = "0";
            m2[1, 3].Text = "0";
            m2[2, 3].Text = "0";
            m2[3, 3].Text = "1";
        }

        private void btnDefault2_Click(object sender, EventArgs e)
        {
            m2[0, 0].Text = "1";
            m2[1, 0].Text = "0";
            m2[2, 0].Text = "0";
            m2[3, 0].Text = "0";
            m2[0, 1].Text = "0";
            m2[1, 1].Text = "CosC";
            m2[2, 1].Text = "-SinC";
            m2[3, 1].Text = "0";
            m2[0, 2].Text = "0";
            m2[1, 2].Text = "SinC";
            m2[2, 2].Text = "CosC";
            m2[3, 2].Text = "0";
            m2[0, 3].Text = "0";
            m2[1, 3].Text = "0";
            m2[2, 3].Text = "0";
            m2[3, 3].Text = "1";
        }

        private void btnMoveLeft_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    m1[i, j].Text = m3[i, j].Text;
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    sb.AppendFormat("{0}\t", m3[j, i].Text);
                }
                sb.AppendLine();
            }       
            Clipboard.SetText(sb.ToString());
            MessageBox.Show(sb.ToString());
        }
    }
}
