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
using MathNet.Numerics.LinearAlgebra;

namespace SamSeifert.ML.Controls
{
    public partial class Preprocess : UserControl
    {
        public event DataPopHandler DataPop;
        public delegate void DataPopHandler(Transform fi);

        private bool _Loaded = false;
        private Data.Useable[] _Data;
        private DateTime _DateLoadStart;

        public Preprocess()
        {
            InitializeComponent();

            this.rbNone.Tag = 0;
            this.rbPCA.Tag = 1;
            this.rbLDA.Tag = 2;

            switch (Properties.Settings.Default.PreprocessType)
            {
                case 1:
                    this.rbPCA.Checked = true;
                    break;
                case 2:
                    this.rbLDA.Checked = true;
                    break;
                default:
                    this.rbNone.Checked = true;
                    break;
            }
        }

        private void RadioButtonChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.Checked)
                {
                    if (rb == this.rbNone)
                    {

                    }
                    else // PCA or LDA
                    {
                    }

                    this.LoadData();

                    Properties.Settings.Default.PreprocessType = (int)rb.Tag;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public void SetData(Data.Useable[] data)
        {
            this._Data = data;
            this._Loaded = true;
            this.LoadData();
        }


        private void LoadData()
        {
            if (!this._Loaded) return;
            if (this.bwLoadData.IsBusy)
            {
                this.labelDataStatus.Text = "Canceling last pre-processing...";
                this.bwLoadData.CancelAsync();
            }
            else
            {
                if (this.rbNone.Checked)
                {
                    this.labelDataStatus.ForeColor = Color.Green;
                    this.labelDataStatus.Text = "Passed through!";
                    if (this.DataPop != null)
                        this.DataPop(new Transform(
                            this._Data,
                            null,
                            this._Data[0]._CountColumns));
                }
                else
                {
                    this.labelDataStatus.ForeColor = Color.OrangeRed;
                    this.labelDataStatus.Text = "Pre-processing...";
                    this._DateLoadStart = DateTime.Now;

                    this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgs(
                        this._Data,
                        this.rbPCA.Checked
                        ));
                }
            }
        }

        private class ToBackgroundWorkerArgs
        {
            public Data.Useable[] _Data;
            public bool _True_PCA_False_LDA = false;

            public ToBackgroundWorkerArgs(Data.Useable[] data, bool true_PCA_false_LDA)
            {
                this._Data = data;
                this._True_PCA_False_LDA = true_PCA_false_LDA;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ToBackgroundWorkerArgs;

            int max_count = 0;
            Matrix<float> transform = null;

            if (args._True_PCA_False_LDA)
            {
                var cov = args._Data[0]._Data.Covariance();

                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " PCA Got Covariance");

                var eigen_decomp = cov.Evd(Symmetricity.Symmetric);

                var eigen_values_complex = eigen_decomp.EigenValues;

                var ls = new List<Vector<float>>();

                foreach (var val in eigen_values_complex)
                {
                    if ((val.Imaginary == 0) && (val.Real != 0))
                    {
                        ls.Add(eigen_decomp.EigenVectors.Column(max_count));
                        max_count++;
                    }
                }

                ls.Reverse(); // Biggest Eigens Last!

                transform = Matrix<float>.Build.DenseOfColumns(ls);

                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " PCA Got Eigens: " +
                    (ls.Count).ToString() + " useable " +
                    (eigen_values_complex.Count - ls.Count).ToString() + " wasted");
            }

            if (this.bwLoadData.CancellationPending) e.Result = null;
            else e.Result = new Transform(
                args._Data,
                transform,
                max_count);
        }


        public class Transform
        {
            public int _MaxCount;
            public Matrix<float> _Transform;
            public Data.Useable[] _Data;

            public Transform(Data.Useable[] data, Matrix<float> transform, int max_count)
            {
                this._Data = data;
                this._Transform = transform;
                this._MaxCount = max_count;
            }
        }

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Transform)
            {
                this.labelDataStatus.ForeColor = Color.Green;
                this.labelDataStatus.Text = "Data pre-processed in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";

                var from = e.Result as Transform;

                if (this.DataPop != null)
                    this.DataPop(from);
            }
            else
            {
                // Params Changed
                this.LoadData();
            }
        }
    }
}
