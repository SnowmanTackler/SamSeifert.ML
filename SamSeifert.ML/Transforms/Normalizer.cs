using SamSeifert.MathNet.Numerics.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SamSeifert.ML.Datas;

namespace SamSeifert.ML.Transforms
{
    public static class Normalizer
    {
        internal static void Normalize(
            Datas.Useable[] indata,
            out Datas.Useable[] outdata)
        {
            var train = indata[0];

            var train_means = train._Data.MeanRow();

            var new_train_data = train._Data.AddRow(-train_means);

            var stds = new_train_data.StandardDeviationRow(true);
            for (int i = 0; i < stds.Count; i++)
            {
                if ((stds[i] == 0) || float.IsNaN(stds[i]) || float.IsInfinity(stds[i])) stds[i] = 1;
                else stds[i] = 1 / stds[i];
            }

            new_train_data.MultiplyRowT(stds);

            outdata = new Useable[indata.Length];
            outdata[0] = new Useable(new_train_data, train._Labels);

            for (int i = 1; i < indata.Length; i++)
            {
                var dat = indata[i];
                var new_test_data = dat._Data.AddRow(-train_means);
                new_test_data.MultiplyRowT(stds);
                outdata[i] = new Useable(new_test_data, dat._Labels);
            }
        }
    }
}
