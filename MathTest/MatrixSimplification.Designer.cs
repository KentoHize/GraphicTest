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
            btnDefault = new Button();
            btnDefault2 = new Button();
            btnMoveLeft = new Button();
            btnOutput = new Button();
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
            // btnDefault
            // 
            btnDefault.Location = new Point(328, 370);
            btnDefault.Name = "btnDefault";
            btnDefault.Size = new Size(97, 41);
            btnDefault.TabIndex = 2;
            btnDefault.Text = "預設";
            btnDefault.UseVisualStyleBackColor = true;
            btnDefault.Click += btnDefault_Click;
            // 
            // btnDefault2
            // 
            btnDefault2.Location = new Point(456, 370);
            btnDefault2.Name = "btnDefault2";
            btnDefault2.Size = new Size(97, 41);
            btnDefault2.TabIndex = 3;
            btnDefault2.Text = "預設2";
            btnDefault2.UseVisualStyleBackColor = true;
            btnDefault2.Click += btnDefault2_Click;
            // 
            // btnMoveLeft
            // 
            btnMoveLeft.Location = new Point(588, 370);
            btnMoveLeft.Name = "btnMoveLeft";
            btnMoveLeft.Size = new Size(97, 41);
            btnMoveLeft.TabIndex = 4;
            btnMoveLeft.Text = "複製到左邊";
            btnMoveLeft.UseVisualStyleBackColor = true;
            btnMoveLeft.Click += btnMoveLeft_Click;
            // 
            // btnOutput
            // 
            btnOutput.Location = new Point(727, 379);
            btnOutput.Name = "btnOutput";
            btnOutput.Size = new Size(97, 41);
            btnOutput.TabIndex = 5;
            btnOutput.Text = "輸出";
            btnOutput.UseVisualStyleBackColor = true;
            btnOutput.Click += btnOutput_Click;
            // 
            // MatrixSimplification
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1293, 464);
            Controls.Add(btnOutput);
            Controls.Add(btnMoveLeft);
            Controls.Add(btnDefault2);
            Controls.Add(btnDefault);
            Controls.Add(btnCaculate);
            Controls.Add(btnInput);
            Name = "MatrixSimplification";
            Text = "MatrixSimplification";
            Load += MatrixSimplification_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnInput;
        private Button btnCaculate;
        private Button btnDefault;
        private Button btnDefault2;
        private Button btnMoveLeft;
        private Button btnOutput;
    }
}