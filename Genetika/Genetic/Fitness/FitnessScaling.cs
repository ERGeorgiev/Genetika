using System;
using System.Collections.Generic;
using System.Linq;
using Genetika.Interfaces;

namespace Genetika.Genetic.Fitness
{
    public static class FitnessScaling<T>
        where T : class, IEntity<T>
    {
        public static void ScaleFitness(Genome<T> genome, FitnessScalingType scalingType)
        {
            if (genome.genes.Any() == false) return;

            switch (scalingType)
            {
                case FitnessScalingType.SigmaTruncation:
                    SigmaTruncation(genome.genes);
                    break;
                case FitnessScalingType.Rank:
                    Rank(genome);
                    break;
                case FitnessScalingType.None:
                    return;
            }
        }

        public static float GetFitnessAverage(List<Gene<T>> genes)
        {
            if (genes?.Count == 0) return 0;

            float fitnessAverage = 0;

            foreach (Gene<T> gene in genes)
                fitnessAverage += gene.Fitness;
            fitnessAverage /= genes.Count;

            return fitnessAverage;
        }

        public static float GetFitnessDeviation(List<Gene<T>> genes, float fitnessAverage)
        {
            if (genes?.Count == 0) return 0;

            float fitnessDeviation = 0;

            foreach (Gene<T> gene in genes)
                fitnessDeviation += Math.Abs(fitnessAverage - gene.Fitness);
            fitnessDeviation /= genes.Count;

            return fitnessDeviation;
        }

        private static float SigmaTruncation(float fitness, float fitnessAverage, float fitnessDeviation)
        {
            return Math.Max(fitness - (fitnessAverage - 2 * fitnessDeviation), 0);
        }

        private static void SigmaTruncation(List<Gene<T>> genes)
        {
            float fitnessAverage = GetFitnessAverage(genes);
            float fitnessDeviation = GetFitnessDeviation(genes, fitnessAverage);

            foreach (Gene<T> gene in genes)
                gene.Fitness = SigmaTruncation(gene.Fitness, fitnessAverage, fitnessDeviation);
        }

        /// <summary>
        /// Assigns a rank to all genes starting from 1 and increasing by 1.
        /// The higher the fitness, the higher the rank.
        /// </summary>
        /// <param name="genes">Genes to rank.</param>
        private static void Rank(Genome<T> genome)
        {
            if (genome.genes.Any() == false)
            {
                return;
            }

            int rank;
            genome.genes.Sort((a, b) => genome.FitnessCompare(a.Fitness, b.Fitness));

            switch (genome.genes[0].entity.FitnessLogic)
            {
                case FitnessLogic.PreferHigher:
                    rank = 0;
                    break;
                case FitnessLogic.PreferLower:
                    rank = 0;
                    break;
                case FitnessLogic.PreferZero:
                    rank = genome.Size;
                    break;
                default:
                    throw new NotImplementedException();
            }

            foreach (var gene in genome.genes)
            {
                switch (genome.genes[0].entity.FitnessLogic)
                {
                    case FitnessLogic.PreferHigher:
                        rank++;
                        break;
                    case FitnessLogic.PreferLower:
                        rank--;
                        break;
                    case FitnessLogic.PreferZero:
                        rank--;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                gene.Fitness = rank;
            }
        }
    }
}