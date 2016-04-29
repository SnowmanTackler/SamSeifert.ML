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
            this.label5 = new System.Windows.Forms.Label();
            this.nudFuture = new System.Windows.Forms.NumericUpDown();
            this.lFileName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.bFindNearest = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.lGroup = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.bRandom = new System.Windows.Forms.Button();
            this.bPlayback = new System.Windows.Forms.Button();
            this.pSelect = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timerDraw = new System.Windows.Forms.Timer(this.components);
            this.bwLoad = new System.ComponentModel.BackgroundWorker();
            this.bwSave = new System.ComponentModel.BackgroundWorker();
            this.rbModePlayback = new System.Windows.Forms.RadioButton();
            this.rbModeDraw = new System.Windows.Forms.RadioButton();
            this.glControl1 = new OpenTK.GLControl();
            this.pDrawTrailScaledFiltered = new solution.PanelOverlay();
            this.pDrawTrailScaled = new solution.PanelOverlay();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFuture)).BeginInit();
            this.panelRight.SuspendLayout();
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
            this.panel2.Size = new System.Drawing.Size(224, 935);
            this.panel2.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.nudFuture, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.lFileName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.bFindNearest, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.bSave, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.lGroup, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.bRandom, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.bPlayback, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.rbModePlayback, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.rbModeDraw, 1, 9);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 11;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(224, 935);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 25);
            this.label5.TabIndex = 14;
            this.label5.Text = "Future:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudFuture
            // 
            this.nudFuture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudFuture.Location = new System.Drawing.Point(115, 153);
            this.nudFuture.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudFuture.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFuture.Name = "nudFuture";
            this.nudFuture.Size = new System.Drawing.Size(106, 20);
            this.nudFuture.TabIndex = 13;
            this.nudFuture.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudFuture.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudFuture.ValueChanged += new System.EventHandler(this.nudFuture_ValueChanged);
            // 
            // lFileName
            // 
            this.lFileName.AutoSize = true;
            this.lFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFileName.Location = new System.Drawing.Point(115, 25);
            this.lFileName.Name = "lFileName";
            this.lFileName.Size = new System.Drawing.Size(106, 25);
            this.lFileName.TabIndex = 12;
            this.lFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 25);
            this.label4.TabIndex = 11;
            this.label4.Text = "File Name:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bFindNearest
            // 
            this.bFindNearest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bFindNearest.Location = new System.Drawing.Point(114, 177);
            this.bFindNearest.Margin = new System.Windows.Forms.Padding(2);
            this.bFindNearest.Name = "bFindNearest";
            this.bFindNearest.Size = new System.Drawing.Size(108, 21);
            this.bFindNearest.TabIndex = 10;
            this.bFindNearest.Text = "Find Nearest";
            this.bFindNearest.UseVisualStyleBackColor = true;
            this.bFindNearest.Click += new System.EventHandler(this.buttonFindNearest_Click);
            // 
            // bSave
            // 
            this.bSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bSave.Location = new System.Drawing.Point(114, 202);
            this.bSave.Margin = new System.Windows.Forms.Padding(2);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(108, 21);
            this.bSave.TabIndex = 8;
            this.bSave.Text = "Save To Database";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // lGroup
            // 
            this.lGroup.AutoSize = true;
            this.lGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lGroup.Location = new System.Drawing.Point(115, 0);
            this.lGroup.Name = "lGroup";
            this.lGroup.Size = new System.Drawing.Size(106, 25);
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
            this.label3.Size = new System.Drawing.Size(106, 25);
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
            this.label2.Size = new System.Drawing.Size(106, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Data Location:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox1, 2);
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 78);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(218, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.DoubleClick += new System.EventHandler(this.textBox1_DoubleClick);
            // 
            // bRandom
            // 
            this.bRandom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bRandom.Location = new System.Drawing.Point(114, 52);
            this.bRandom.Margin = new System.Windows.Forms.Padding(2);
            this.bRandom.Name = "bRandom";
            this.bRandom.Size = new System.Drawing.Size(108, 21);
            this.bRandom.TabIndex = 4;
            this.bRandom.Text = "Pick Random";
            this.bRandom.UseVisualStyleBackColor = true;
            this.bRandom.Click += new System.EventHandler(this.bRandom_Click);
            // 
            // bPlayback
            // 
            this.bPlayback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bPlayback.Location = new System.Drawing.Point(2, 177);
            this.bPlayback.Margin = new System.Windows.Forms.Padding(2);
            this.bPlayback.Name = "bPlayback";
            this.bPlayback.Size = new System.Drawing.Size(108, 21);
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
            this.pSelect.Size = new System.Drawing.Size(755, 451);
            this.pSelect.TabIndex = 6;
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.panelRight.Controls.Add(this.glControl1);
            this.panelRight.Controls.Add(this.pDrawTrailScaledFiltered);
            this.panelRight.Controls.Add(this.pDrawTrailScaled);
            this.panelRight.Controls.Add(this.pSelect);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(224, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(777, 935);
            this.panelRight.TabIndex = 2;
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
            // bwLoad
            // 
            this.bwLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwLoad_Complete);
            // 
            // rbModePlayback
            // 
            this.rbModePlayback.AutoSize = true;
            this.rbModePlayback.Checked = true;
            this.rbModePlayback.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbModePlayback.Location = new System.Drawing.Point(3, 278);
            this.rbModePlayback.Name = "rbModePlayback";
            this.rbModePlayback.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.rbModePlayback.Size = new System.Drawing.Size(106, 19);
            this.rbModePlayback.TabIndex = 15;
            this.rbModePlayback.TabStop = true;
            this.rbModePlayback.Text = "Playback";
            this.rbModePlayback.UseVisualStyleBackColor = true;
            this.rbModePlayback.CheckedChanged += new System.EventHandler(this.rbModeDraw_CheckedChanged);
            // 
            // rbModeDraw
            // 
            this.rbModeDraw.AutoSize = true;
            this.rbModeDraw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbModeDraw.Location = new System.Drawing.Point(115, 278);
            this.rbModeDraw.Name = "rbModeDraw";
            this.rbModeDraw.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.rbModeDraw.Size = new System.Drawing.Size(106, 19);
            this.rbModeDraw.TabIndex = 16;
            this.rbModeDraw.Text = "Draw";
            this.rbModeDraw.UseVisualStyleBackColor = true;
            this.rbModeDraw.CheckedChanged += new System.EventHandler(this.rbModeDraw_CheckedChanged);
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.glControl1.Location = new System.Drawing.Point(0, 0);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(777, 402);
            this.glControl1.TabIndex = 7;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 935);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panel2);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFuture)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerStartup;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bRandom;
        private System.Windows.Forms.Label lGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bPlayback;
        private System.Windows.Forms.Timer timerDraw;
        private PanelOverlay pDrawTrailScaled;
        private PanelOverlay pDrawTrailScaledFiltered;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bFindNearest;
        private System.Windows.Forms.Panel pSelect;
        private System.Windows.Forms.Label lFileName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudFuture;
        private System.ComponentModel.BackgroundWorker bwLoad;
        private OpenTK.GLControl glControl1;
        private System.ComponentModel.BackgroundWorker bwSave;
        private System.Windows.Forms.RadioButton rbModePlayback;
        private System.Windows.Forms.RadioButton rbModeDraw;
    }
}