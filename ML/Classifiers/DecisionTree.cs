using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Classifiers
{
    public class DecisionTree : Classifier
    {
        public bool _IsLeaf = false;

        public float _LeafClassification = 0;

        public int _BranchColumn;
        public float _BranchSplitValue;
        public DecisionTree _BranchLess;
        public DecisionTree _BranchMore;

        public DecisionTree(DataUseable train, int max_depth = -1)
            : this(train, max_depth, 0)
        {

        }

        private DecisionTree(DataUseable train, int max_depth, int current_depth)
        {
            var branch_score = train.getLabelCounts();

            int max_correct_branch = branch_score.Values.Max();

            if ((branch_score.Values.Sum() != max_correct_branch) && // All children are in one cluster.
                (max_depth != current_depth)) // Limit Levels
            {
                var tups = new Tuple<float, float>[train._Labels.Count];

                int cols = train._CountColumns;
                int rows = train._CountRows;

                int max_correct = max_correct_branch;
                int max_correct_column = -1;
                float max_correct_split = -1;

                for (int c = 0; c < cols; c++)
                {
                    for (int r = 0; r < rows; r++)
                        tups[r] = new Tuple<float, float>(train._Labels[r], train._Data[r, c]);

                    Array.Sort(tups, (Tuple<float, float> a, Tuple<float, float> b) =>
                        { return a.Item2.CompareTo(b.Item2); });

                    var branch_1_data = new Dictionary<float, int>();
                    var branch_2_data = new Dictionary<float, int>();

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
                        branch_1_data[this_label]++;
                        branch_2_data[this_label]--;
                        float next_value = tups[split_point + 1].Item2;

                        // Skip identical values.
                        float split_value = (this_value + next_value) / 2;
                        if ((this_value < split_value) == (next_value < split_value)) continue;

                        int correct = branch_1_data.Values.Max() + branch_2_data.Values.Max();

                        if (correct > max_correct)
                        {
                            max_correct = correct;
                            max_correct_split = split_value;
                            max_correct_column = c;
                        }
                    }
                }

                if (max_correct != max_correct_branch)
                {
                    this._BranchSplitValue = max_correct_split;
                    this._BranchColumn = max_correct_column;

                    DataUseable less, more;

                    train.Split(
                        this._BranchColumn,
                        this._BranchSplitValue,
                        out less,
                        out more);

                    this._BranchLess = new DecisionTree(less, max_depth, current_depth + 1);
                    this._BranchMore = new DecisionTree(more, max_depth, current_depth + 1);

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

        private float CompileF(float[] fs)
        {
            if (this._IsLeaf) return this._LeafClassification;
            else if (fs[this._BranchColumn] < this._BranchSplitValue) return this._BranchLess.CompileF(fs);
            else return this._BranchMore.CompileF(fs);
        }

        public Func<float[], float> Compile()
        {
            return (float[] fs) =>
            {
                return this.CompileF(fs);
            };            
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
