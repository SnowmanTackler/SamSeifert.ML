﻿using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Classifiers
{
    public class RandomForest : Classifier
    {
        private readonly DecisionTree[] _Trees;

        public readonly int _MaxDepth;

        /// <summary>
        /// How much of the data is used to train each tree.
        /// </summary>
        public readonly float _HoldOut;

        public int _TreeCount
        {
            get
            {
                return this._Trees.Length;
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="train"></param>
        /// <param name="max_depth"></param>
        /// <param name="tree_count"></param>
        /// <param name="hold_out">How much of the data is used to train each tree.</param>
        public RandomForest(
            int max_depth, 
            int tree_count,
            float hold_out = 0.1f)
        {
            this._Trees = new DecisionTree[tree_count];
            this._MaxDepth = max_depth;
            this._HoldOut = hold_out;
        }

        public void Train(DataUseable train)
        {
            int rows = train._CountRows;
            int cols = train._CountColumns;
            int count = Math.Max(2, (int)Math.Round(this._HoldOut * train._CountRows));

            Matrix<float> data = Matrix<float>.Build.Dense(count, cols);
            Vector<float> labels = Vector<float>.Build.Dense(count);
            var subset = new DataUseable(data, labels);
            Boolean[] bools = null;


            for (int i = 0; i < this._TreeCount; i++)
            {
                int good_dex = 0;
                int main_dex = 0;

                Util.PickRandom(count, rows - count, ref bools);
                foreach (var b in bools)
                {
                    if (b)
                    {
                        data.SetRow(good_dex, train._Data.Row(main_dex));
                        labels[good_dex] = train._Labels[main_dex];
                        good_dex++;
                    }
                    main_dex++;
                }

                this._Trees[i] = new DecisionTree(this._MaxDepth);
                this._Trees[i].Train(subset);
            }
        }

        public Func<float[], float> Compile()
        {
            int lens = this._Trees.Length;
            var funcs = new Func<float[], float>[lens];

            for (int i = 0; i < lens; i++)
                funcs[i] = this._Trees[i].Compile();

            Func<float[], float> func = (float[] fs) =>
            {
                var dict = new Dictionary<float, int>();

                for (int i = 0; i < lens; i++)
                {
                    float vote = funcs[i](fs);
                    int count;
                    if (!dict.TryGetValue(vote, out count)) count = 0;
                    dict[vote] = ++count;
                }

                var max_value = dict.Values.Max();

                // Leaf node!
                foreach (var kvp in dict)
                    if (max_value == kvp.Value)
                        return kvp.Key;

                String s = "CANT FIND MAX";
                Console.WriteLine(s);
                throw new Exception(s);
            };

            return func;
        }
    }
}
