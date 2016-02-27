﻿namespace ML
{
    partial class Form1
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
            this.dataLabeler1 = new ML.DataLabeler();
            this.dataSplitter1 = new ML.DataSplitter();
            this.dataLoader1 = new ML.DataLoader();
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
            this.panel1.Location = new System.Drawing.Point(10, 106);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(667, 25);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(10, 201);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(667, 25);
            this.panel2.TabIndex = 3;
            // 
            // dataLabeler1
            // 
            this.dataLabeler1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataLabeler1.Location = new System.Drawing.Point(10, 226);
            this.dataLabeler1.Name = "dataLabeler1";
            this.dataLabeler1.Size = new System.Drawing.Size(667, 175);
            this.dataLabeler1.TabIndex = 4;
            // 
            // dataSplitter1
            // 
            this.dataSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataSplitter1.Enabled = false;
            this.dataSplitter1.Location = new System.Drawing.Point(10, 131);
            this.dataSplitter1.Name = "dataSplitter1";
            this.dataSplitter1.Size = new System.Drawing.Size(667, 70);
            this.dataSplitter1.TabIndex = 1;
            this.dataSplitter1.DataPop += new ML.DataSplitter.DataPopHandler(this.dataSplitter1_DataPop);
            // 
            // dataLoader1
            // 
            this.dataLoader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataLoader1.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dataLoader1.Location = new System.Drawing.Point(10, 10);
            this.dataLoader1.Name = "dataLoader1";
            this.dataLoader1.Size = new System.Drawing.Size(667, 96);
            this.dataLoader1.TabIndex = 0;
            this.dataLoader1.DataPop += new ML.DataLoader.DataPopHandler(this.LoaderDataPop);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(687, 624);
            this.Controls.Add(this.dataLabeler1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dataSplitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataLoader1);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.ResumeLayout(false);

        }

        #endregion

        private DataLoader dataLoader1;
        private DataSplitter dataSplitter1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private DataLabeler dataLabeler1;
    }
}