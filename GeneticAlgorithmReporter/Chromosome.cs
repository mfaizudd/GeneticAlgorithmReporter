using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithmReporter
{
    class Chromosome<T>
    {
        public T[] Genes { get; set; }
        public int Length => Genes.Length;

        public Chromosome() { }
        public Chromosome(T[] genes)
        {
            Genes = genes;
        }

        public override string ToString()
        {
            return $"[{string.Join(';', Genes)}]";
        }
    }
}
