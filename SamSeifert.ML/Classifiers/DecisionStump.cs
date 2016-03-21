using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSeifert.Utilities;

namespace SamSeifert.ML.Classifiers
{
    public class DecisionStump : Classifier
    {
        public bool _IsLeaf = false;
        public float _LeafClassification = 0;

        public int _BranchColumn;
        public float _BranchSplitValue;
        public float _BranchLessClassification = 0;
        public float _BranchMoreClassification = 0;

        public DecisionStump()
        {

        }

        public void Train(Datas.Useable train)
        {
            var branch_score = train.getLabelCounts();

            int max_correct_branch = branch_score.Values.Max();

            if (branch_score.Values.Sum() != max_correct_branch) // All children in one levels
            {
                var tups = new Tuple<float, float>[train._Labels.Count];

                int cols = train._CountColumns;
                int rows = train._CountRows;

                int best_correct = int.MinValue;

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

                        int correct = branch_less_data.Values.Max() + branch_more_data.Values.Max();

                        if (correct > best_correct)
                        {
                            best_correct = correct;
                            this._BranchSplitValue = split_value;
                            this._BranchColumn = c;
                            this._BranchLessClassification = branch_less_data.ArgMax();
                            this._BranchMoreClassification = branch_more_data.ArgMax();
                        }
                    }
                }

                if (best_correct != int.MinValue) return;
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
            else if (fs[this._BranchColumn] < this._BranchSplitValue) return this._BranchLessClassification;
            else return this._BranchMoreClassification;
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
                sb.Append("return ");
                sb.Append(this._BranchLessClassification);
                sb.Append("f;");
                sb.Append("}");
                sb.Append(Environment.NewLine);
                sb.Append("else {");
                sb.Append(Environment.NewLine);
                sb.Append("return ");
                sb.Append(this._BranchMoreClassification);
                sb.Append("f;");
                sb.Append("}");
                sb.Append(Environment.NewLine);
            }
        }
    }
}
