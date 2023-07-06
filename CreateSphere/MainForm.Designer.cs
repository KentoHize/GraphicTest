namespace CreateSphere
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
            pictureBox1 = new PictureBox();
            lblDirection = new Label();
            lblDepth = new Label();
            lblResult = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(800, 450);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lblDirection
            // 
            lblDirection.AutoSize = true;
            lblDirection.Location = new Point(0, 0);
            lblDirection.Name = "lblDirection";
            lblDirection.Size = new Size(16, 19);
            lblDirection.TabIndex = 1;
            lblDirection.Text = "_";
            // 
            // lblDepth
            // 
            lblDepth.AutoSize = true;
            lblDepth.Location = new Point(86, 265);
            lblDepth.Name = "lblDepth";
            lblDepth.Size = new Size(16, 19);
            lblDepth.TabIndex = 2;
            lblDepth.Text = "_";
            // 
            // lblResult
            // 
            lblResult.AutoSize = true;
            lblResult.Location = new Point(502, 265);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(16, 19);
            lblResult.TabIndex = 3;
            lblResult.Text = "_";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblResult);
            Controls.Add(lblDepth);
            Controls.Add(lblDirection);
            Controls.Add(pictureBox1);
            Name = "MainForm";
            Text = "MainForm";
            Load += MainForm_Load;
            KeyPress += MainForm_KeyPress;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lblDirection;
        private Label lblDepth;
        private Label lblResult;
    }
}