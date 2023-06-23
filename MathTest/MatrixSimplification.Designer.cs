namespace MathTest
{
    partial class MatrixSimplification
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
            btnInput = new Button();
            btnCaculate = new Button();
            SuspendLayout();
            // 
            // btnInput
            // 
            btnInput.Location = new Point(49, 370);
            btnInput.Name = "btnInput";
            btnInput.Size = new Size(97, 41);
            btnInput.TabIndex = 0;
            btnInput.Text = "輸入";
            btnInput.UseVisualStyleBackColor = true;
            btnInput.Click += btnInput_Click;
            // 
            // btnCaculate
            // 
            btnCaculate.Location = new Point(197, 370);
            btnCaculate.Name = "btnCaculate";
            btnCaculate.Size = new Size(97, 41);
            btnCaculate.TabIndex = 1;
            btnCaculate.Text = "計算";
            btnCaculate.UseVisualStyleBackColor = true;
            // 
            // MatrixSimplification
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1293, 464);
            Controls.Add(btnCaculate);
            Controls.Add(btnInput);
            Name = "MatrixSimplification";
            Text = "MatrixSimplification";
            ResumeLayout(false);
        }

        #endregion

        private Button btnInput;
        private Button btnCaculate;
    }
}