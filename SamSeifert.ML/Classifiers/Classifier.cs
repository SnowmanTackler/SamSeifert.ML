using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.ML.Classifiers
{
    public interface Classifier
    {
        float Compile(float[] f);
        void Train(Data.Useable indata);
    }
}
