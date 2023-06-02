namespace ConstantBuffer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pibMain = new PictureBox();
            menuStrip1 = new MenuStrip();
            測試ToolStripMenuItem = new ToolStripMenuItem();
            tsiRun = new ToolStripMenuItem();
            t1ToolStripMenuItem = new ToolStripMenuItem();
            t2ToolStripMenuItem = new ToolStripMenuItem();
            t3ToolStripMenuItem = new ToolStripMenuItem();
            tmiStop = new ToolStripMenuItem();
            timMain = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pibMain).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // pibMain
            // 
            pibMain.Dock = DockStyle.Fill;
            pibMain.Location = new Point(0, 27);
            pibMain.Name = "pibMain";
            pibMain.Size = new Size(800, 423);
            pibMain.TabIndex = 0;
            pibMain.TabStop = false;
            pibMain.Click += pibMain_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 測試ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 27);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // 測試ToolStripMenuItem
            // 
            測試ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsiRun, t1ToolStripMenuItem, t2ToolStripMenuItem, t3ToolStripMenuItem, tmiStop });
            測試ToolStripMenuItem.Name = "測試ToolStripMenuItem";
            測試ToolStripMenuItem.Size = new Size(53, 23);
            測試ToolStripMenuItem.Text = "測試";
            // 
            // tsiRun
            // 
            tsiRun.Name = "tsiRun";
            tsiRun.Size = new Size(109, 26);
            tsiRun.Text = "跑";
            tsiRun.Click += tsiRun_Click;
            // 
            // t1ToolStripMenuItem
            // 
            t1ToolStripMenuItem.Name = "t1ToolStripMenuItem";
            t1ToolStripMenuItem.Size = new Size(109, 26);
            t1ToolStripMenuItem.Text = "T1";
            t1ToolStripMenuItem.Click += t1ToolStripMenuItem_Click;
            // 
            // t2ToolStripMenuItem
            // 
            t2ToolStripMenuItem.Name = "t2ToolStripMenuItem";
            t2ToolStripMenuItem.Size = new Size(109, 26);
            t2ToolStripMenuItem.Text = "T2";
            t2ToolStripMenuItem.Click += t2ToolStripMenuItem_Click;
            // 
            // t3ToolStripMenuItem
            // 
            t3ToolStripMenuItem.Name = "t3ToolStripMenuItem";
            t3ToolStripMenuItem.Size = new Size(109, 26);
            t3ToolStripMenuItem.Text = "T3";
            t3ToolStripMenuItem.Click += t3ToolStripMenuItem_Click;
            // 
            // tmiStop
            // 
            tmiStop.Name = "tmiStop";
            tmiStop.Size = new Size(109, 26);
            tmiStop.Text = "停";
            tmiStop.Click += tmiStop_Click;
            // 
            // timMain
            // 
            timMain.Interval = 20;
            timMain.Tick += timMain_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pibMain);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "MainForm";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)pibMain).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pibMain;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 測試ToolStripMenuItem;
        private ToolStripMenuItem tsiRun;
        private ToolStripMenuItem t1ToolStripMenuItem;
        private ToolStripMenuItem t2ToolStripMenuItem;
        private ToolStripMenuItem t3ToolStripMenuItem;
        private System.Windows.Forms.Timer timMain;
        private ToolStripMenuItem tmiStop;
    }
}