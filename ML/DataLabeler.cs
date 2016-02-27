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
    public partial class DataLabeler : UserControl
    {
        public readonly String DefaultText1;
        private bool _Loaded = false;
        private DataImported _Train;
        private DataImported _Test;

        public DataLabeler()
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

        internal void SetData(DataImported train, DataImported test)
        {
            this._Loaded = false;

            int cols = train._Columns;

            this.numericUpDown1.Value = 0;
            this.numericUpDown1.Maximum = cols - 1;
            this.numericUpDown1.Value = Math.Min(cols - 1, Properties.Settings.Default.LabelerIndex);

            this.label1.Text = this.DefaultText1.Replace("X", cols.ToString());

            this._Train = train;
            this._Test = test;

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
                this.bwLoadData.CancelAsync();
            }
            else
            {
                int labels = (int)Math.Round(this.numericUpDown1.Value);
                var pass_through = new Boolean[this._Train._Columns];

                int nv = (int)Math.Round(this.numericUpDown1.Value);

                for (int i = 0; i < this._Train._Columns; i++)
                {
                    if (nv == i) pass_through[i] = false;
                    else if (this._Ignoring.Contains(i)) pass_through[i] = false;
                    else pass_through[i] = true;
                }

                this.bwLoadData.RunWorkerAsync(new ToBackgroundWorkerArgs(
                    labels,
                    pass_through,
                    this._Train,
                    this._Test
                    ));
            }
        }

        private class ToBackgroundWorkerArgs
        {
            public int _LabelIndex;
            public bool[] _PassThrough;
            public DataImported _Test;
            public DataImported _Train;

            public ToBackgroundWorkerArgs(int labels, bool[] pass_through, DataImported train, DataImported test)
            {
                this._LabelIndex = labels;
                this._PassThrough = pass_through;
                this._Train = train;
                this._Test = test;
            }
        }

        private void bwLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ToBackgroundWorkerArgs;

            var ret = new DataUseable[2];
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
                    
            foreach (var di in new DataImported[] { args._Train, args._Test })
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

                ret[ret_dex++] = new DataUseable(data, data_labels);
            }

            if (this.bwLoadData.CancellationPending) e.Result = null;
            else e.Result = ret;
        }

        public event DataPopHandler DataPop;
        public delegate void DataPopHandler(DataUseable train, DataUseable test);

        private void bwLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is DataUseable[])
            {
                var train_and_test = e.Result as DataUseable[];
                var labels = new Label[] { this.label5, this.label6 };

                for (int i = 0; i < 2; i++)
                {
                    var data = train_and_test[i];
                    var counts = new Dictionary<float, int>();

                    foreach (var f in data._Labels)
                    {
                        int count;
                        if (!counts.TryGetValue(f, out count)) count = 0;
                        counts[f] = ++count;
                    }

                    var sb = new StringBuilder();
                    sb.Append(((i == 0) ? "Train" : "Test"));
                    sb.Append(":");

                    var keys = counts.Keys.ToArray();
                    Array.Sort(keys);

                    foreach (var key in keys)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append(key.ToString());
                        sb.Append("  :  ");
                        sb.Append(Math.Round((counts[key] * 100.0f) / data._Labels.Count));
                        sb.Append("%");
                    }

                    labels[i].Text = sb.ToString();
                }

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
