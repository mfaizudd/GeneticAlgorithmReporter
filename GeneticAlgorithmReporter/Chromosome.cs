using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithmReporter
{
    class Chromosome<T>
    {
        public T[] Genes { get; set; }
        public int Length => Genes.Length;

        public Func<Chromosome<T>, T> ObjectiveFunction { get; set; }
        public Action<Chromosome<T>, int> MutateFunction { get; set; }

        public Chromosome() { }
        public Chromosome(T[] genes)
        {
            Genes = genes;
        }

        public T Evaluate()
        {
            return ObjectiveFunction.Invoke(this);
        }

        public void Mutate(int index)
        {
            MutateFunction?.Invoke(this, index);
        }

        public override string ToString()
        {
            return $"[{string.Join(';', Genes)}]";
        }
    }
}
