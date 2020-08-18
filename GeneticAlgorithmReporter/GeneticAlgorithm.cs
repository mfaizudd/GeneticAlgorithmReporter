using GeneticAlgorithmReporter.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace GeneticAlgorithmReporter
{
    class GeneticAlgorithm<T>
    {
        double crossoverRate = .25;
        double mutationRate;
        int totalGeneration;
        string reportPath;
        int documentWidth = 30;
        string report;
        Chromosome[] chromosomes;

        public IChromosomeOperationProvider operationProvider { get; }

        public GeneticAlgorithm(string filepath, IChromosomeOperationProvider operationProvider, double crossoverRate = .25, double mutationRate = .10, int totalGeneration = 10, string reportPath = "")
        {
            this.crossoverRate = crossoverRate;
            this.mutationRate = mutationRate;
            this.totalGeneration = totalGeneration;
            this.reportPath = reportPath;
            this.operationProvider = operationProvider;

            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException($"Input file({filepath}) doesn't exists");
            }
            string json;
            using (var stream = new StreamReader(filepath))
            {
                json = stream.ReadToEnd();
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new GeneConverter());
            chromosomes = JsonSerializer.Deserialize<Chromosome[]>(json, serializeOptions);
        }

        public void Execute()
        {
            for (int generation = 0; generation < totalGeneration; generation++)
            {
                object[] objectives = new object[chromosomes.Length];
                double[] fitnesses = new double[chromosomes.Length];
                double[] probabilities = new double[chromosomes.Length];
                double[] cumulativeProbabilities = new double[chromosomes.Length];
                double[] randoms = new double[chromosomes.Length];
                double totalFitness = 0;

                Print($"Initialize Generation #{generation+1}", tag: "h2");
                PrintChromosomes(chromosomes);
                Print();

                // Evaluation
                Print("Evaluation", tag: "h3");
                for (int i = 0; i < chromosomes.Length; i++)
                {
                    //Console.WriteLine($"Chromosome[{i+1}]: {chromosomes[i]}");
                    objectives[i] = operationProvider.ObjectiveFunction(chromosomes[i]);
                    fitnesses[i] = operationProvider.GetChromosomeFitness(objectives[i]);
                    Print($"F_obj[{i + 1}] = {objectives[i]}");
                    totalFitness += fitnesses[i];
                }

                //Fitness
                for (int i = 0; i < fitnesses.Length; i++)
                {
                    Print($"Fitness[{i + 1}] = {fitnesses[i]}");
                }
                Print($"Total fitness = {totalFitness}");
                Print();

                Random random = new Random();

                Print("Probabilities");
                for (int i = 0; i < chromosomes.Length; i++)
                {
                    probabilities[i] = fitnesses[i] / totalFitness;
                    Print($"P[{i + 1}]: {probabilities[i]}");
                    cumulativeProbabilities[i] = probabilities.Sum();
                    randoms[i] = random.NextDouble();
                    //Console.WriteLine($"C[{i + 1}]{cumulativeProbabilities[i]}");
                }
                Print();

                Print("Cumulative probabilities");
                for (int i = 0; i < chromosomes.Length; i++)
                {
                    Print($"C[{i + 1}]: {cumulativeProbabilities[i]}");
                }
                Print();

                Print("Roulette-wheel", tag: "h3");
                for (int i = 0; i < chromosomes.Length; i++)
                {
                    Print($"R[{i + 1}]: {randoms[i]}");
                }
                Print();

                Chromosome[] newChromosomes = new Chromosome[chromosomes.Length];
                for (int i = 0; i < randoms.Length; i++)
                {
                    for (int j = 0; j < cumulativeProbabilities.Length; j++)
                    {
                        if (randoms[i] > cumulativeProbabilities[j])
                            continue;
                        newChromosomes[i] = new Chromosome();
                        newChromosomes[i].Genes = new Gene[chromosomes[j].Genes.Length];
                        chromosomes[j].Genes.CopyTo(newChromosomes[i].Genes, 0);
                        break;
                    }
                }

                Print("New selected chromosomes:", tag: "h3");
                PrintChromosomes(newChromosomes);
                Print();

                // Crossover
                int[] selectedIndexes = SelectChromosomes(chromosomes.Length).ToArray();

                Print("Crossover", tag: "h3", centered: true);
                //Console.WriteLine($"Indexcount:{selectedIndexes.Length}");
                if (selectedIndexes.Length > 1)
                {
                    Dictionary<int, Gene[]> offsprings = new Dictionary<int, Gene[]>();
                    for (int i = 0; i < selectedIndexes.Length; i++)
                    {
                        int cut = random.Next(1, newChromosomes[selectedIndexes[i]].Genes.Length);
                        int targeti = i + 1 >= selectedIndexes.Length ? 0 : i + 1;
                        Print($"Cut:{cut}", centered: true);
                        Print($"Chromosome[{selectedIndexes[i] + 1}]><Chromosome[{selectedIndexes[targeti] + 1}]", centered: true);

                        var genes = newChromosomes[selectedIndexes[i]].Genes;
                        var targetGenes = newChromosomes[selectedIndexes[targeti]].Genes;
                        Print($"[{string.Join(';', genes.Cast<object>())}]><[{string.Join(';', targetGenes.Cast<object>())}]", centered: true);

                        Gene[] offspring = new Gene[genes.Length];
                        for (int j = 0; j < offspring.Length; j++)
                        {
                            offspring[j] = genes[j];
                            if (j >= cut)
                            {
                                offspring[j] = targetGenes[j];
                            }
                        }
                        Print($"[{string.Join(';', offspring.Cast<object>())}]", centered: true);
                        Print();
                        offsprings.Add(selectedIndexes[i], offspring);
                    }
                    Print();

                    foreach (var item in offsprings)
                    {
                        item.Value.CopyTo(newChromosomes[item.Key].Genes, 0);
                    }

                    Print("New Generation Chromosomes");
                    PrintChromosomes(newChromosomes);
                }
                else
                {
                    Print("No crossover");
                }
                Print();

                // Mutation
                Print("Mutation", tag: "h3");
                (int x, int y)[] selectedGenes = SelectGeneForMutation(newChromosomes.Length, newChromosomes[0].Genes.Length).ToArray();
                foreach (var item in selectedGenes)
                {
                    operationProvider.MutateFunction(ref newChromosomes[item.x].Genes[item.y]);
                    Print($"Mutated Gene[{item.x + 1}, {item.y}]: {newChromosomes[item.x].Genes[item.y]}");
                }
                Print();
                chromosomes = newChromosomes;

                Print("Mutated Chromosomes");
                PrintChromosomes(chromosomes);
                Print();

                Print("New Generation Objective Function");
                double max = operationProvider.GetChromosomeFitness(chromosomes[0]);
                int maxIndex = 0;
                for (int i = 0; i < chromosomes.Length; i++)
                {
                    var eval = operationProvider.GetChromosomeFitness(chromosomes[i]);
                    if (eval > max)
                    {
                        max = eval;
                        maxIndex = i;
                    }
                    Print($"Chromosome[{i + 1}]={chromosomes[i]}");
                    Print($"Fitness[{i + 1}]={eval}");
                    Print();
                }
                Print();

                Print($"New best Chromosome[{maxIndex + 1}] with fitness of {max}");
                Print();
            }

            if (reportPath != "")
            {
                using (var writer = new StreamWriter(reportPath))
                {
                    writer.Write(report);
                }
            }
        }

        private void PrintChromosomes(Chromosome[] newChromosomes)
        {
            for (int i = 0; i < chromosomes.Length; i++)
            {
                Print($"Chromosome[{i + 1}]: {newChromosomes[i]}");
            }
        }

        IEnumerable<int> SelectChromosomes(int length)
        {
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                double randomedDouble = random.NextDouble();
                if (randomedDouble < crossoverRate)
                {
                    yield return i;
                }
            }
        }

        IEnumerable<(int, int)> SelectGeneForMutation(int chromosomeLength, int geneLength)
        {
            Random random = new Random();
            for (int i = 0; i < chromosomeLength; i++)
            {
                for (int k = 0; k < geneLength; k++)
                {
                    if (random.NextDouble() < mutationRate)
                        yield return (i, k);
                }
            }
        }

        public void Print(string text = "", string tag = "", bool centered = false)
        {
            string extension = Path.GetExtension(reportPath);

            if (centered)
                text = text.CenteredString(documentWidth);

            switch (extension)
            {
                case ".html":
                    string element = text;

                    element = element.Replace("<", "&lt;");
                    element = element.Replace(">", "&gt;");

                    if (tag != "")
                        element = $"<{tag}>{element}</{tag}>";
                    else
                        element = $"<p>{element}</p>";

                    if (centered)
                        element = $"<div style=\"text-align:center\">{element}</div>";

                    report += $"{element}\n";
                    break;
                default:
                    report = $"{text}\n";
                    break;
            }

            Console.Write($"{text}\n");
        }
    }
}
