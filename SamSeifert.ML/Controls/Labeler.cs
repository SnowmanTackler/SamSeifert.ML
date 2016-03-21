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
using SamSeifert.Utilities;

namespace SamSeifert.ML.Controls
{
    /// <summary>
    /// First one is training.  Every other one is test (sometimes more than 1 for cross validation)
    /// </summary>
    /// <param name="data"></param>
    public delegate void DataPopHandler(Data.Useable[] data);

    public partial class Labeler : UserControl
    {
        public event DataPopHandler DataPop;

        public readonly String DefaultText1;
        private bool _Loaded = false;
        private Data.ImportCSV[] _Data;
        private DateTime _DateLoadStart;

        public Labeler()
        {
            InitializeComponent();

            this.textBox1.Text = Properties.Settings.Default.LabelerIgnore;

            this.DefaultText1 = this.label1.Text;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!this._Loaded) return;

            int nv = (int)Math.Round(this.numericUpDown1.Value);

            Properties.Settings.Default.LabelerIndex = nv;
            Properties.Settings.Default.Save();
         
            this.LoadData();
        }

        public void SetData(Data.ImportCSV[] data)
        {
            this._Loaded = false;

            int cols = data[0]._Columns;

            this.numericUpDown1.Value = 0;
            this.numericUpDown1.Maximum = cols - 1;
            this.numericUpDown1.Value = Math.Min(cols - 1, Properties.Settings.Default.LabelerIndex);

            this.label1.Text = this.DefaultText1.Replace("X", cols.ToString());

            this._Data = data;

            this._Loaded = true;

            this.numericUpDown1_ValueChanged(null, EventArgs.Empty);
        }


        private HashSet<int> _Ignoring = new HashSet<int>();
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this._Ignoring.Clear();
            bool all_good = true;
            foreach (var s in this.textBox1.Text.Split(','))
            {
                var st = s.Trim();
                if (st.Length > 0)
                {
                    int outp;
                    if (int.TryParse(st, out outp)) this._Ignoring.Add(outp);
                    else all_good = false;
                }
            }

            Properties.Settings.Default.LabelerIgnore = this.textBox1.Text;
            Properties.Settings.Default.Save();

            this.textBox1.ForeColor = all_good ? Color.Green : Color.OrangeRed;
            this.LoadData();
        }

        private void LoadData()
        {
            if (!this._Loaded) return;
            if (this.bwLoadData.IsBusy)
            {
                this.labelDataStatus.Text = "Canceling last extract...";
                this.bwLoadData.CancelAsync();
            }
            else
            {
                this.labelDataStatus.Text = "Extracting labels...";
                this.labelDataStatus.ForeColor = Color.OrangeRed;
                this._DateLoadStart = DateTime.Now;

                int labels = (int)Math.Round(this.numericUpDown1.Value);
                var pass_through = new Boolean[this._Data[0]._Columns];

                int nv = (int)Math.Round(this.numericUpDown1.Value);

                for (int i = 0; i < this._Data[0]._Columns; i++)
                {
                    if (nv == i) pass_through[i] = false;
                    else if (this._Ignoring.Contains(i)) pass_through[i] = false;
                    else pass_through[i] = true;
                }

                this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgs(
                    labels,
                    pass_through,
                    this._Data
                    ));
            }
        }

        private class ToBackgroundWorkerArgs
        {
            public int _LabelIndex;
            public bool[] _PassThrough;
            public Data.ImportCSV[] _Data;

            public ToBackgroundWorkerArgs(int labels, bool[] pass_through, Data.ImportCSV[] data)
            {
                this._LabelIndex = labels;
                this._PassThrough = pass_through;
                this._Data = data;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ToBackgroundWorkerArgs;

            var ret = new Data.Useable[args._Data.Length];
            int ret_dex = 0;

            int new_colmns = 0;
            foreach (var b in args._PassThrough)
                if (b)
                    new_colmns++;
                    
            int new_dex = 0;
            int old_dex = 0;

            var new_columns_indices = new int[new_colmns];

            foreach (var b in args._PassThrough)
            {
                if (b) new_columns_indices[new_dex++] = old_dex++;
                else old_dex++;
            }
                    
            foreach (var di in args._Data)
            {
                int rows = di._Rows;

                var data = Matrix<float>.Build.Dense(rows, new_colmns, 0);
                var data_labels = Vector<float>.Build.Dense(rows);

                for (int r = 0; r < rows; r++)
                {
                    var row = di._DataPoints[r];

                    for (int c = 0; c < new_colmns; c++)
                        data[r, c] = row[new_columns_indices[c]];

                    data_labels[r] = row[args._LabelIndex];
                }

                ret[ret_dex++] = new Data.Useable(data, data_labels);
            }

            if (this.bwLoadData.CancellationPending) e.Result = null;
            else e.Result = ret;
        }

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Data.Useable[])
            {
                this.labelDataStatus.Text = "Labels extracted in " + (DateTime.Now - this._DateLoadStart).TotalSeconds.ToString("0.00") + " seconds!";
                this.labelDataStatus.ForeColor = Color.Green;

                var datas = e.Result as Data.Useable[];

                while (this.panel1.Controls.Count != 0)
                {
                    var cont = this.panel1.Controls[0];
                    cont.RemoveFromParent();
                    cont.Dispose();
                }

                for (int i = 0; i < datas.Length; i++)
                {
                    var data = datas[i];
                    var counts = data.getLabelCounts();

                    var sb = new StringBuilder();
                    sb.Append(((i == 0) ? "Train" : ("Test " + i)));
                    sb.Append(":");

                    var keys = counts.Keys.ToArray();
                    Array.Sort(keys);

                    foreach (var key in keys)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append(key.ToString());
                        sb.Append("  :  ");
                        sb.Append(Math.Round((counts[key] * 100.0f) / data._CountRows));
                        sb.Append("%");
                    }

                    Label l = new Label();
                    this.panel1.Controls.Add(l);
                    l.Text = sb.ToString();
                    l.AutoSize = false;
                    l.Dock = DockStyle.Left;
                    l.Width = 100;
                    l.Text = sb.ToString();
                }

                if (this.DataPop != null)
                    this.DataPop(datas);
            }
            else
            {
                // Params Changed
                this.LoadData();
            }
        }

    }
}
