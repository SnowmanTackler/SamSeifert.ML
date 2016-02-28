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
    public partial class DataTransformer : UserControl
    {
        private bool _Loaded = false;
        private DataPreProcess.PreProcessTransform _Data;
        private DateTime _DateLoadStart;

        public DataTransformer()
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

        internal void SetData(DataPreProcess.PreProcessTransform fi)
        {
            this._Loaded = false;

            this.numericUpDown1.Value = 0;
            this.numericUpDown1.Maximum = fi._MaxCount;
            this.numericUpDown1.Value = Math.Min(fi._MaxCount, Properties.Settings.Default.TransformNumber);

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
                    this.labelDataStatus.ForeColor = Color.Green;
                    this.labelDataStatus.Text = "Passed through!";
                    if (this.DataPop != null)
                        this.DataPop(this._Data._Train, this._Data._Test);
                }
                else
                {
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
            public DataPreProcess.PreProcessTransform _PreProcessTransform;
            public int _Count = 0;

            public ToBackgroundWorkerArgs(DataPreProcess.PreProcessTransform data, int count)
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

            var new_train_data = args._PreProcessTransform._Train._Data * mat;
            var new_test_data = args._PreProcessTransform._Test._Data * mat;

            if (this.bwLoadData.CancellationPending) e.Result = null;
            else e.Result = new DataUseable[] {
                new DataUseable(new_train_data, args._PreProcessTransform._Train._Labels.Clone()),
                new DataUseable(new_test_data, args._PreProcessTransform._Test._Labels.Clone())
            };
        }

        public event DataPopHandler DataPop;
        public delegate void DataPopHandler(DataUseable train, DataUseable test);

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is DataUseable[])
            {
                var train_and_test = e.Result as DataUseable[];

                this.labelDataStatus.ForeColor = Color.Green;
                this.labelDataStatus.Text = "Transformed to " + train_and_test[0]._Data.ColumnCount + " columns in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";

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
