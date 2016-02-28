namespace ML
{
    partial class DataTrainer
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
            this.label3 = new System.Windows.Forms.Label();
            this.nudForestTrees = new System.Windows.Forms.NumericUpDown();
            this.rbRandomForest = new System.Windows.Forms.RadioButton();
            this.labelDataStatus = new System.Windows.Forms.Label();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.rbDecisionTree = new System.Windows.Forms.RadioButton();
            this.nudDecisionTreeDepth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.bwLoadData = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudForestTrees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecisionTreeDepth)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(703, 119);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Trainer";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.nudForestTrees, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.rbRandomForest, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelDataStatus, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.rbNone, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.rbDecisionTree, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.nudDecisionTreeDepth, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(697, 100);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(128, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 25);
            this.label3.TabIndex = 12;
            this.label3.Text = "Number of Trees:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudForestTrees
            // 
            this.nudForestTrees.Location = new System.Drawing.Point(228, 28);
            this.nudForestTrees.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudForestTrees.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudForestTrees.Name = "nudForestTrees";
            this.nudForestTrees.Size = new System.Drawing.Size(75, 20);
            this.nudForestTrees.TabIndex = 10;
            this.nudForestTrees.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudForestTrees.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudForestTrees.ValueChanged += new System.EventHandler(this.NudChanged);
            // 
            // rbRandomForest
            // 
            this.rbRandomForest.AutoSize = true;
            this.rbRandomForest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbRandomForest.Location = new System.Drawing.Point(3, 28);
            this.rbRandomForest.Name = "rbRandomForest";
            this.rbRandomForest.Size = new System.Drawing.Size(119, 19);
            this.rbRandomForest.TabIndex = 7;
            this.rbRandomForest.TabStop = true;
            this.rbRandomForest.Text = "Random Forest";
            this.rbRandomForest.UseVisualStyleBackColor = true;
            this.rbRandomForest.CheckedChanged += new System.EventHandler(this.rbDecisionTree_CheckedChanged);
            // 
            // labelDataStatus
            // 
            this.labelDataStatus.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelDataStatus, 4);
            this.labelDataStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDataStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDataStatus.ForeColor = System.Drawing.Color.Red;
            this.labelDataStatus.Location = new System.Drawing.Point(3, 75);
            this.labelDataStatus.Name = "labelDataStatus";
            this.labelDataStatus.Size = new System.Drawing.Size(691, 25);
            this.labelDataStatus.TabIndex = 1;
            this.labelDataStatus.Text = "....";
            this.labelDataStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbNone.Enabled = false;
            this.rbNone.Location = new System.Drawing.Point(3, 53);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(119, 19);
            this.rbNone.TabIndex = 0;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "None";
            this.rbNone.UseVisualStyleBackColor = true;
            this.rbNone.CheckedChanged += new System.EventHandler(this.rbDecisionTree_CheckedChanged);
            // 
            // rbDecisionTree
            // 
            this.rbDecisionTree.AutoSize = true;
            this.rbDecisionTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbDecisionTree.Location = new System.Drawing.Point(3, 3);
            this.rbDecisionTree.Name = "rbDecisionTree";
            this.rbDecisionTree.Size = new System.Drawing.Size(119, 19);
            this.rbDecisionTree.TabIndex = 1;
            this.rbDecisionTree.TabStop = true;
            this.rbDecisionTree.Text = "DecisionTree";
            this.rbDecisionTree.UseVisualStyleBackColor = true;
            this.rbDecisionTree.CheckedChanged += new System.EventHandler(this.rbDecisionTree_CheckedChanged);
            // 
            // nudDecisionTreeDepth
            // 
            this.nudDecisionTreeDepth.Location = new System.Drawing.Point(228, 3);
            this.nudDecisionTreeDepth.Name = "nudDecisionTreeDepth";
            this.nudDecisionTreeDepth.Size = new System.Drawing.Size(75, 20);
            this.nudDecisionTreeDepth.TabIndex = 8;
            this.nudDecisionTreeDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudDecisionTreeDepth.ValueChanged += new System.EventHandler(this.NudChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(128, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 25);
            this.label1.TabIndex = 10;
            this.label1.Text = "Max Depth:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bwLoadData
            // 
            this.bwLoadData.WorkerSupportsCancellation = true;
            this.bwLoadData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwLoadData_DoWork);
            this.bwLoadData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwLoadData_RunWorkerCompleted);
            // 
            // DataTrainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "DataTrainer";
            this.Size = new System.Drawing.Size(703, 119);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudForestTrees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecisionTreeDepth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.ComponentModel.BackgroundWorker bwLoadData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton rbRandomForest;
        private System.Windows.Forms.Label labelDataStatus;
        private System.Windows.Forms.RadioButton rbNone;
        private System.Windows.Forms.RadioButton rbDecisionTree;
        private System.Windows.Forms.NumericUpDown nudDecisionTreeDepth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudForestTrees;
        private System.Windows.Forms.Label label3;
    }
}
