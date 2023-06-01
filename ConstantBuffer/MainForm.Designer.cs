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
            pibMain = new PictureBox();
            menuStrip1 = new MenuStrip();
            測試ToolStripMenuItem = new ToolStripMenuItem();
            tsiRun = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)pibMain).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // pibMain
            // 
            pibMain.Dock = DockStyle.Fill;
            pibMain.Location = new Point(0, 28);
            pibMain.Name = "pibMain";
            pibMain.Size = new Size(800, 422);
            pibMain.TabIndex = 0;
            pibMain.TabStop = false;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { 測試ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // 測試ToolStripMenuItem
            // 
            測試ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsiRun });
            測試ToolStripMenuItem.Name = "測試ToolStripMenuItem";
            測試ToolStripMenuItem.Size = new Size(53, 24);
            測試ToolStripMenuItem.Text = "測試";
            // 
            // tsiRun
            // 
            tsiRun.Name = "tsiRun";
            tsiRun.Size = new Size(224, 26);
            tsiRun.Text = "跑";
            tsiRun.Click += tsiRun_Click;
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
    }
}