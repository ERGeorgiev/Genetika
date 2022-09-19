using Genetika.Neural;
using System;
using Genetika.Interfaces;
using System.Linq;

namespace Genetika.Genetic.Crossover
{
    public sealed class GeneticCrossover<T>
        where T : class, IEntity<T>
    {
        public static float Ratio { get; set; } = 0.7f;

        /// <summary>
        /// Exchanges the information between to Genes.
        /// </summary>
        /// <param name="geneA">Gene to use.</param>
        /// <param name="geneB">Gene to use.</param>
        /// <param name="crossoverType">Crossover type.</param>
        public static void NeuronLayer(Gene<T> geneA, Gene<T> geneB, CrossoverType crossoverType)
        {
            switch (crossoverType)
            {
                case CrossoverType.None:
                    Process(geneA, geneB, None);
                    break;
                case CrossoverType.SinglePoint:
                    Process(geneA, geneB, SinglePoint);
                    break;
                case CrossoverType.MultiPoint:
                    Process(geneA, geneB, MultiPoint);
                    break;
                case CrossoverType.OrderBased:
                    Process(geneA, geneB, OrderBased);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void Process(Gene<T> geneA, Gene<T> geneB, Action<NeuronLayer, NeuronLayer> method)
        {
            int maxLayers = Math.Min(geneA.network.hiddenLayers.Length, geneB.network.hiddenLayers.Length) - 1;
            for (int i = 0; i < maxLayers; i++)
            {
                if ((float)GenetikaParameters.random.NextDouble() >= Ratio)
                    continue;
                method(geneA.network.hiddenLayers[i], geneB.network.hiddenLayers[i]);
            }
        }

        private static void None(NeuronLayer layerA, NeuronLayer layerB)
        {
        }

        private static void SinglePoint(NeuronLayer layerA, NeuronLayer layerB)
        {
            int maxNeurons = Math.Min(layerA.Count, layerB.Count);
            int position = GenetikaParameters.random.Next(0, maxNeurons);

            for (int i = position; i < maxNeurons; i++)
            {
                Neuron exchangedA = new Neuron(layerB.neurons[i].weights.Length);
                for (int w = 0; w < exchangedA.weights.Length; w++)
                {
                    if (w < layerA.neurons[i].weights.Length)
                    {
                        exchangedA.weights[w] = layerA.neurons[i].weights[w];
                    }
                    else
                    {
                        // If Neuron from A has less than needed in the position of B, add more random ones.
                        exchangedA.weights[w] = Weight.Generate();
                    }
                }

                Neuron exchangedB = new Neuron(layerA.neurons[i].weights.Length);
                for (int w = 0; w < exchangedB.weights.Length; w++)
                {
                    if (w < layerB.neurons[i].weights.Length)
                    {
                        exchangedB.weights[w] = layerB.neurons[i].weights[w];
                    }
                    else
                    {
                        // If Neuron from A has less than needed in the position of B, add more random ones.
                        exchangedB.weights[w] = Weight.Generate();
                    }
                }

                layerA.neurons[i] = exchangedB;
                layerB.neurons[i] = exchangedA;
            }
        }

        private static void MultiPoint(NeuronLayer layerA, NeuronLayer layerB)
        {
            int maxNeurons = Math.Min(layerA.Count, layerB.Count);
            int positionA = GenetikaParameters.random.Next(0, maxNeurons);
            int positionB = GenetikaParameters.random.Next(positionA, maxNeurons);

            for (int i = positionA; i <= positionB; i++)
            {
                layerA.neurons[i] = new Neuron(layerB.neurons[i]);
            }
        }

        private static void OrderBased(NeuronLayer layerA, NeuronLayer layerB)
        {
            int maxNeurons = Math.Min(layerA.Count, layerB.Count);

            for (int i = 0; i < maxNeurons; i++)
            {
                if ((float)GenetikaParameters.random.NextDouble() >= 0.5f)
                    continue;
                layerA.neurons[i] = new Neuron(layerB.neurons[i]);
            }
        }
    }
}
