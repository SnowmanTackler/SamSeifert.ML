using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SamSeifert.MathNet.Numerics.Extensions;

namespace ML
{
    public partial class DataNormalizer : UserControl
    {
        private bool _Loaded = false;
        private DataUseable _Train;
        private DataUseable _Test;
        private DateTime _DateLoadStart;

        public DataNormalizer()
        {
            InitializeComponent();

            this.checkBox1.Checked = Properties.Settings.Default.Normalize;
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
            Properties.Settings.Default.Normalize = this.checkBox1.Checked;
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
            var args = e.Argument as ToBackgroundWorkerArgs;

            var train_means = args._Train._Data.MeanRow();

            var new_train_data = args._Train._Data.AddRow(-train_means);
            var new_test_data = args._Test._Data.AddRow(-train_means);

            var stds = args._Train._Data.StandardDeviationRow(true);
            for (int i = 0; i < stds.Count; i++)
            {
                if ((stds[i] == 0) || float.IsNaN(stds[i]) || float.IsInfinity(stds[i])) stds[i] = 1;
                else stds[i] = 1 / stds[i];
            }

            new_train_data = new_train_data.MultiplyRow(stds);
            new_test_data = new_test_data.MultiplyRow(stds);



            if (this.bwLoadData.CancellationPending) e.Result = null;
            else e.Result = new DataUseable[] {
                new DataUseable(new_train_data, args._Train._Labels.Clone()),
                new DataUseable(new_test_data, args._Test._Labels.Clone())
        };
        }

        public event DataPopHandler DataPop;
        public delegate void DataPopHandler(DataUseable train, DataUseable test);

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is DataUseable[])
            {
                this.labelDataStatus.ForeColor = Color.Green;
                this.labelDataStatus.Text = "Data normalized in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";

                var train_and_test = e.Result as DataUseable[];

                if (this.DataPop != null)
                    this.DataPop(train_and_test[0], train_and_test[1]);
            }
            else
            {
                // Params Changed
                this.LoadData();
            }
        }

    }
}
