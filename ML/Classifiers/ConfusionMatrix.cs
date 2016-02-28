using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Classifiers
{
    public class ConfusionMatrix
    {
        /// <summary>
        /// column number = actual,
        /// row number = predicted
        /// </summary>
        private readonly Matrix<float> _Matrix;
        private readonly float[] _KeysIndexToValues;
        private readonly Dictionary<float, int> _KeysValuesToIndex;
        private readonly int _TotalPoints;

        private int _NumberVariables
        {
            get
            {
                return this._KeysIndexToValues.Length;
            }
        }

        public ConfusionMatrix(Func<float[], float> func, DataUseable data)
        {
            // Point.x is actual
            // Point.y is predicted
            var counts = new Dictionary<PointF, int>();

            int rows = data._DataRows;
            int cols = data._DataColumns;
            int count = 0;

            var parameters = new float[cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                    parameters[c] = data._Data[r, c];


                var key = new PointF(data._Labels[r], func(parameters));

                if (!counts.TryGetValue(key, out count)) count = 0;
                counts[key] = ++count;
            }


            var hs = new HashSet<float>();
            foreach (var pt in counts.Keys)
            {
                hs.Add(pt.X);
                hs.Add(pt.Y);
            }

            this._KeysIndexToValues = hs.ToArray();
            Array.Sort(this._KeysIndexToValues);

            this._KeysValuesToIndex = new Dictionary<float, int>();
            count = 0;
            foreach (var value in this._KeysIndexToValues)
                this._KeysValuesToIndex[value] = count++;

            this._Matrix = Matrix<float>.Build.Dense(this._KeysIndexToValues.Length, this._KeysIndexToValues.Length, 0);

            foreach (var kvp in counts)
            {
                this._Matrix[this._KeysValuesToIndex[kvp.Key.X],
                             this._KeysValuesToIndex[kvp.Key.Y]] = kvp.Value;
            }

            this._TotalPoints = rows;
        }

        public float Accuracy
        {
            get
            {
                float sum = 0;
                for (int i = 0; i < this._NumberVariables; i++)
                    sum += this._Matrix[i, i];

                return sum / this._TotalPoints;
            }
        }
    }
}
