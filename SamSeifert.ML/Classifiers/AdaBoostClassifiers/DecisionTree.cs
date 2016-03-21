using MathNet.Numerics.LinearAlgebra;
using SamSeifert.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.ML.Classifiers.BoostableClassifiers
{
    public class DecisionTree : BoostableClassifier
    {
        public bool _IsLeaf = false;

        public float _LeafClassification = 0;

        public int _BranchColumn;
        public float _BranchSplitValue;
        public DecisionTree _BranchLess;
        public DecisionTree _BranchMore;

        private readonly int _Depth;
        private readonly int _MaxDepth;

        public DecisionTree(int max_depth)
            : this(0, max_depth)
        {
        }

        private DecisionTree(int depth, int max_depth)
        {
            this._Depth = depth;
            this._MaxDepth = max_depth;
        }

        private DecisionTree(float val)
        {
            this._IsLeaf = true;
            this._LeafClassification = val;
        }

        public float Predict(float[] fs)
        {
            if (this._IsLeaf) return this._LeafClassification;
            else if (fs[this._BranchColumn] < this._BranchSplitValue) return this._BranchLess.Predict(fs);
            else return this._BranchMore.Predict(fs);
        }

        public void Train(Datas.Useable data, float[] weights)
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

            if ((branch_score.Values.Sum() != max_correct_branch) && // All children are in one cluster.
                (this._Depth != _MaxDepth)) // Limit Levels
            {
                int cols = data._CountColumns;
                int rows = data._CountRows;

                var tups = new Tuple<float, float, float>[rows];

                double best_entropy = double.MaxValue;
                int best_column = -1;
                float best_split = -1;

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

                        double p_less = (split_point + 1.0) / rows;
                        double p_more = 1 - p_less;
                        double entropy = p_less * branch_less_data.Values.Entropy() +
                                         p_more * branch_more_data.Values.Entropy();

                        if (entropy < best_entropy)
                        {
                            best_entropy = entropy;
                            best_split = split_value;
                            best_column = c;
                        }
                    }
                }

                // Better options exist. We should split the branch!
                if (best_entropy != double.MaxValue)
                {
                    this._BranchSplitValue = best_split;
                    this._BranchColumn = best_column;

                    // Console.WriteLine("Best Split: "+ this._BranchSplitValue+ " on column:"+ this._BranchColumn);

                    int count_less = 0;
                    int count_more = 0;

                    var branch_less_data = new Dictionary<float, float>();
                    var branch_more_data = new Dictionary<float, float>();

                    for (int r = 0; r < rows; r++)
                    {
                        if (data._Data[r, this._BranchColumn] < this._BranchSplitValue)
                        {
                            count_less++;

                            float f = data._Labels[r];
                            float count;
                            if (!branch_less_data.TryGetValue(f, out count)) count = 0;
                            branch_less_data[f] = count + weights[r];
                        }
                        else
                        {
                            count_more++;

                            float f = data._Labels[r];
                            float count;
                            if (!branch_more_data.TryGetValue(f, out count)) count = 0;
                            branch_more_data[f] = count + weights[r];
                        }
                    }

                    // Only split data less if we need too!
                    if ((this._Depth == this._MaxDepth - 1) ||
                        (branch_less_data.Values.NumberOfNonZeros() == branch_less_data.Count - 1))
                    {
                        this._BranchLess = new DecisionTree(branch_less_data.ArgMax());
                    }
                    else
                    {
                        var labels_less = Vector<float>.Build.Dense(count_less);
                        var weights_less = new float[count_less];
                        var data_less = Matrix<float>.Build.Dense(count_less, cols);
                        int dex_less = 0;

                        for (int r = 0; r < rows; r++)
                        {
                            if (data._Data[r, this._BranchColumn] < this._BranchSplitValue)
                            {
                                data_less.SetRow(dex_less, data._Data.Row(r));
                                labels_less[dex_less] = data._Labels[r];
                                weights_less[dex_less] = weights[r];
                                dex_less++;
                            }
                        }

                        Datas.Useable less = new Datas.Useable(data_less, labels_less);
                        this._BranchLess = new DecisionTree(this._Depth + 1, this._MaxDepth);
                        this._BranchLess.Train(less, weights_less);
                    }

                    // Only split data more if we need too!
                    if ((this._Depth == this._MaxDepth - 1) ||
                        (branch_more_data.Values.NumberOfNonZeros() == branch_more_data.Count - 1))
                    {
                        this._BranchMore = new DecisionTree(branch_more_data.ArgMax());
                    }
                    else
                    {
                        var labels_more = Vector<float>.Build.Dense(count_more);
                        var weights_more = new float[count_more];
                        var data_more = Matrix<float>.Build.Dense(count_more, cols);
                        int dex_more = 0;

                        for (int r = 0; r < rows; r++)
                        {
                            if (data._Data[r, this._BranchColumn] >= this._BranchSplitValue)
                            {
                                data_more.SetRow(dex_more, data._Data.Row(r));
                                labels_more[dex_more] = data._Labels[r];
                                weights_more[dex_more] = weights[r];
                                dex_more++;
                            }
                        }

                        Datas.Useable more = new Datas.Useable(data_more, labels_more);
                        this._BranchMore = new DecisionTree(this._Depth + 1, this._MaxDepth);
                        this._BranchMore.Train(more, weights_more);
                    }

                    return;
                }
            }

            this._LeafClassification = branch_score.ArgMax();
            this._IsLeaf = true;
        }
    }
}
