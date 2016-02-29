using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Classifiers.AdaBoostClassifiers
{
    public class DecisionStump : DecisionTree
    {
        public DecisionStump() : base(1)
        {

        }
    }

    public class DecisionTree : AdaBoostClassifier
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

        private DecisionTree(int depth, int max_depth )
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


        public void Train(DataUseable data, float[] weights)
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

                float max_correct = max_correct_branch;
                int max_correct_column = -1;
                float max_correct_split = -1;

                for (int c = 0; c < cols; c++)
                {
                    for (int r = 0; r < rows; r++)
                        tups[r] = new Tuple<float, float, float>(
                            data._Labels[r], 
                            data._Data[r, c],
                            weights[r]);

                    Array.Sort(tups, (Tuple<float, float, float> a, Tuple<float, float, float> b) =>
                    { return a.Item2.CompareTo(b.Item2); });

                    var branch_1_data = new Dictionary<float, float>();
                    var branch_2_data = new Dictionary<float, float>();

                    foreach (var kvp in branch_score)
                    {
                        branch_1_data[kvp.Key] = 0;
                        branch_2_data[kvp.Key] = kvp.Value;
                    }


                    for (int split_point = 0; split_point < rows - 1; split_point++)
                    {
                        var tup = tups[split_point];
                        float this_label = tup.Item1;
                        float this_value = tup.Item2;
                        branch_1_data[this_label] += tup.Item3;
                        branch_2_data[this_label] -= tup.Item3;
                        float next_value = tups[split_point + 1].Item2;

                        // Skip identical values.
                        float split_value = (this_value + next_value) / 2;
                        if ((this_value < split_value) == (next_value < split_value)) continue;

                        float correct = branch_1_data.Values.Max() + branch_2_data.Values.Max();

                        if (correct > max_correct)
                        {
                            max_correct = correct;
                            max_correct_split = split_value;
                            max_correct_column = c;
                        }
                    }
                }

                // Better options exist. We should split the branch!
                if (max_correct != max_correct_branch)
                {
                    this._BranchSplitValue = max_correct_split;
                    this._BranchColumn = max_correct_column;

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
                        (branch_less_data.Values.NNZ() == branch_less_data.Count - 1))
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

                        DataUseable less = new DataUseable(data_less, labels_less);
                        this._BranchLess = new DecisionTree(this._Depth + 1, this._MaxDepth);
                        this._BranchLess.Train(less, weights_less);
                    }

                    // Only split data more if we need too!
                    if ((this._Depth == this._MaxDepth - 1) ||
                        (branch_more_data.Values.NNZ() == branch_more_data.Count - 1))
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

                        DataUseable more = new DataUseable(data_more, labels_more);
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
