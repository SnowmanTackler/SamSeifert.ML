using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSeifert.Utilities;

namespace SamSeifert.ML.Classifiers.BoostableClassifiers
{
    public class DecisionStump : BoostableClassifier
    {
        public bool _IsLeaf = false;

        public float _LeafClassification = 0;

        public int _BranchColumn;
        public float _BranchSplitValue;
        public float _BranchLess;
        public float _BranchMore;

        public DecisionStump()
        {
        }

        public float Predict(float[] fs)
        {
            if (this._IsLeaf) return this._LeafClassification;
            else if (fs[this._BranchColumn] < this._BranchSplitValue) return this._BranchLess;
            else return this._BranchMore;
        }

        public void Train(Data.Useable data, float[] weights)
        {
            var branch_score = new Dictionary<float, float>();

            for (int i = 0; i < weights.Length; i++)
            {
                float f = data._Labels[i];
                float count;
                if (!branch_score.TryGetValue(f, out count)) count = 0;
                branch_score[f] = count + weights[i];
            }

            float max_correct_branch = branch_score.Values.Max();

            if (branch_score.Values.Sum() != max_correct_branch) // All children are in one cluster.
            {
                int cols = data._CountColumns;
                int rows = data._CountRows;

                var tups = new Tuple<float, float, float>[rows];

                float max_correct = max_correct_branch;

                for (int c = 0; c < cols; c++)
                {
                    for (int r = 0; r < rows; r++)
                        tups[r] = new Tuple<float, float, float>(
                            data._Labels[r],
                            data._Data[r, c],
                            weights[r]);

                    Array.Sort(tups, (Tuple<float, float, float> a, Tuple<float, float, float> b) =>
                    { return a.Item2.CompareTo(b.Item2); });

                    var branch_less_data = new Dictionary<float, float>();
                    var branch_more_data = new Dictionary<float, float>();

                    foreach (var kvp in branch_score)
                    {
                        branch_less_data[kvp.Key] = 0;
                        branch_more_data[kvp.Key] = kvp.Value;
                    }


                    for (int split_point = 0; split_point < rows - 1; split_point++)
                    {
                        var tup = tups[split_point];
                        float this_label = tup.Item1;
                        float this_value = tup.Item2;
                        branch_less_data[this_label] += tup.Item3;
                        branch_more_data[this_label] -= tup.Item3;
                        float next_value = tups[split_point + 1].Item2;

                        // Skip identical values.
                        float split_value = (this_value + next_value) / 2;
                        if ((this_value < split_value) == (next_value < split_value)) continue;

                        float correct = branch_less_data.Values.Max() + branch_more_data.Values.Max();

                        if (correct > max_correct)
                        {
                            max_correct = correct;
                            this._BranchSplitValue = split_value;
                            this._BranchColumn = c;
                            this._BranchLess = branch_less_data.ArgMax();
                            this._BranchMore = branch_more_data.ArgMax();
                        }
                    }
                }

                // Better options exist. We should split the branch!
                if (max_correct != max_correct_branch) return;
            }

            this._LeafClassification = branch_score.ArgMax();
            this._IsLeaf = true;
        }
    }


}
