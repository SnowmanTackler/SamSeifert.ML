using SamSeifert.MathNet.Numerics.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.ML.Transforms
{
    public static class Normalizer
    {
        internal static void Normalize(
            Data.Useable train_in, 
            Data.Useable test_in,
            out Data.Useable train, 
            out Data.Useable test)
        {
            var train_means = train_in._Data.MeanRow();

            var new_train_data = train_in._Data.AddRow(-train_means);
            var new_test_data = test_in._Data.AddRow(-train_means);

            var stds = new_train_data.StandardDeviationRow(true);
            for (int i = 0; i < stds.Count; i++)
            {
                if ((stds[i] == 0) || float.IsNaN(stds[i]) || float.IsInfinity(stds[i])) stds[i] = 1;
                else stds[i] = 1 / stds[i];
            }

            new_train_data.MultiplyRowT(stds);
            new_test_data.MultiplyRowT(stds);

            train = new Data.Useable(new_train_data, train_in._Labels);
            test = new Data.Useable(new_test_data, test_in._Labels);
        }
    }
}
