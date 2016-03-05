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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.dataTrainer1 = new SamSeifert.ML.Controls.Trainer();
            this.dataTransformer1 = new SamSeifert.ML.Controls.Transformer();
            this.dataPreProcess1 = new SamSeifert.ML.Controls.Preprocess();
            this.dataNormalizer1 = new SamSeifert.ML.Controls.ValueNormalizer();
            this.dataDistributionNormalizer1 = new SamSeifert.ML.Controls.LabelNormalizer();
            this.dataLabeler1 = new SamSeifert.ML.Controls.UseableDataFromCSV();
            this.dataSplitter1 = new SamSeifert.ML.Controls.Splitter();
            this.panel9 = new System.Windows.Forms.Panel();
            this.dataLoader1 = new SamSeifert.ML.Controls.LoadCSV();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 720);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(554, 25);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 814);
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
            this.panel6.Location = new System.Drawing.Point(0, 213);
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
            this.textBox1.Size = new System.Drawing.Size(400, 741);
            this.textBox1.TabIndex = 13;
            // 
            // panel7
            // 
            this.panel7.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel7.Location = new System.Drawing.Point(581, 10);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(10, 741);
            this.panel7.TabIndex = 14;
            // 
            // panel8
            // 
            this.panel8.AutoScroll = true;
            this.panel8.Controls.Add(this.dataTrainer1);
            this.panel8.Controls.Add(this.panel2);
            this.panel8.Controls.Add(this.dataTransformer1);
            this.panel8.Controls.Add(this.panel1);
            this.panel8.Controls.Add(this.dataPreProcess1);
            this.panel8.Controls.Add(this.panel3);
            this.panel8.Controls.Add(this.dataNormalizer1);
            this.panel8.Controls.Add(this.panel4);
            this.panel8.Controls.Add(this.dataDistributionNormalizer1);
            this.panel8.Controls.Add(this.panel5);
            this.panel8.Controls.Add(this.dataLabeler1);
            this.panel8.Controls.Add(this.panel6);
            this.panel8.Controls.Add(this.dataSplitter1);
            this.panel8.Controls.Add(this.panel9);
            this.panel8.Controls.Add(this.dataLoader1);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(10, 10);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(571, 741);
            this.panel8.TabIndex = 15;
            // 
            // dataTrainer1
            // 
            this.dataTrainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataTrainer1.Location = new System.Drawing.Point(0, 839);
            this.dataTrainer1.Name = "dataTrainer1";
            this.dataTrainer1.Size = new System.Drawing.Size(554, 194);
            this.dataTrainer1.TabIndex = 11;
            // 
            // dataTransformer1
            // 
            this.dataTransformer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataTransformer1.Location = new System.Drawing.Point(0, 745);
            this.dataTransformer1.Name = "dataTransformer1";
            this.dataTransformer1.Size = new System.Drawing.Size(554, 69);
            this.dataTransformer1.TabIndex = 10;
            this.dataTransformer1.DataPop += new SamSeifert.ML.Controls.Transformer.DataPopHandler(this.dataTransformer1_DataPop);
            // 
            // dataPreProcess1
            // 
            this.dataPreProcess1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataPreProcess1.Location = new System.Drawing.Point(0, 651);
            this.dataPreProcess1.Name = "dataPreProcess1";
            this.dataPreProcess1.Size = new System.Drawing.Size(554, 69);
            this.dataPreProcess1.TabIndex = 8;
            this.dataPreProcess1.DataPop += new SamSeifert.ML.Controls.Preprocess.DataPopHandler(this.dataPreProcess1_DataPop);
            // 
            // dataNormalizer1
            // 
            this.dataNormalizer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataNormalizer1.Location = new System.Drawing.Point(0, 557);
            this.dataNormalizer1.Name = "dataNormalizer1";
            this.dataNormalizer1.Size = new System.Drawing.Size(554, 69);
            this.dataNormalizer1.TabIndex = 0;
            this.dataNormalizer1.DataPop += new SamSeifert.ML.Controls.ValueNormalizer.DataPopHandler(this.dataNormalizer1_DataPop);
            // 
            // dataDistributionNormalizer1
            // 
            this.dataDistributionNormalizer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataDistributionNormalizer1.Location = new System.Drawing.Point(0, 463);
            this.dataDistributionNormalizer1.Name = "dataDistributionNormalizer1";
            this.dataDistributionNormalizer1.Size = new System.Drawing.Size(554, 69);
            this.dataDistributionNormalizer1.TabIndex = 13;
            this.dataDistributionNormalizer1.DataPop += new SamSeifert.ML.Controls.LabelNormalizer.DataPopHandler(this.dataDistributionNormalizer1_DataPop);
            // 
            // dataLabeler1
            // 
            this.dataLabeler1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataLabeler1.Location = new System.Drawing.Point(0, 238);
            this.dataLabeler1.Name = "dataLabeler1";
            this.dataLabeler1.Size = new System.Drawing.Size(554, 200);
            this.dataLabeler1.TabIndex = 4;
            this.dataLabeler1.DataPop += new SamSeifert.ML.Controls.UseableDataFromCSV.DataPopHandler(this.dataLabeler1_DataPop);
            // 
            // dataSplitter1
            // 
            this.dataSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataSplitter1.Enabled = false;
            this.dataSplitter1.Location = new System.Drawing.Point(0, 119);
            this.dataSplitter1.Name = "dataSplitter1";
            this.dataSplitter1.Size = new System.Drawing.Size(554, 94);
            this.dataSplitter1.TabIndex = 1;
            this.dataSplitter1.DataPop += new SamSeifert.ML.Controls.Splitter.DataPopHandler(this.dataSplitter1_DataPop);
            // 
            // panel9
            // 
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 94);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(554, 25);
            this.panel9.TabIndex = 14;
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1001, 761);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.panel8.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SamSeifert.ML.Controls.LoadCSV dataLoader1;
        private SamSeifert.ML.Controls.Splitter dataSplitter1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private SamSeifert.ML.Controls.UseableDataFromCSV dataLabeler1;
        private SamSeifert.ML.Controls.ValueNormalizer dataNormalizer1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private SamSeifert.ML.Controls.Preprocess dataPreProcess1;
        private System.Windows.Forms.Panel panel5;
        private SamSeifert.ML.Controls.Transformer dataTransformer1;
        private SamSeifert.ML.Controls.Trainer dataTrainer1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private SamSeifert.ML.Controls.LabelNormalizer dataDistributionNormalizer1;
        private System.Windows.Forms.Panel panel9;
    }
}