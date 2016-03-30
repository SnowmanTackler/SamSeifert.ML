namespace solution
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
            this.components = new System.ComponentModel.Container();
            this.timerStartup = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lFileName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.bFindNearest = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.lGroup = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudCountourSections = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.bRandom = new System.Windows.Forms.Button();
            this.bPlayback = new System.Windows.Forms.Button();
            this.pSelect = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pDrawTrailScaledFiltered = new solution.PanelOverlay();
            this.pDrawTrailScaled = new solution.PanelOverlay();
            this.pDrawTrail = new solution.PanelOverlay();
            this.pDrawMain = new solution.PanelOverlay();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timerDraw = new System.Windows.Forms.Timer(this.components);
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCountourSections)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerStartup
            // 
            this.timerStartup.Enabled = true;
            this.timerStartup.Interval = 500;
            this.timerStartup.Tick += new System.EventHandler(this.timerStartup_Tick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(360, 935);
            this.panel2.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.lFileName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.bFindNearest, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.bSave, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.lGroup, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.nudCountourSections, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.bRandom, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.bPlayback, 0, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(360, 935);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lFileName
            // 
            this.lFileName.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lFileName, 2);
            this.lFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFileName.Location = new System.Drawing.Point(123, 25);
            this.lFileName.Name = "lFileName";
            this.lFileName.Size = new System.Drawing.Size(234, 25);
            this.lFileName.TabIndex = 12;
            this.lFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 25);
            this.label4.TabIndex = 11;
            this.label4.Text = "File Name:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bFindNearest
            // 
            this.bFindNearest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bFindNearest.Location = new System.Drawing.Point(122, 227);
            this.bFindNearest.Margin = new System.Windows.Forms.Padding(2);
            this.bFindNearest.Name = "bFindNearest";
            this.bFindNearest.Size = new System.Drawing.Size(116, 21);
            this.bFindNearest.TabIndex = 10;
            this.bFindNearest.Text = "Find Nearest";
            this.bFindNearest.UseVisualStyleBackColor = true;
            this.bFindNearest.Click += new System.EventHandler(this.buttonFindNearest_Click);
            // 
            // bSave
            // 
            this.bSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bSave.Location = new System.Drawing.Point(242, 252);
            this.bSave.Margin = new System.Windows.Forms.Padding(2);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(116, 21);
            this.bSave.TabIndex = 8;
            this.bSave.Text = "Save To Database";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // lGroup
            // 
            this.lGroup.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lGroup, 2);
            this.lGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lGroup.Location = new System.Drawing.Point(123, 0);
            this.lGroup.Name = "lGroup";
            this.lGroup.Size = new System.Drawing.Size(234, 25);
            this.lGroup.TabIndex = 6;
            this.lGroup.Text = "airplane";
            this.lGroup.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 25);
            this.label3.TabIndex = 5;
            this.label3.Text = "Data Group:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Data Location:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudCountourSections
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.nudCountourSections, 2);
            this.nudCountourSections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudCountourSections.Location = new System.Drawing.Point(123, 153);
            this.nudCountourSections.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCountourSections.Name = "nudCountourSections";
            this.nudCountourSections.Size = new System.Drawing.Size(234, 20);
            this.nudCountourSections.TabIndex = 0;
            this.nudCountourSections.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudCountourSections.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudCountourSections.ValueChanged += new System.EventHandler(this.nudCountourSections_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Contour Sections:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox1, 2);
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(123, 53);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(234, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.DoubleClick += new System.EventHandler(this.textBox1_DoubleClick);
            // 
            // bRandom
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.bRandom, 2);
            this.bRandom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bRandom.Location = new System.Drawing.Point(122, 77);
            this.bRandom.Margin = new System.Windows.Forms.Padding(2);
            this.bRandom.Name = "bRandom";
            this.bRandom.Size = new System.Drawing.Size(236, 21);
            this.bRandom.TabIndex = 4;
            this.bRandom.Text = "Pick Random";
            this.bRandom.UseVisualStyleBackColor = true;
            this.bRandom.Click += new System.EventHandler(this.bRandom_Click);
            // 
            // bPlayback
            // 
            this.bPlayback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bPlayback.Location = new System.Drawing.Point(2, 227);
            this.bPlayback.Margin = new System.Windows.Forms.Padding(2);
            this.bPlayback.Name = "bPlayback";
            this.bPlayback.Size = new System.Drawing.Size(116, 21);
            this.bPlayback.TabIndex = 7;
            this.bPlayback.Text = "Playback";
            this.bPlayback.UseVisualStyleBackColor = true;
            this.bPlayback.Click += new System.EventHandler(this.bPlayback_Click);
            // 
            // pSelect
            // 
            this.pSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pSelect.AutoScroll = true;
            this.pSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.pSelect.Location = new System.Drawing.Point(10, 472);
            this.pSelect.Name = "pSelect";
            this.pSelect.Size = new System.Drawing.Size(619, 451);
            this.pSelect.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.panel1.Controls.Add(this.pDrawTrailScaledFiltered);
            this.panel1.Controls.Add(this.pDrawTrailScaled);
            this.panel1.Controls.Add(this.pSelect);
            this.panel1.Controls.Add(this.pDrawTrail);
            this.panel1.Controls.Add(this.pDrawMain);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(360, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(641, 935);
            this.panel1.TabIndex = 2;
            // 
            // pDrawTrailScaledFiltered
            // 
            this.pDrawTrailScaledFiltered.Location = new System.Drawing.Point(66, 416);
            this.pDrawTrailScaledFiltered.Name = "pDrawTrailScaledFiltered";
            this.pDrawTrailScaledFiltered.Size = new System.Drawing.Size(50, 50);
            this.pDrawTrailScaledFiltered.TabIndex = 5;
            this.pDrawTrailScaledFiltered.Paint += new System.Windows.Forms.PaintEventHandler(this.pDrawTrailScaledFiltered_Paint);
            // 
            // pDrawTrailScaled
            // 
            this.pDrawTrailScaled.Location = new System.Drawing.Point(10, 416);
            this.pDrawTrailScaled.Name = "pDrawTrailScaled";
            this.pDrawTrailScaled.Size = new System.Drawing.Size(50, 50);
            this.pDrawTrailScaled.TabIndex = 4;
            this.pDrawTrailScaled.Paint += new System.Windows.Forms.PaintEventHandler(this.pDrawTrailScaled_Paint);
            // 
            // pDrawTrail
            // 
            this.pDrawTrail.Location = new System.Drawing.Point(416, 10);
            this.pDrawTrail.Name = "pDrawTrail";
            this.pDrawTrail.Size = new System.Drawing.Size(400, 400);
            this.pDrawTrail.TabIndex = 3;
            this.pDrawTrail.Paint += new System.Windows.Forms.PaintEventHandler(this.pDrawTrail_Paint);
            // 
            // pDrawMain
            // 
            this.pDrawMain.BackColor = System.Drawing.Color.White;
            this.pDrawMain.Location = new System.Drawing.Point(10, 10);
            this.pDrawMain.Name = "pDrawMain";
            this.pDrawMain.Size = new System.Drawing.Size(400, 400);
            this.pDrawMain.TabIndex = 0;
            this.pDrawMain.Paint += new System.Windows.Forms.PaintEventHandler(this.pDrawMainLarge_Paint);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "SVG Files|*.svg";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // timerDraw
            // 
            this.timerDraw.Interval = 10;
            this.timerDraw.Tick += new System.EventHandler(this.timerDraw_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 935);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCountourSections)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerStartup;
        private PanelOverlay pDrawMain;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.NumericUpDown nudCountourSections;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bRandom;
        private System.Windows.Forms.Label lGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bPlayback;
        private System.Windows.Forms.Timer timerDraw;
        private PanelOverlay pDrawTrail;
        private PanelOverlay pDrawTrailScaled;
        private PanelOverlay pDrawTrailScaledFiltered;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bFindNearest;
        private System.Windows.Forms.Panel pSelect;
        private System.Windows.Forms.Label lFileName;
        private System.Windows.Forms.Label label4;
    }
}