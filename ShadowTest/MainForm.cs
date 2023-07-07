﻿using GraphicLibrary2;
using GraphicLibrary2.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShadowTest
{
    public partial class MainForm : Form
    {
        SharpDXEngine sde;
        public MainForm()
        {
            InitializeComponent();
            pictureBox1.Dock = DockStyle.Fill;
            sde = new SharpDXEngine();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //GC.Collect();
            lblMemory.Text = $"{sde.AdapterName} Shared Memory Use: {GetMB(sde.SharedMemoryUsage)}/{GetMB(sde.SharedSystemMemory)} Mb. Dedicated Memory Use: {GetMB(sde.DedicatedMemoryUsage)}/{GetMB(sde.DedicatedVideoMemory)} Mb";
        }
        double GetMB(long byteCount, int reservedDigits = 2)
         => Math.Round((double)byteCount / 1024 / 1024, reservedDigits);

        private void MainForm_Load(object sender, EventArgs e)
        {
            sde.SetGrahpicCardAndRenderTarget(new SharpDXInitializeSetting(
                pictureBox1.Handle, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height, true, 1));

            

            //for (int i = 0; i < 8; i++)
            //    LoadTexture(i);

            timer1.Enabled = true;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            sde.Close();
        }
    }
}
