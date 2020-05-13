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
                    GeneticAlgorithm algorithm = new GeneticAlgorithm(o.FileName, o.CrossoverRate, o.MutationRate, o.TotalGeneration, o.ReportPath);
                    algorithm.Execute();
                });
        }
    }
}
