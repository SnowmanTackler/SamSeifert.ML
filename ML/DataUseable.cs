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
            this._Data = data;
            this._Labels = labels;
        }

        public int _DataColumns
        {
            get
            {
                return this._Data.ColumnCount;
            }
        }

        public int _DataRows
        {
            get
            {
                return this._Labels.Count;
            }
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

            int rows = this._DataRows;
            int cols = this._DataColumns;

            for (int r = 0; r < rows; r++)
                if (this._Data[r, split_column] < split_value)
                    count_less++;

            int count_more = rows - count_less;

            Vector<float> labels_less = Vector<float>.Build.Dense(count_less);
            Vector<float> labels_more = Vector<float>.Build.Dense(count_more);

            Matrix<float> data_less = Matrix<float>.Build.Dense(count_less, cols);
            Matrix<float> data_more = Matrix<float>.Build.Dense(count_more, cols);

            int dex_less = 0;
            int dex_more = 0;

            for (int r = 0; r < rows; r++)
            {
                if (this._Data[r, split_column] < split_value)
                {
                    data_less.SetRow(dex_less, this._Data.Row(r));
                    labels_less[dex_less] = this._Labels[r];
                    dex_less++;
                }
                else
                {
                    data_more.SetRow(dex_more, this._Data.Row(r));
                    labels_more[dex_more] = this._Labels[r];
                    dex_more++;
                }
            }

            less = new DataUseable(data_less, labels_less);
            more = new DataUseable(data_more, labels_more);
        }
    }
}
