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
    }
}
