namespace Texture
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
            lblDebug = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pibMain).BeginInit();
            SuspendLayout();
            // 
            // pibMain
            // 
            pibMain.Dock = DockStyle.Fill;
            pibMain.Location = new Point(0, 0);
            pibMain.Name = "pibMain";
            pibMain.Size = new Size(800, 450);
            pibMain.TabIndex = 0;
            pibMain.TabStop = false;
            pibMain.Click += pibMain_Click;
            // 
            // lblDebug
            // 
            lblDebug.Location = new Point(-2, -1);
            lblDebug.Name = "lblDebug";
            lblDebug.Size = new Size(658, 300);
            lblDebug.TabIndex = 1;
            lblDebug.Text = "_";
            // 
            // timer1
            // 
            timer1.Interval = 33;
            timer1.Tick += timer1_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblDebug);
            Controls.Add(pibMain);
            Name = "MainForm";
            Text = "Form1";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)pibMain).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pibMain;
        private Label lblDebug;
        private System.Windows.Forms.Timer timer1;
    }
}