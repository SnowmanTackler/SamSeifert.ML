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

namespace SamSeifert.ML.Controls
{
    public partial class ValueNormalizer : UserControl
    {
        private bool _Loaded = false;
        private Data.Useable _Train;
        private Data.Useable _Test;
        private DateTime _DateLoadStart;

        public ValueNormalizer()
        {
            InitializeComponent();

            this.checkBox1.Checked = Properties.Settings.Default.Normalize;
        }

        public void SetData(Data.Useable train, Data.Useable test)
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
                    this.labelDataStatus.ForeColor = Color.Green;
                    this.labelDataStatus.Text = "Passed through!";
                    if (this.DataPop != null)
                        this.DataPop(this._Train, this._Test);
                }
            }
        }

        private class ToBackgroundWorkerArgs
        {
            public Data.Useable _Test;
            public Data.Useable _Train;

            public ToBackgroundWorkerArgs(Data.Useable train, Data.Useable test)
            {
                this._Train = train;
                this._Test = test;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ToBackgroundWorkerArgs;

            try
            {
                Data.Useable train, test;
                Transforms.Normalizer.Normalize(args._Train, args._Test, out train, out test);
                if (this.bwLoadData.CancellationPending) e.Result = null;
                else e.Result = new Data.Useable[] { train, test };
            }
            catch (Exception exc)
            {
                e.Result = exc.ToString();
            }
        }

        public event DataPopHandler DataPop;
        public delegate void DataPopHandler(Data.Useable train, Data.Useable test);

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Data.Useable[])
            {
                this.labelDataStatus.ForeColor = Color.Green;
                this.labelDataStatus.Text = "Data normalized in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";

                var train_and_test = e.Result as Data.Useable[];

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
