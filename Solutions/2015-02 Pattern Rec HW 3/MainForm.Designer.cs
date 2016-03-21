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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.dataTrainer1 = new SamSeifert.ML.Controls.Trainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.transformer1 = new SamSeifert.ML.Controls.Transformer();
            this.preprocess1 = new SamSeifert.ML.Controls.Preprocess();
            this.valueNormalizer1 = new SamSeifert.ML.Controls.ValueNormalizer();
            this.trainingDataLabelNormalizer1 = new SamSeifert.ML.Controls.TrainingDataLabelNormalizer();
            this.splitter1 = new SamSeifert.ML.Controls.Splitter();
            this.panel9 = new System.Windows.Forms.Panel();
            this.labeler1 = new SamSeifert.ML.Controls.Labeler();
            this.dataLoader1 = new SamSeifert.ML.Controls.LoadCSV();
            this.bResetText = new System.Windows.Forms.Button();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 720);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(554, 25);
            this.panel2.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 626);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(554, 25);
            this.panel3.TabIndex = 5;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 532);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(554, 25);
            this.panel4.TabIndex = 7;
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 438);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(554, 25);
            this.panel5.TabIndex = 9;
            // 
            // panel6
            // 
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 94);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(554, 25);
            this.panel6.TabIndex = 12;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.textBox1.Location = new System.Drawing.Point(591, 10);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(400, 1044);
            this.textBox1.TabIndex = 13;
            // 
            // panel7
            // 
            this.panel7.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel7.Location = new System.Drawing.Point(581, 10);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(10, 1044);
            this.panel7.TabIndex = 14;
            // 
            // panel8
            // 
            this.panel8.AutoScroll = true;
            this.panel8.Controls.Add(this.dataTrainer1);
            this.panel8.Controls.Add(this.panel1);
            this.panel8.Controls.Add(this.transformer1);
            this.panel8.Controls.Add(this.panel2);
            this.panel8.Controls.Add(this.preprocess1);
            this.panel8.Controls.Add(this.panel3);
            this.panel8.Controls.Add(this.valueNormalizer1);
            this.panel8.Controls.Add(this.panel4);
            this.panel8.Controls.Add(this.trainingDataLabelNormalizer1);
            this.panel8.Controls.Add(this.panel5);
            this.panel8.Controls.Add(this.splitter1);
            this.panel8.Controls.Add(this.panel9);
            this.panel8.Controls.Add(this.labeler1);
            this.panel8.Controls.Add(this.panel6);
            this.panel8.Controls.Add(this.dataLoader1);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(10, 10);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(571, 1044);
            this.panel8.TabIndex = 15;
            // 
            // dataTrainer1
            // 
            this.dataTrainer1._K = 5;
            this.dataTrainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataTrainer1.Location = new System.Drawing.Point(0, 839);
            this.dataTrainer1.Name = "dataTrainer1";
            this.dataTrainer1.Size = new System.Drawing.Size(554, 219);
            this.dataTrainer1.TabIndex = 11;
            this.dataTrainer1.DataPop += new SamSeifert.ML.Controls.Trainer.DataPopHandler(this.dataTrainer1_DataPop);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 814);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(554, 25);
            this.panel1.TabIndex = 16;
            // 
            // transformer1
            // 
            this.transformer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.transformer1.Location = new System.Drawing.Point(0, 745);
            this.transformer1.Name = "transformer1";
            this.transformer1.Size = new System.Drawing.Size(554, 69);
            this.transformer1.TabIndex = 17;
            this.transformer1.DataPop += new SamSeifert.ML.Controls.DataPopHandler(this.transformer1_DataPop);
            // 
            // preprocess1
            // 
            this.preprocess1.Dock = System.Windows.Forms.DockStyle.Top;
            this.preprocess1.Location = new System.Drawing.Point(0, 651);
            this.preprocess1.Name = "preprocess1";
            this.preprocess1.Size = new System.Drawing.Size(554, 69);
            this.preprocess1.TabIndex = 15;
            this.preprocess1.DataPop += new SamSeifert.ML.Controls.Preprocess.DataPopHandler(this.preprocess1_DataPop);
            // 
            // valueNormalizer1
            // 
            this.valueNormalizer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.valueNormalizer1.Location = new System.Drawing.Point(0, 557);
            this.valueNormalizer1.Name = "valueNormalizer1";
            this.valueNormalizer1.Size = new System.Drawing.Size(554, 69);
            this.valueNormalizer1.TabIndex = 0;
            this.valueNormalizer1.DataPop += new SamSeifert.ML.Controls.DataPopHandler(this.valueNormalizer1_DataPop);
            // 
            // trainingDataLabelNormalizer1
            // 
            this.trainingDataLabelNormalizer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.trainingDataLabelNormalizer1.Location = new System.Drawing.Point(0, 463);
            this.trainingDataLabelNormalizer1.Name = "trainingDataLabelNormalizer1";
            this.trainingDataLabelNormalizer1.Size = new System.Drawing.Size(554, 69);
            this.trainingDataLabelNormalizer1.TabIndex = 13;
            this.trainingDataLabelNormalizer1.DataPop += new SamSeifert.ML.Controls.DataPopHandler(this.trainingDataLabelNormalizer1_DataPop);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Enabled = false;
            this.splitter1.Location = new System.Drawing.Point(0, 344);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(554, 94);
            this.splitter1.TabIndex = 1;
            this.splitter1.DataPop += new SamSeifert.ML.Controls.DataPopHandler(this.splitter1_DataPop);
            // 
            // panel9
            // 
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 319);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(554, 25);
            this.panel9.TabIndex = 14;
            // 
            // labeler1
            // 
            this.labeler1.Dock = System.Windows.Forms.DockStyle.Top;
            this.labeler1.Location = new System.Drawing.Point(0, 119);
            this.labeler1.Name = "labeler1";
            this.labeler1.Size = new System.Drawing.Size(554, 200);
            this.labeler1.TabIndex = 4;
            this.labeler1.DataPop += new SamSeifert.ML.Controls.DataPopHandler(this.labeler1_DataPop);
            // 
            // dataLoader1
            // 
            this.dataLoader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataLoader1.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dataLoader1.Location = new System.Drawing.Point(0, 0);
            this.dataLoader1.Name = "dataLoader1";
            this.dataLoader1.Size = new System.Drawing.Size(554, 94);
            this.dataLoader1.TabIndex = 0;
            this.dataLoader1.DataPop += new SamSeifert.ML.Controls.LoadCSV.DataPopHandler(this.LoaderDataPop);
            // 
            // bResetText
            // 
            this.bResetText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bResetText.Location = new System.Drawing.Point(897, 13);
            this.bResetText.Name = "bResetText";
            this.bResetText.Size = new System.Drawing.Size(75, 23);
            this.bResetText.TabIndex = 16;
            this.bResetText.Text = "Clear Text";
            this.bResetText.UseVisualStyleBackColor = true;
            this.bResetText.Click += new System.EventHandler(this.bResetText_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1001, 1064);
            this.Controls.Add(this.bResetText);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.textBox1);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.panel8.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SamSeifert.ML.Controls.LoadCSV dataLoader1;
        private SamSeifert.ML.Controls.Splitter splitter1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel2;
        private SamSeifert.ML.Controls.Labeler labeler1;
        private SamSeifert.ML.Controls.ValueNormalizer valueNormalizer1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private SamSeifert.ML.Controls.Trainer dataTrainer1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private SamSeifert.ML.Controls.TrainingDataLabelNormalizer trainingDataLabelNormalizer1;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Button bResetText;
        private System.Windows.Forms.Panel panel1;
        private SamSeifert.ML.Controls.Transformer transformer1;
        private SamSeifert.ML.Controls.Preprocess preprocess1;
    }
}