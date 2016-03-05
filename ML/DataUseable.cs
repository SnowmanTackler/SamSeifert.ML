using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace ML
{
    public class DataUseable
    {
        /// <summary>
        /// Rows are entries, columns are different parameters
        /// </summary>
        public Matrix<float> _Data;

        public Vector<float> _Labels;

        public DataUseable(Matrix<float> data, Vector<float> labels)
        {
            if (data != null)
                if (labels != null)
                    if (data.RowCount != labels.Count) throw new Exception("Size Mismatch");

            this._Data = data;
            this._Labels = labels;
        }

        public int _CountColumns
        {
            get
            {
                return this._Data.ColumnCount;
            }
        }

        public int _CountRows
        {
            get
            {
                return this._Labels.Count;
            }
        }

        public Dictionary<float, int> getLabelCounts()
        {
            var label_counts = new Dictionary<float, int>();

            foreach (var f in this._Labels)
            {
                int count;
                if (!label_counts.TryGetValue(f, out count)) count = 0;
                label_counts[f] = ++count;
            }

            return label_counts;
        }
        /// <summary>
        /// Split as in a decision tree
        /// </summary>
        /// <param name="split_column"></param>
        /// <param name="split_value"></param>
        /// <param name="less"></param>
        /// <param name="more"></param>
        internal void Split(
            int split_column, 
            float split_value,
            out DataUseable less,
            out DataUseable more)
        {
            int count_less = 0;

            int rows = this._CountRows;
            int cols = this._CountColumns;

            for (int r = 0; r < rows; r++)
                if (this._Data[r, split_column] < split_value)
                    count_less++;

            int count_more = rows - count_less;

            Vector<float> labels_less = Vector<float>.Build.Dense(count_less);
            Matrix<float> data___less = Matrix<float>.Build.Dense(count_less, cols);

            Vector<float> labels_more = Vector<float>.Build.Dense(count_more);
            Matrix<float> data___more = Matrix<float>.Build.Dense(count_more, cols);

            int dex_less = 0;
            int dex_more = 0;

            for (int r = 0; r < rows; r++)
            {
                if (this._Data[r, split_column] < split_value)
                {
                    data___less.SetRow(dex_less, this._Data.Row(r));
                    labels_less[dex_less] = this._Labels[r];
                    dex_less++;
                }
                else
                {
                    data___more.SetRow(dex_more, this._Data.Row(r));
                    labels_more[dex_more] = this._Labels[r];
                    dex_more++;
                }
            }

            less = new DataUseable(data___less, labels_less);
            more = new DataUseable(data___more, labels_more);
        }

        /// <summary>
        /// Randomly split data to percentage.
        /// </summary>
        /// <param name="percent_first"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        internal void Split(
            float percent_first,
            out DataUseable first,
            out DataUseable second)
        {
            int rows = this._CountRows;
            int cols = this._CountColumns;

            int count_first = Math.Max(1, (int)Math.Round(percent_first * this._CountRows));
            int count_second = rows - count_first;

            var bools = Util.PickRandom(count_first, count_second);


            Vector<float> labels_first = Vector<float>.Build.Dense(count_first);
            Vector<float> labels_second = Vector<float>.Build.Dense(count_second);

            Matrix<float> data_first = Matrix<float>.Build.Dense(count_first, cols);
            Matrix<float> data_second = Matrix<float>.Build.Dense(count_second, cols);

            int dex_less = 0;
            int dex_more = 0;

            for (int r = 0; r < rows; r++)
            {
                if (bools[r])
                {
                    data_first.SetRow(dex_less, this._Data.Row(r));
                    labels_first[dex_less] = this._Labels[r];
                    dex_less++;
                }
                else
                {
                    data_second.SetRow(dex_more, this._Data.Row(r));
                    labels_second[dex_more] = this._Labels[r];
                    dex_more++;
                }
            }

            first = new DataUseable(data_first, labels_first);
            second = new DataUseable(data_second, labels_second);
        }
    }
}
