namespace GraphicTest
{
    partial class Form1
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
            pibMain.Size = new Size(1364, 705);
            pibMain.TabIndex = 0;
            pibMain.TabStop = false;
            pibMain.Paint += pictureBox1_Paint;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1364, 705);
            Controls.Add(pibMain);
            DoubleBuffered = true;
            Name = "Form1";
            Text = "Form1";
            KeyPress += Form1_KeyPress;
            ((System.ComponentModel.ISupportInitialize)pibMain).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pibMain;
    }
}