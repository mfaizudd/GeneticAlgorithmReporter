using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithmReporter
{
    struct Gene
    {
        public object value;

        public T GetValue<T>()
        {
            if (value is T)
                return (T)value;
            return default;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
