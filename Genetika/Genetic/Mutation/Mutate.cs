using Genetika.Neural;
using EdsLibrary.Extensions;
using System;
using System.Linq;

namespace Genetika.Genetic.Mutation
{
    internal static class Mutate
    {
        public static float MutationRate { get; set; } = 0.1f;

        public static void Try(NeuralNetwork network, MutationType? mutationType = null)
        {
            if (GenetikaParameters.random.NextDouble() > MutationRate)
                return;

            Process(network, mutationType);
        }

        public static void Process(NeuralNetwork network, MutationType? mutationType = null)
        {
            if (mutationType == null) mutationType = RandomMutationType();

            switch (mutationType)
            {
                case MutationType.Exchange:
                    Exchange(network);
                    break;
                case MutationType.RandomizeWeight:
                    RandomizeWeight(network);
                    break;
                case MutationType.NullifyWeight:
                    NullifyWeight(network);
                    break;
                case MutationType.NullifyInput:
                    NullifyInput(network);
                    break;
                default:
                    throw new NotImplementedException($"Selected mutation type '{mutationType}' is not implemented.");
            }
        }

        private static void Exchange(NeuralNetwork network)
        {
            var layers = network.hiddenLayers.Where(l => l.Count > 1);
            if (layers.Any() == false)
            {
                return;
            }

            NeuronLayer layer = layers.ToList().RandomElement();
            int posA = GenetikaParameters.random.Next(0, layer.Count);
            int posB = GenetikaParameters.random.Next(0, layer.Count);

            // Make sure mutation happens even when equal
            if (posA == posB)
            {
                if (posB > 0)
                {
                    posA = posB - 1;
                }
                else if (posB < layer.Count - 1)
                {
                    posA = posB++;
                }
                else
                {
                    throw new Exception("Unexpected behavior while Mutating.");
                }
            }

            Neuron neuronA = layer.neurons[posA];
            layer.neurons[posA] = layer.neurons[posB];
            layer.neurons[posB] = neuronA;
        }

        private static void RandomizeWeight(NeuralNetwork network)
        {
            NeuronLayer layer = network.RandomLayer();
            int posA = GenetikaParameters.random.Next(0, layer.Count);

            layer.neurons[posA].RandomizeSingle();
        }

        private static void NullifyInput(NeuralNetwork network)
        {
            NeuronLayer layer = network.hiddenLayers[0];
            int posA = GenetikaParameters.random.Next(0, layer.Count);
            layer.neurons[posA].NullifyAll();
        }

        private static void NullifyWeight(NeuralNetwork network)
        {
            NeuronLayer layer = network.RandomLayer();
            int posA = GenetikaParameters.random.Next(0, layer.Count);
            if (GenetikaParameters.random.Next(0, 10) != 0)
            {
                layer.neurons[posA].NullifySingle();
            }
            else
            {
                layer.neurons[posA].NullifyAll();
            }
        }

        public static MutationType RandomMutationType()
        {
            return (MutationType)GenetikaParameters.random.Next(0, Enum.GetValues(typeof(MutationType)).Length);
        }
    }
}