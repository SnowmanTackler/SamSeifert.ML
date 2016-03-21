using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.ML.Controls
{
    public partial class Splitter : UserControl
    {
        public readonly String DefaultText1;
        public readonly String DefaultText3;
        private Data.Useable[] _Data;
        private DateTime _DateLoadStart;

        public Splitter()
        {
            InitializeComponent();

            this.DefaultText1 = this.label1.Text;
            this.DefaultText3 = this.label3.Text;

            this.numericUpDown1.Value = Properties.Settings.Default.SplitPercent;
        }

        public event DataPopHandler DataPop;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SplitPercent = (int)Math.Round(this.numericUpDown1.Value);
            Properties.Settings.Default.Save();
            this.LoadData();
        }

        private bool _Loaded = false;
        public void SetData(Data.Useable[] di)
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
                    this.label1.Text = this.DefaultText1.Replace("X", this._Data[0]._CountRows.ToString());
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
                    this._DateLoadStart = DateTime.Now;
                    this.bwLoadData_RunWorkerCompleted(null, new RunWorkerCompletedEventArgs(
                        this._Data,
                        null,
                        false));
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
            public Data.Useable _Data;

            public ToBackgroundWorkerArgs(Data.Useable data, float percent_test)
            {
                this._Data = data;
                this._PercentTest = percent_test;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ToBackgroundWorkerArgs;

            var data = new Data.Useable[2];
            args._Data.Split(
                args._PercentTest,
                out data[1],
                out data[0]);
            
            if (this.bwLoadData.CancellationPending) e.Result = null;
            else e.Result = data;
        }

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Data.Useable[])
            {
                this.labelDataStatus.Text = "Split in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";
                this.labelDataStatus.ForeColor = Color.Green;

                var dat = e.Result as Data.Useable[];

                this.label3.Text = this.DefaultText3
                    .Replace("Y", dat[0]._CountRows.ToString())
                    .Replace("Z", dat[1]._CountRows.ToString());

                if (this.DataPop != null)
                    this.DataPop(dat);
            }
            else
            {
                // Params Changed
                this.LoadData();
            }
        }
    }
}
