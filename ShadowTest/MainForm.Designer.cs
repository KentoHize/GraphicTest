namespace ShadowTest
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureBox1 = new PictureBox();
            lblMemory = new Label();
            lblR2 = new Label();
            lblL2 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(294, 70);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(246, 217);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lblMemory
            // 
            lblMemory.AutoSize = true;
            lblMemory.Location = new Point(0, 0);
            lblMemory.Name = "lblMemory";
            lblMemory.Size = new Size(16, 19);
            lblMemory.TabIndex = 1;
            lblMemory.Text = "_";
            // 
            // lblR2
            // 
            lblR2.AutoSize = true;
            lblR2.Location = new Point(737, 422);
            lblR2.Name = "lblR2";
            lblR2.Size = new Size(16, 19);
            lblR2.TabIndex = 2;
            lblR2.Text = "_";
            // 
            // lblL2
            // 
            lblL2.AutoSize = true;
            lblL2.Location = new Point(0, 422);
            lblL2.Name = "lblL2";
            lblL2.Size = new Size(16, 19);
            lblL2.TabIndex = 3;
            lblL2.Text = "_";
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
            Controls.Add(lblL2);
            Controls.Add(lblR2);
            Controls.Add(lblMemory);
            Controls.Add(pictureBox1);
            Name = "MainForm";
            Text = "MainForm";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lblMemory;
        private Label lblR2;
        private Label lblL2;
        private System.Windows.Forms.Timer timer1;
    }
}