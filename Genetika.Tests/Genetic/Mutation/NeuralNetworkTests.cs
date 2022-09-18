using Genetika.Genetic.Mutation;
using Genetika.Neural;
using EdsLibrary.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Genetika.Tests.Genetic.Mutation
{
    [TestClass]
    public class NeuralNetworkTests
    {
        [TestMethod]
        public void FeedForward_IdenticalNeuralNetwork_Equal()
        {
            NeuralNetwork nnA = GetNeuralNetwork();
            NeuralNetwork nnB = new NeuralNetwork(1, 1)
            {
                hiddenLayers = GetHiddenLayers()
            };
            nnB.hiddenLayers = nnA.hiddenLayers;

            float[] input = new float[1] { 1 };
            float[] outputNn = nnA.FeedForward(input);
            float[] outputMn = nnB.FeedForward(input);

            CollectionAssert.AreEquivalent(outputMn, outputNn);
        }

        [TestMethod]
        public void DeepCopy_Normal_Same()
        {
            NeuralNetwork network = new NeuralNetwork(3, 1);

            float[] input = new float[3] { 1, 2, 3 };
            float[] outputA = network.FeedForward(input);

            network = new NeuralNetwork(network);
            float[] outputB = network.FeedForward(input);

            CollectionAssert.AreEquivalent(outputA, outputB);
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
