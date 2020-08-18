using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticAlgorithmReporter
{
    class Chromosome
    {
        public Gene[] Genes { get; set; }
        public int Length => Genes.Length;

        public Chromosome() { }
        public Chromosome(Gene[] genes)
        {
            Genes = genes;
        }

        public override string ToString()
        {
            return $"[{string.Join(';', Genes.Select(x=>x.value.ToString()))}]";
        }
    }
}
