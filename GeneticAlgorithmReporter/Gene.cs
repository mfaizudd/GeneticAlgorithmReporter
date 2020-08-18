﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithmReporter
{
    class Gene
    {
        public object value;

        public T GetValue<T>()
        {
            if (value is T)
                return (T)value;
            return default(T);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}