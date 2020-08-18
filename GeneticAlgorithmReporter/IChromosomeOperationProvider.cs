using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithmReporter
{
    interface IChromosomeOperationProvider
    {
        object ObjectiveFunction(Chromosome chromosome);
        Gene MutateFunction(ref Gene gene);
        double GetChromosomeFitness(Chromosome chromosome);
        double GetChromosomeFitness(object objective);
    }
}
