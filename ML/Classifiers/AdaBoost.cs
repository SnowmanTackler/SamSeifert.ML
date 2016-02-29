using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdaBoostClassifier = ML.Classifiers.AdaBoostClassifiers.AdaBoostClassifier;

namespace ML.Classifiers
{
    public class AdaBoost : Classifier
    {
        private readonly int _Length;
        private readonly AdaBoostClassifier[] _Classifiers;
        private readonly float[] _ClassifierWeights;

        public AdaBoost(DataUseable train, DataUseable test, int count, Func<AdaBoostClassifier> Factory )
        {
            int rows = train._CountRows;
            int cols = train._CountColumns;

            var weights = new float[rows];
            var predictions = new bool[rows];
            var parameters = new float[cols];

            for (int r = 0; r < rows; r++)
                weights[r] = 1.0f / rows;

            this._Classifiers = new AdaBoostClassifier[count];
            this._ClassifierWeights = new float[count];
            this._Length = count;

            for (int i = 1; i <= count; i++)
            {
                var classy = Factory();

                classy.Train(train, weights);

                float error = 0;

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                        parameters[c] = train._Data[r, c];

                    predictions[r] = train._Labels[r] == classy.Predict(parameters);
                    if (!predictions[r]) error += weights[r];
                }

                if (error == 0)
                {
                    String err = "Adaboost Error is 0";
                    Console.WriteLine(err);
                    throw new Exception(err);
                }

                float alpha = 0.5f * (float)Math.Log((1 - error) / error);

                float sum_weights = 0;
                for (int r = 0; r < rows; r++)
                {
                    float learning = 1 * (predictions[r] ? 5 : -5);
                    float new_weight = weights[r] * (float)Math.Exp(-alpha * learning);
                    if (float.IsInfinity(new_weight) ||
                        float.IsNaN(new_weight) ||
                        (new_weight == 0))
                    {
                        // Don't update
                    }
                    else
                    {
                        weights[r] = new_weight;
                    }
                    sum_weights += weights[r];
                }
                for (int r = 0; r < rows; r++)
                    weights[r] /= sum_weights;

                // Console.WriteLine("\tAlpha: " + alpha);
                // Console.WriteLine("\tMin: " + weights.Min());
                // Console.Write("\tMax: " + weights.Max() + '\t');
                // Console.WriteLine();

                this._ClassifierWeights[i - 1] = alpha;
                this._Classifiers[i - 1] = classy;

                var func = this.Compile(i);
                var conf_train = new ConfusionMatrix(func, train);
                var conf_test = new ConfusionMatrix(func, test);

                Form1.Instance.WriteLine("% Ada Boost: " + i + " boosts");
                Form1.Instance.WriteLine("\t" + i + ", " + conf_train.Accuracy + ", " + conf_test.Accuracy + ";");

            }
        }


        public Func<float[], float> Compile()
        {
            return this.Compile(this._Length);
        }

        private Func<float[], float> Compile(int lens)
        {
            return (float[] fs) =>
            {
                var dict = new Dictionary<float, float>();

                for (int i = 0; i < lens; i++)
                {
                    float vote = this._Classifiers[i].Predict(fs);
                    float running_sum;
                    if (!dict.TryGetValue(vote, out running_sum)) running_sum = 0;
                    dict[vote] = running_sum + this._ClassifierWeights[i];
                }

                return dict.ArgMax();
            };

        }
    }
}
