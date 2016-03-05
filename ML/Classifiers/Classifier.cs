using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Classifiers
{
    public interface Classifier
    {
        Func<float[], float> Compile();
        void Train(DataUseable indata);
    }
}
