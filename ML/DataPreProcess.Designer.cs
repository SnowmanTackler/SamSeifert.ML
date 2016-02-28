namespace ML
{
    partial class DataPreProcess
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.rbLDA = new System.Windows.Forms.RadioButton();
            this.labelDataStatus = new System.Windows.Forms.Label();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.rbPCA = new System.Windows.Forms.RadioButton();
            this.bwLoadData = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(568, 69);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preprocessing";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.Controls.Add(this.rbLDA, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelDataStatus, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.rbNone, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.rbPCA, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(562, 50);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // rbLDA
            // 
            this.rbLDA.AutoSize = true;
            this.rbLDA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbLDA.Enabled = false;
            this.rbLDA.Location = new System.Drawing.Point(78, 3);
            this.rbLDA.Name = "rbLDA";
            this.rbLDA.Size = new System.Drawing.Size(69, 19);
            this.rbLDA.TabIndex = 7;
            this.rbLDA.TabStop = true;
            this.rbLDA.Text = "LDA";
            this.rbLDA.UseVisualStyleBackColor = true;
            this.rbLDA.CheckedChanged += new System.EventHandler(this.RadioButtonChanged);
            // 
            // labelDataStatus
            // 
            this.labelDataStatus.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelDataStatus, 4);
            this.labelDataStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDataStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDataStatus.ForeColor = System.Drawing.Color.Red;
            this.labelDataStatus.Location = new System.Drawing.Point(3, 25);
            this.labelDataStatus.Name = "labelDataStatus";
            this.labelDataStatus.Size = new System.Drawing.Size(556, 25);
            this.labelDataStatus.TabIndex = 1;
            this.labelDataStatus.Text = "....";
            this.labelDataStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbNone.Location = new System.Drawing.Point(153, 3);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(69, 19);
            this.rbNone.TabIndex = 0;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "None";
            this.rbNone.UseVisualStyleBackColor = true;
            this.rbNone.CheckedChanged += new System.EventHandler(this.RadioButtonChanged);
            // 
            // rbPCA
            // 
            this.rbPCA.AutoSize = true;
            this.rbPCA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbPCA.Location = new System.Drawing.Point(3, 3);
            this.rbPCA.Name = "rbPCA";
            this.rbPCA.Size = new System.Drawing.Size(69, 19);
            this.rbPCA.TabIndex = 1;
            this.rbPCA.TabStop = true;
            this.rbPCA.Text = "PCA";
            this.rbPCA.UseVisualStyleBackColor = true;
            this.rbPCA.CheckedChanged += new System.EventHandler(this.RadioButtonChanged);
            // 
            // bwLoadData
            // 
            this.bwLoadData.WorkerSupportsCancellation = true;
            this.bwLoadData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwLoadData_DoWork);
            this.bwLoadData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwLoadData_RunWorkerCompleted);
            // 
            // DataPreProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "DataPreProcess";
            this.Size = new System.Drawing.Size(568, 69);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelDataStatus;
        private System.Windows.Forms.RadioButton rbNone;
        private System.Windows.Forms.RadioButton rbPCA;
        private System.Windows.Forms.RadioButton rbLDA;
        private System.ComponentModel.BackgroundWorker bwLoadData;
    }
}
