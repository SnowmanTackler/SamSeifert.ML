using System;
using SamSeifert.ML.Classifiers;
using SamSeifert.ML.Datas;
using System.Drawing;
using System.Collections.Generic;
using SamSeifert.Utilities;
using MathNet.Numerics.LinearAlgebra;

namespace SamSeifert.ML.Classifiers
{
    internal class kNN : Classifier
    {
        private int _kNN;

        public kNN(int kNN)
        {
            this._kNN = kNN;
        }

        public float Compile(float[] fs)
        {
            //var vec = Vector<float>.Build.DenseOfArray(fs);
            //pt.X = (float)(row - vec).L2Norm();

            int index = 0;

            var top_points = new PointF[this._kNN + 1]; // 1 extra slot at index 0.  add new points to this slot then bubble sort
            top_points[this._kNN] = new PointF(float.MaxValue, 0); // Starter

            foreach (var row in this._Data._Data.EnumerateRows())
            {
                var pt = new PointF(0, this._Data._Labels[index]);

                for (int i = 0; i < fs.Length; i++)
                {
                    var dist = fs[i] - row[i];
                    pt.X += dist * dist;
                }

                if (index < this._kNN)
                {
                    top_points[index] = pt;
                    index++;
                    if (index == this._kNN) Array.Sort(top_points, (a, b) => b.X.CompareTo(a.X));
                }
                else
                {
                    index++;
                    top_points[0] = pt;
                    for (int offset = 0; // Bubble Sort
                        (offset <= this._kNN) && (top_points[offset].X < top_points[offset + 1].X);                         
                        offset++)
                    {
                        Utilities.MiscUtil.Swap(
                            ref top_points[offset],
                            ref top_points[offset + 1]);
                    }
                }
            }

            var dict = new Dictionary<float, int>();

            for (int i = 1; i < Math.Min(index, this._kNN + 1); i++)
            {
                float vote = top_points[i].Y;
                int count;
                if (!dict.TryGetValue(vote, out count)) count = 0;
                dict[vote] = ++count;
            }

            return dict.ArgMax();
        }


        private Useable _Data;
        public void Train(Useable indata)
        {
            this._Data = indata;
        }
    }
}