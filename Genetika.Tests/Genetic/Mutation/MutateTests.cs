using Genetika.Genetic.Mutation;
using Genetika.Neural;
using EdsLibrary.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Genetika.Tests.Genetic.Mutation
{
    [TestClass]
    public class MutateTests
    {
        [TestMethod]
        public void TryMutate_Randomized_Mutatated()
        {
            NeuralNetwork nn = GetNeuralNetwork();
            NeuralNetwork network = new NeuralNetwork(1, 1)
            {
                hiddenLayers = GetHiddenLayers()
            };
            Mutate.Process(network, MutationType.RandomizeWeight);

            float[] input = new float[1] { 1 };
            float[] outputNn = nn.FeedForward(input);
            float[] outputMn = network.FeedForward(input);

            CollectionAssert.AreNotEquivalent(outputMn, outputNn);
        }

        [TestMethod]
        public void TryMutate_Exchange_Mutatated()
        {
            for (int i = 0; i < 100; i++)
            {
                NeuralNetwork networkA = new NeuralNetwork(100, 1);
                NeuralNetwork networkB = new NeuralNetwork(networkA);

                float[] input = new float[100];
                for (int y = 0; y < input.Length; y++)
                {
                    input[y] = RandomExt.Next();
                }
                float[] outputBeforeMutation = networkA.FeedForward(input);

                Mutate.Process(networkB, MutationType.Exchange);
                float[] outputAfterMutation = networkB.FeedForward(input);

                // Many inputs can produce very low values, thus check for null
                if (outputBeforeMutation.Sum() != 0 || outputAfterMutation.Sum() != 0)
                {
                    CollectionAssert.AreNotEquivalent(outputBeforeMutation, outputAfterMutation);
                }
            }
        }

        private NeuralNetwork GetNeuralNetwork()
        {
            NeuralNetwork nn = new NeuralNetwork(1, 1)
            {
                hiddenLayers = GetHiddenLayers()
            };

            return nn;
        }

        private NeuronLayer[] GetHiddenLayers()
        {
            NeuronLayer[] layers = new NeuronLayer[3];
            layers[0] = GetNeuronLayer();
            layers[1] = GetNeuronLayer();
            layers[2] = GetNeuronLayer();

            return layers;
        }

        private NeuronLayer GetNeuronLayer()
        {
            float[] weights = new float[1]
            {
                Weight.Generate()
            };
            weights.ToList().ForEach(x => x = 1);

            Neuron[] neurons = new Neuron[1]
            {
                new Neuron()
            };
            neurons[0].weights = weights;

            NeuronLayer nl = new NeuronLayer
            {
                neurons = neurons
            };

            return nl;
        }
    }
}
