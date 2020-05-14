using System;
using CommandLine;

namespace GeneticAlgorithmReporter
{
    class Program
    {
        public class Options
        {
            [Option('f', "filename", Required = true, HelpText = "Input files to be processed")]
            public string FileName { get; set; }


            [Option('c', "crossover", HelpText = "The rate of crossover", Default = .25)]
            public double CrossoverRate { get; set; }

            [Option('m', "mutation", HelpText = "The rate of mutation", Default = .10)]
            public double MutationRate { get; set; }


            [Option('r', "report", HelpText = "Generate report, specify the name of the report that will be generated (html, txt)", Default = "")]
            public string ReportPath { get; set; }


            [Option('g', "generation", HelpText = "The total generation, default is 10", Default = 10)]
            public int TotalGeneration { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    GeneticAlgorithm<double> algorithm = new GeneticAlgorithm<double>(o.FileName, o.CrossoverRate, o.MutationRate, o.TotalGeneration, o.ReportPath);
                    algorithm.ObjectiveFunction = chromosome =>
                    {
                        return Math.Abs((chromosome.Genes[0] + 2 * chromosome.Genes[1] + 3 * chromosome.Genes[2] + 4 * chromosome.Genes[3]) - 30);
                    };
                    algorithm.MutateFunction = (chromosome, index) =>
                    {
                        Random random = new Random();
                        chromosome.Genes[index] = random.Next(1, 31);
                    };
                    algorithm.Execute();
                });
        }
    }
}
