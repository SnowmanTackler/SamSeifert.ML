﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace ML.Classifiers.BoostableClassifiers
{
    public interface BoostableClassifier
    {
        void Train(DataUseable indata, float[] weights);
        float Predict(float[] parameters);
    }
}