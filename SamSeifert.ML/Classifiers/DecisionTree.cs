using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSeifert.Utilities;

namespace SamSeifert.ML.Classifiers
{
    public class DecisionTree : Classifier
    {
        public bool _IsLeaf = false;

        public float _LeafClassification = 0;

        public int _BranchColumn;
        public float _BranchSplitValue;
        public DecisionTree _BranchLess;
        public DecisionTree _BranchMore;

        private readonly int _MaxDepth = 0;

        public DecisionTree(int max_depth = -1)
        {
            this._MaxDepth = max_depth;
        }

        public void Train(Datas.Useable train)
        {
            this.Train(train, 0);
        }

        public void Train(Datas.Useable train, int current_depth)
        {
            var branch_score = train.getLabelCounts();

            int max_correct_branch = branch_score.Values.Max();

            if ((branch_score.Values.Sum() != max_correct_branch) && // All children are in one cluster.
                (this._MaxDepth != current_depth)) // Limit Levels
            {
                var tups = new Tuple<float, float>[train._Labels.Count];

                int cols = train._CountColumns;
                int rows = train._CountRows;

                double best_entropy = double.MaxValue;
                int best_column = -1;
                float best_split = -1;

                for (int c = 0; c < cols; c++)
                {
                    for (int r = 0; r < rows; r++)
                        tups[r] = new Tuple<float, float>(train._Labels[r], train._Data[r, c]);

                    Array.Sort(tups, (Tuple<float, float> a, Tuple<float, float> b) =>
                        { return a.Item2.CompareTo(b.Item2); });

                    var branch_less_data = new Dictionary<float, int>();
                    var branch_more_data = new Dictionary<float, int>();

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
                        branch_less_data[this_label]++;
                        branch_more_data[this_label]--;
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

                if (best_column != -1)
                {
                    this._BranchSplitValue = best_split;
                    this._BranchColumn = best_column;

                    Datas.Useable less, more;

                    train.Split(
                        this._BranchColumn,
                        this._BranchSplitValue,
                        out less,
                        out more);

                    this._BranchLess = new DecisionTree(this._MaxDepth);
                    this._BranchMore = new DecisionTree(this._MaxDepth);

                    this._BranchLess.Train(less, current_depth + 1);
                    this._BranchMore.Train(more, current_depth + 1);

                    return;
                }
            }

            this._LeafClassification = branch_score.ArgMax();
            this._IsLeaf = true;
        }


        public Func<float[], float> CompileCode()
        {
            String err;

            var sb = new StringBuilder();

            sb.Append("public static float Function(float[] fs) {");
            sb.Append(Environment.NewLine);
            this.CompileText(sb);
            sb.Append("}");

            var mi = Compiler.Compile(sb.ToString(), out err);

            if (err != null)
            {
                throw new Exception(err);
            }
            else
            {
                return Delegate.CreateDelegate(typeof(Func<float[], float>), mi) as Func<float[], float>;
            }
        }

        public float Compile(float[] fs)
        {
            if (this._IsLeaf) return this._LeafClassification;
            else if (fs[this._BranchColumn] < this._BranchSplitValue) return this._BranchLess.Compile(fs);
            else return this._BranchMore.Compile(fs);
        }

        private void CompileText(StringBuilder sb)
        {
            if (this._IsLeaf)
            {
                sb.Append("return ");
                sb.Append(this._LeafClassification);
                sb.Append("f;");
                sb.Append(Environment.NewLine);
            }
            else
            {
                sb.Append("if (fs[");
                sb.Append(this._BranchColumn);
                sb.Append("] < ");
                sb.Append(this._BranchSplitValue);
                sb.Append(") {");
                sb.Append(Environment.NewLine);
                this._BranchLess.CompileText(sb);
                sb.Append("}");
                sb.Append(Environment.NewLine);
                sb.Append("else {");
                sb.Append(Environment.NewLine);
                this._BranchMore.CompileText(sb);
                sb.Append("}");
                sb.Append(Environment.NewLine);
            }
        }
    }
}
