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
        public event DataPopHandler DataPop;
        private bool _Loaded = false;
        private Datas.Useable[] _Data;
        private DateTime _DateLoadStart;

        public ValueNormalizer()
        {
            InitializeComponent();

            this.checkBox1.Checked = Properties.Settings.Default.Normalize;
        }

        public void SetData(Datas.Useable[] data)
        {
            this._Data = data;
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
                        this._Data));
                }
                else
                {
                    this.labelDataStatus.ForeColor = Color.Green;
                    this.labelDataStatus.Text = "Passed through!";
                    if (this.DataPop != null)
                        this.DataPop(this._Data);
                }
            }
        }

        private class ToBackgroundWorkerArgs
        {
            public Datas.Useable[] _Data;

            public ToBackgroundWorkerArgs(Datas.Useable[] data)
            {
                this._Data = data;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ToBackgroundWorkerArgs;

            try
            {
                Datas.Useable[] data;
                Transforms.Normalizer.Normalize(args._Data, out data);
                if (this.bwLoadData.CancellationPending) e.Result = null;
                else e.Result = data;
            }
            catch (Exception exc)
            {
                e.Result = exc.ToString();
            }
        }

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Datas.Useable[])
            {
                this.labelDataStatus.ForeColor = Color.Green;
                this.labelDataStatus.Text = "Data normalized in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";

                if (this.DataPop != null)
                    this.DataPop(e.Result as Datas.Useable[]);
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
