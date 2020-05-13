using GeneticAlgorithmReporter.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace GeneticAlgorithmReporter
{
    class GeneticAlgorithm
    {
        double crossoverRate = .25;
        double mutationRate;
        int totalGeneration;
        string reportPath;
        int documentWidth = 30;
        string report;
        Chromosome<int>[] chromosomes;

        public GeneticAlgorithm(string filepath, double crossoverRate = .25, double mutationRate = .10, int totalGeneration = 10, string reportPath = "")
        {
            this.crossoverRate = crossoverRate;
            this.mutationRate = mutationRate;
            this.totalGeneration = totalGeneration;
            this.reportPath = reportPath;
            if (!File.Exists(filepath))
            {
                Console.WriteLine("Input file doesn't exists");
                return;
            }
            string json;
            using (var stream = new StreamReader(filepath))
            {
                json = stream.ReadToEnd();
            }
            chromosomes = JsonSerializer.Deserialize<Chromosome<int>[]>(json);
        }

        public void Execute()
        {
            for (int generation = 0; generation < totalGeneration; generation++)
            {
                double[] fObj = new double[chromosomes.Length];
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
                    chromosomes[i].ObjectiveFunction = chromosome =>
                    {
                        return Math.Abs((chromosome.Genes[0] + 2 * chromosome.Genes[1] + 3 * chromosome.Genes[2] + 4 * chromosome.Genes[3]) - 30);
                    };
                    chromosomes[i].MutateFunction = (chromosome, index) =>
                    {
                        Random random = new Random();
                        chromosome.Genes[index] = random.Next(1, 31);
                    };
                    fObj[i] = chromosomes[i].Evaluate();
                    fitnesses[i] = 1 / (1 + fObj[i]);
                    Print($"F_obj[{i + 1}] = {fObj[i]}");
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

                Chromosome<int>[] newChromosomes = new Chromosome<int>[chromosomes.Length];
                for (int i = 0; i < randoms.Length; i++)
                {
                    for (int j = 0; j < cumulativeProbabilities.Length; j++)
                    {
                        if (randoms[i] > cumulativeProbabilities[j])
                            continue;
                        newChromosomes[i] = new Chromosome<int>();
                        newChromosomes[i].Genes = new int[chromosomes[j].Genes.Length];
                        newChromosomes[i].ObjectiveFunction = chromosomes[j].ObjectiveFunction;
                        newChromosomes[i].MutateFunction = chromosomes[j].MutateFunction;
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
                    Dictionary<int, int[]> offsprings = new Dictionary<int, int[]>();
                    for (int i = 0; i < selectedIndexes.Length; i++)
                    {
                        int cut = random.Next(1, newChromosomes[selectedIndexes[i]].Genes.Length);
                        int targeti = i + 1 >= selectedIndexes.Length ? 0 : i + 1;
                        Print($"Cut:{cut}", centered: true);
                        Print($"Chromosome[{selectedIndexes[i] + 1}]><Chromosome[{selectedIndexes[targeti] + 1}]", centered: true);

                        var gene = newChromosomes[selectedIndexes[i]].Genes;
                        var targetgene = newChromosomes[selectedIndexes[targeti]].Genes;
                        Print($"[{string.Join(';', gene)}]><[{string.Join(';', targetgene)}]", centered: true);

                        int[] offspring = new int[gene.Length];
                        for (int j = 0; j < offspring.Length; j++)
                        {
                            offspring[j] = gene[j];
                            if (j >= cut)
                            {
                                offspring[j] = targetgene[j];
                            }
                        }
                        Print($"[{string.Join(';', offspring)}]", centered: true);
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
                    newChromosomes[item.x].Mutate(item.y);
                    Print($"Mutated Gene[{item.x + 1}, {item.y}]: {newChromosomes[item.x].Genes[item.y]}");
                }
                Print();
                chromosomes = newChromosomes;

                Print("Mutated Chromosomes");
                PrintChromosomes(chromosomes);
                Print();

                Print("New Generation Objective Function");
                int min = chromosomes[0].Evaluate();
                int minIndex = 0;
                for (int i = 0; i < chromosomes.Length; i++)
                {
                    var eval = chromosomes[i].Evaluate();
                    if (eval < min)
                    {
                        min = eval;
                        minIndex = i;
                    }
                    Print($"Chromosome[{i + 1}]={chromosomes[i]}");
                    Print($"F_obj[{i + 1}]={eval}");
                    Print();
                }
                Print();

                Print($"New best Chromosome[{minIndex + 1}] with F_obj of {min}");
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

        private void PrintChromosomes(Chromosome<int>[] newChromosomes)
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
