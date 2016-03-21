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

namespace SamSeifert.ML.Controls
{
    public partial class Transformer : UserControl
    {
        public event DataPopHandler DataPop;
        private bool _Loaded = false;
        private Preprocess.Transform _Data;
        private DateTime _DateLoadStart;

        public Transformer()
        {
            InitializeComponent();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!this._Loaded) return;

            int nv = (int)Math.Round(this.numericUpDown1.Value);

            Properties.Settings.Default.TransformNumber = nv;
            Properties.Settings.Default.Save();

            this.LoadData();
        }

        public void SetData(Preprocess.Transform fi)
        {
            this._Loaded = false;

            this.numericUpDown1.Value = 0;
            this.numericUpDown1.Maximum = fi._MaxCount;
            this.numericUpDown1.Value = Math.Min(Math.Max(
                // Properties.Settings.Default.TransformNumber,
                20,
                this.numericUpDown1.Minimum),
                this.numericUpDown1.Maximum);

            this._Data = fi;

            this._Loaded = true;
            
            this.numericUpDown1_ValueChanged(null, EventArgs.Empty);

            this.numericUpDown1.Enabled = this._Data._Transform != null;
        }

        private void LoadData()
        {
            if (!this._Loaded) return;
            if (this.bwLoadData.IsBusy)
            {
                this.labelDataStatus.Text = "Canceling last transform...";
                this.bwLoadData.CancelAsync();
            }
            else
            {
                if (this._Data._Transform == null)
                {
                    this.Enabled = false;
                    this.labelDataStatus.ForeColor = Color.Green;
                    this.labelDataStatus.Text = "Passed through!";
                    if (this.DataPop != null)
                        this.DataPop(this._Data._Data);
                }
                else
                {
                    this.Enabled = true;
                    this.labelDataStatus.ForeColor = Color.OrangeRed;
                    this.labelDataStatus.Text = "Transforming...";
                    this._DateLoadStart = DateTime.Now;

                    int count = (int)Math.Round(this.numericUpDown1.Value);
                    if (count == 0) count = this._Data._MaxCount;

                    this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgs(this._Data, count));
                }
            }
        }

        private class ToBackgroundWorkerArgs
        {
            public Preprocess.Transform _PreProcessTransform;
            public int _Count = 0;

            public ToBackgroundWorkerArgs(Preprocess.Transform data, int count)
            {
                this._Count = count;
                this._PreProcessTransform = data;
            }
        }

        private IEnumerable<Vector<float>> GetForMatrix(Matrix<float> m, int count)
        {
            for (int i = 0; i < count; i++)
                yield return m.Column(i);
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ToBackgroundWorkerArgs;

            var mat = Matrix<float>.Build.DenseOfColumns(this.GetForMatrix(
                args._PreProcessTransform._Transform,
                args._Count));

            var du = new Datas.Useable[args._PreProcessTransform._Data.Length];

            for (int i = 0; i < du.Length; i++)
                du[i] = new Datas.Useable(
                    args._PreProcessTransform._Data[i]._Data * mat,
                    args._PreProcessTransform._Data[i]._Labels);

            if (this.bwLoadData.CancellationPending) e.Result = null;
            else e.Result = du;
        }


        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Datas.Useable[])
            {
                var data = e.Result as Datas.Useable[];

                this.labelDataStatus.ForeColor = Color.Green;
                this.labelDataStatus.Text =
                    "Transformed to " + data[0]._Data.ColumnCount + 
                    " columns in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + 
                    " seconds!";

                if (this.DataPop != null)
                    this.DataPop(data);
            }
            else
            {
                // Params Changed
                this.LoadData();
            }
        }
    }
}
