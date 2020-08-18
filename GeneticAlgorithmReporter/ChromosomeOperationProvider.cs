using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithmReporter
{
    class ChromosomeOperationProvider : IChromosomeOperationProvider
    {
        Random random = new Random();

        public ChromosomeOperationProvider()
        {

        }

        public double GetChromosomeFitness(Chromosome chromosome)
        {
            var objective = ObjectiveFunction(chromosome);
            return GetChromosomeFitness(objective);
        }

        public double GetChromosomeFitness(object objective)
        {
            var fitness = 1 / (1 + (double)objective);
            return fitness;
        }

        public Gene MutateFunction(ref Gene gene)
        {
            gene.value = random.Next(1, 31);
            return gene;
        }

        public object ObjectiveFunction(Chromosome chromosome)
        {
            var objective = Math.Abs((chromosome.Genes[0].GetValue<double>() + 2 * chromosome.Genes[1].GetValue<double>() + 3 * chromosome.Genes[2].GetValue<double>() + 4 * chromosome.Genes[3].GetValue<double>()) - 30);
            return objective;
        }
    }
}
