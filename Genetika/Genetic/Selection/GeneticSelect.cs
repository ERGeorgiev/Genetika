using Genetika.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Genetika.Genetic.Selection
{
    public static class GeneticSelect<T>
        where T : class, IEntity<T>
    {
        public static Gene<T> Gene(Genome<T> genome, SelectionType selectionType)
        {
            switch (selectionType)
            {
                case SelectionType.Random:
                    return Random(genome.genes);
                case SelectionType.RouletteWheel:
                    return RouletteWheel(genome);
                case SelectionType.Tournament:
                    return Tournament(genome);
                default:
                    return default;
            }
        }

        public static Gene<T> Random(List<Gene<T>> genes)
        {
            if (genes?.Count <= 0)
                return default;

            int index = GenetikaParameters.random.Next(0, genes.Count);
            return genes[index];
        }

        public static Gene<T> RouletteWheel(Genome<T> genome)
        {
            if (genome.genes.Any() == false)
                return default;

            float totalScore = 0;
            Dictionary<Gene<T>, float> pieLedger = new Dictionary<Gene<T>, float>();

            foreach (Gene<T> gene in genome.genes)
            {
                totalScore += gene.Fitness;
                pieLedger.Add(gene, totalScore);
            }

            float chosen = (float)GenetikaParameters.random.NextDouble() * totalScore;
            foreach (Gene<T> gene in pieLedger.Keys)
            {
                if (pieLedger[gene] >= chosen)
                    return gene;
            }

            return pieLedger.Last().Key;
        }

        public static Gene<T> Tournament(Genome<T> genome)
        {
            if (genome.genes.Any() == false)
                return default;

            Gene<T> geneA = Random(genome.genes);
            Gene<T> geneB = Random(genome.genes);

            if (genome.FitnessCompare(geneA.Fitness, geneB.Fitness) >= 0)
                return geneA;
            else
                return geneB;
        }
    }
}
