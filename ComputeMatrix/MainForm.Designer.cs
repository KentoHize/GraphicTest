namespace ComputeMatrix
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
            ((System.ComponentModel.ISupportInitialize)pibMain).BeginInit();
            SuspendLayout();
            // 
            // pibMain
            // 
            pibMain.Dock = DockStyle.Fill;
            pibMain.Location = new Point(0, 0);
            pibMain.Name = "pibMain";
            pibMain.Size = new Size(800, 450);
            pibMain.TabIndex = 1;
            pibMain.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pibMain);
            Name = "MainForm";
            Text = "ComputeMatrix";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)pibMain).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pibMain;
    }
}