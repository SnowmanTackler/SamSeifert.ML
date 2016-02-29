using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace ML
{
    public partial class DataDistributionNormalizer : UserControl
    {
        private bool _Loaded = false;
        private DataUseable _Train;
        private DataUseable _Test;
        private DateTime _DateLoadStart;

        public DataDistributionNormalizer()
        {
            InitializeComponent();

            this.checkBox1.Checked = Properties.Settings.Default.NormalizeDistribution;
        }

        internal void SetData(DataUseable train, DataUseable test)
        {
            this._Train = train;
            this._Test = test;
            this._Loaded = true;
            this.LoadData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NormalizeDistribution = this.checkBox1.Checked;
            Properties.Settings.Default.Save();

            this.LoadData();
        }

        private void LoadData()
        {
            if (!this._Loaded) return;
            if (this.bwLoadData.IsBusy)
            {
                this.labelDataStatus.Text = "Canceling last normalize...";
                this.bwLoadData.CancelAsync();
            }
            else
            {
                if (this.checkBox1.Checked)
                {
                    this.labelDataStatus.ForeColor = Color.OrangeRed;
                    this.labelDataStatus.Text = "Normalizing...";
                    this._DateLoadStart = DateTime.Now;

                    this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgs(
                        this._Train,
                        this._Test
                        ));
                }
                else
                {
                    this.labelDataStatus.Text = "Passed through!";
                    this.labelDataStatus.ForeColor = Color.Green;
                    if (this.DataPop != null)
                        this.DataPop(this._Train, this._Test);
                }
            }
        }

        private class ToBackgroundWorkerArgs
        {
            public DataUseable _Test;
            public DataUseable _Train;

            public ToBackgroundWorkerArgs(DataUseable train, DataUseable test)
            {
                this._Train = train;
                this._Test = test;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var args = e.Argument as ToBackgroundWorkerArgs;

                var label_counts = args._Train.getLabelCounts();


                var max_labels = label_counts.Values.Max();
                var total_rows = max_labels * label_counts.Count;

                if (total_rows > 5 * args._Train._CountRows)
                {
                    max_labels = 5 * args._Train._CountRows / label_counts.Count;
                    total_rows = max_labels * label_counts.Count;
                }

                var new_train_data = Matrix<float>.Build.Dense(total_rows, args._Train._CountColumns);
                var new_train_labels = Vector<float>.Build.Dense(total_rows);

                int new_dex = 0;

                foreach (var key in label_counts.Keys)
                {
                    int used = 0;
                    int old_dex = 0;

                    while (used < max_labels)
                    {
                        if (args._Train._Labels[old_dex] == key)
                        {
                            new_train_labels[new_dex] = key;
                            new_train_data.SetRow(new_dex, args._Train._Data.Row(old_dex));
                            new_dex++;
                            used++;
                        }
                        old_dex = (old_dex + 1) % args._Train._CountRows;
                    }
                }

                if (this.bwLoadData.CancellationPending) e.Result = null;
                else e.Result = new DataUseable[] {
                    new DataUseable(new_train_data, new_train_labels),
                    args._Test
                };
            }
            catch (Exception exc)
            {
                e.Result = exc.ToString();
            }
        }

        public event DataPopHandler DataPop;
        public delegate void DataPopHandler(DataUseable train, DataUseable test);

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is DataUseable[])
            {
                var train_and_test = e.Result as DataUseable[];

                this.labelDataStatus.ForeColor = Color.Green;
                this.labelDataStatus.Text = "Training data expanded to " + train_and_test[0]._CountRows +  
                    " points in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";


                if (this.DataPop != null)
                    this.DataPop(train_and_test[0], train_and_test[1]);
            }
            else if (e.Result is String)
            {
                this.labelDataStatus.Text = e.Result as String;
                Console.WriteLine(e.Result as String);
            }
            else
            {
                // Params Changed
                this.LoadData();
            }
        }
    }
}
