using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ML
{
    public partial class DataSplitter : UserControl
    {
        public readonly String DefaultText1;
        public readonly String DefaultText3;
        private DataImported[] _Data;
        private DateTime _DateLoadStart;

        public DataSplitter()
        {
            InitializeComponent();

            this.DefaultText1 = this.label1.Text;
            this.DefaultText3 = this.label3.Text;

            this.numericUpDown1.Value = Properties.Settings.Default.SplitPercent;
        }

        public event DataPopHandler DataPop;
        public delegate void DataPopHandler(DataImported train, DataImported test);

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SplitPercent = (int)Math.Round(this.numericUpDown1.Value);
            Properties.Settings.Default.Save();
            this.LoadData();
        }

        private bool _Loaded = false;
        internal void SetData(DataImported[] di)
        {
            this._Data = di;
            this._Loaded = true;
            this.LoadData();
        }

        private void LoadData()
        {
            if (!this._Loaded) return;
            if (this.bwLoadData.IsBusy)
            {
                this.labelDataStatus.Text = "Canceling last split...";
                this.bwLoadData.CancelAsync();
            }
            else
            {
                this.labelDataStatus.ForeColor = Color.OrangeRed;

                if (this._Data == null)
                {
                    this.labelDataStatus.Text = "Error: I never got data!";
                    this.Enabled = true;
                    this.label1.Text = this.DefaultText1;
                    this.label3.Text = this.DefaultText3;
                }
                else if (this._Data.Length == 1)
                {

                    this._DateLoadStart = DateTime.Now;
                    this.Enabled = true;
                    this.label1.Text = this.DefaultText1.Replace("X", this._Data[0]._Rows.ToString());
                    this.label3.Text = "";
                    this.labelDataStatus.Text = "Splitting...";
                    this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgs(
                        this._Data[0],
                        (float)(this.numericUpDown1.Value) / 100));
                }
                else if (this._Data.Length == 2)
                {
                    this.labelDataStatus.ForeColor = Color.Green;
                    this.labelDataStatus.Text = "...";
                    this.Enabled = false;
                    this.label1.Text = this.DefaultText1;
                    this.label3.Text = this.DefaultText3;

                    if (this.DataPop != null)
                    {
                        this.DataPop(this._Data[0], this._Data[1]);
                    }
                }
                else
                {
                    this.labelDataStatus.Text = "Error: I was expecting 1 or 2 data objects!";
                    this.Enabled = true;
                    this.label1.Text = this.DefaultText1;
                    this.label3.Text = this.DefaultText3;
                }
            }
        }

        private class ToBackgroundWorkerArgs
        {
            public float _PercentTest;
            public DataImported _Data;

            public ToBackgroundWorkerArgs(DataImported data, float percent_test)
            {
                this._Data = data;
                this._PercentTest = percent_test;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ToBackgroundWorkerArgs;

            int count_test = (int)Math.Round(args._PercentTest * args._Data._Rows);
            int count_train = args._Data._Rows - count_test;

            var res = args._Data.Split(count_train, count_test);

            if (this.bwLoadData.CancellationPending) e.Result = null;
            else e.Result = res;
        }

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is DataImported[])
            {
                this.labelDataStatus.Text = "Split in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";
                this.labelDataStatus.ForeColor = Color.Green;

                var dat = e.Result as DataImported[];

                this.label3.Text = this.DefaultText3
                    .Replace("Y", dat[0]._Rows.ToString())
                    .Replace("Z", dat[1]._Rows.ToString());

                if (this.DataPop != null)
                    this.DataPop(dat[0], dat[1]);
            }
            else
            {
                // Params Changed
                this.LoadData();
            }
        }
    }
}
