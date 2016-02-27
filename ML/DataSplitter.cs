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
        public String DefaultText1;
        public String DefaultText3;
        private DataImported[] _Data;

        public DataSplitter()
        {
            InitializeComponent();

            this.DefaultText1 = this.label1.Text;
            this.DefaultText3 = this.label3.Text;
        }

        public event DataPopHandler DataPop;
        public delegate void DataPopHandler(DataImported train, DataImported test);

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
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
                this.bwLoadData.CancelAsync();
            }
            else
            {
                if (this._Data == null)
                {
                    this.Enabled = false;
                    this.label1.Text = this.DefaultText1;
                    this.label3.Text = this.DefaultText3;
                }
                else if (this._Data.Length == 1)
                {
                    this.Enabled = true;
                    this.label1.Text = this.DefaultText1.Replace("X", this._Data[0]._Rows.ToString());
                    this.label3.Text = "working...";
                    this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgs(
                        this._Data,
                        (float)this.numericUpDown1.Value));
                }
                else if (this._Data.Length == 2)
                {
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
                    MessageBox.Show("I was expecting 1 or 2 data objects!");
                }
            }
        }


        private class ToBackgroundWorkerArgs
        {
            private float _PercentTest;
            private DataImported[] _Data;

            public ToBackgroundWorkerArgs(DataImported[] _Data, float value)
            {
                this._Data = _Data;
                this._PercentTest = value;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ToBackgroundWorkerArgs;




            if (this.bwLoadData.CancellationPending) e.Result = null;
            else e.Result = new DataImported[0];
        }

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is DataImported[])
            {
                var dat = e.Result as DataImported[];
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
