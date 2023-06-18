namespace MathTest
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
            pnlMain = new Panel();
            txtPointP = new TextBox();
            lblPointP = new Label();
            btnRefresh = new Button();
            txtPointC = new TextBox();
            txtPointB = new TextBox();
            txtPointA = new TextBox();
            lblPointC = new Label();
            lblPointB = new Label();
            lblPointA = new Label();
            erpMain = new ErrorProvider(components);
            pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)erpMain).BeginInit();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.BackColor = SystemColors.ControlDark;
            pnlMain.Controls.Add(txtPointP);
            pnlMain.Controls.Add(lblPointP);
            pnlMain.Controls.Add(btnRefresh);
            pnlMain.Controls.Add(txtPointC);
            pnlMain.Controls.Add(txtPointB);
            pnlMain.Controls.Add(txtPointA);
            pnlMain.Controls.Add(lblPointC);
            pnlMain.Controls.Add(lblPointB);
            pnlMain.Controls.Add(lblPointA);
            pnlMain.Location = new Point(12, 12);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(268, 166);
            pnlMain.TabIndex = 0;
            // 
            // txtPointP
            // 
            txtPointP.Location = new Point(90, 98);
            txtPointP.Name = "txtPointP";
            txtPointP.Size = new Size(112, 27);
            txtPointP.TabIndex = 7;
            txtPointP.Text = "100, 100";
            txtPointP.Validated += txtPoint_Validated;
            // 
            // lblPointP
            // 
            lblPointP.AutoSize = true;
            lblPointP.Location = new Point(15, 101);
            lblPointP.Name = "lblPointP";
            lblPointP.Size = new Size(54, 19);
            lblPointP.TabIndex = 6;
            lblPointP.Text = "PointP";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(90, 131);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(69, 27);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Draw";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // txtPointC
            // 
            txtPointC.Location = new Point(90, 69);
            txtPointC.Name = "txtPointC";
            txtPointC.Size = new Size(112, 27);
            txtPointC.TabIndex = 5;
            txtPointC.Text = "200, 150";
            txtPointC.TextChanged += txtPoint_Validated;
            // 
            // txtPointB
            // 
            txtPointB.Location = new Point(90, 40);
            txtPointB.Name = "txtPointB";
            txtPointB.Size = new Size(112, 27);
            txtPointB.TabIndex = 4;
            txtPointB.Text = "80, 200";
            txtPointB.TextChanged += txtPoint_Validated;
            // 
            // txtPointA
            // 
            txtPointA.Location = new Point(90, 10);
            txtPointA.Name = "txtPointA";
            txtPointA.Size = new Size(112, 27);
            txtPointA.TabIndex = 1;
            txtPointA.Text = "20, 30";
            txtPointA.Validated += txtPoint_Validated;
            // 
            // lblPointC
            // 
            lblPointC.AutoSize = true;
            lblPointC.Location = new Point(15, 72);
            lblPointC.Name = "lblPointC";
            lblPointC.Size = new Size(55, 19);
            lblPointC.TabIndex = 3;
            lblPointC.Text = "PointC";
            // 
            // lblPointB
            // 
            lblPointB.AutoSize = true;
            lblPointB.Location = new Point(15, 43);
            lblPointB.Name = "lblPointB";
            lblPointB.Size = new Size(54, 19);
            lblPointB.TabIndex = 2;
            lblPointB.Text = "PointB";
            // 
            // lblPointA
            // 
            lblPointA.AutoSize = true;
            lblPointA.Location = new Point(15, 13);
            lblPointA.Name = "lblPointA";
            lblPointA.Size = new Size(55, 19);
            lblPointA.TabIndex = 1;
            lblPointA.Text = "PointA";
            // 
            // erpMain
            // 
            erpMain.ContainerControl = this;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(979, 543);
            Controls.Add(pnlMain);
            Name = "MainForm";
            Text = "Form1";
            Load += MainForm_Load;
            Paint += MainForm_Paint;
            Resize += MainForm_Resize;
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)erpMain).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMain;
        private Label lblPointC;
        private Label lblPointB;
        private Label lblPointA;
        private TextBox txtPointA;
        private TextBox txtPointC;
        private TextBox txtPointB;
        private Button btnRefresh;
        private ErrorProvider erpMain;
        private TextBox txtPointP;
        private Label lblPointP;
    }
}