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
    public partial class TrainingDataLabelNormalizer : UserControl
    {
        public event DataPopHandler DataPop;
        private bool _Loaded = false;
        private Datas.Useable[] _Data;
        private DateTime _DateLoadStart;

        public TrainingDataLabelNormalizer()
        {
            InitializeComponent();

            this.checkBox1.Checked = Properties.Settings.Default.NormalizeDistribution;
        }

        public void SetData(Datas.Useable[] data)
        {
            this._Data = data;
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
                        this._Data));
                }
                else
                {
                    this.labelDataStatus.Text = "Passed through!";
                    this.labelDataStatus.ForeColor = Color.Green;
                    if (this.DataPop != null)
                        this.DataPop(this._Data);
                }
            }
        }

        private class ToBackgroundWorkerArgs
        {
            public Datas.Useable[] _Data;

            public ToBackgroundWorkerArgs(Datas.Useable[] train)
            {
                this._Data = train;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var args = e.Argument as ToBackgroundWorkerArgs;

                var train = args._Data[0];

                var label_counts = train.getLabelCounts();


                var max_labels = label_counts.Values.Max();
                var total_rows = max_labels * label_counts.Count;

                if (total_rows > 5 * train._CountRows)
                {
                    max_labels = 5 * train._CountRows / label_counts.Count;
                    total_rows = max_labels * label_counts.Count;
                }

                var new_train_data = Matrix<float>.Build.Dense(total_rows, train._CountColumns);
                var new_train_labels = Vector<float>.Build.Dense(total_rows);

                int new_dex = 0;

                foreach (var key in label_counts.Keys)
                {
                    int used = 0;
                    int old_dex = 0;

                    while (used < max_labels)
                    {
                        if (train._Labels[old_dex] == key)
                        {
                            new_train_labels[new_dex] = key;
                            new_train_data.SetRow(new_dex, train._Data.Row(old_dex));
                            new_dex++;
                            used++;
                        }
                        old_dex = (old_dex + 1) % train._CountRows;
                    }
                }

                if (this.bwLoadData.CancellationPending) e.Result = null;
                else
                {
                    var ret = args._Data.Clone() as Datas.Useable[];
                    ret[0] = new Datas.Useable(new_train_data, new_train_labels);
                    e.Result = ret;
                }
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
                var train_and_test = e.Result as Datas.Useable[];

                this.labelDataStatus.ForeColor = Color.Green;
                this.labelDataStatus.Text = "Training data expanded to " + train_and_test[0]._CountRows +  
                    " points in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";


                if (this.DataPop != null)
                    this.DataPop(train_and_test);
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
