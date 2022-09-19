using System;
using System.Collections.Generic;
using System.Linq;
using Genetika.Interfaces;
using EdsLibrary.Logging;
using EdsLibrary.Extensions;
using Newtonsoft.Json;

namespace Genetika.Neural
{
    /// <summary>
    /// Initializes the neural network with random numbers
    /// </summary>
    /// <param name="layers">layers to the neural network</param>
    [JsonObject(MemberSerialization.OptIn)]
    public class NeuralNetwork : ITablePrint<NeuralNetwork>
    {
        private readonly int minLayers;
        private readonly int maxLayers;
        [JsonProperty]
        public NeuronLayer[] hiddenLayers;

        public NeuralNetwork(int numberOfInputs, int numberOfOutputs)
        {
            this.minLayers = numberOfInputs;
            this.maxLayers = numberOfInputs * 3;
            int hiddenLayersCount = GenetikaParameters.random.Next(this.minLayers, this.maxLayers + 1);
            int numberOfNeurons = numberOfInputs;

            this.hiddenLayers = new NeuronLayer[hiddenLayersCount];
            this.hiddenLayers[0] = new NeuronLayer(numberOfNeurons, numberOfNeurons);

            int minNeurons = Math.Max(NeuronLayer.MinNeurons, numberOfInputs);
            int neuronsLimit = (int)Math.Round(numberOfInputs * 5 * MathExt.Exponential(GenetikaParameters.random.NextDouble(), 5));
            neuronsLimit = Math.Max(minNeurons, neuronsLimit);
            for (int i = 1; i < this.hiddenLayers.Length; i++)
            {
                // Weights per neuron should be equal to the number of neurons in the last layer.
                int previousNeuronCount = this.hiddenLayers[i - 1].Count;

                if (i < this.hiddenLayers.Length - 1)
                {
                    // Set max neurons based on the previous layer's neuron count.
                    int maxNeurons = Math.Max(minNeurons, previousNeuronCount * 2 * numberOfOutputs);

                    // Limit max neurons based on the calculated neuron limit.
                    maxNeurons = Math.Min(maxNeurons, neuronsLimit);

                    // Set the neuron count to a random value between min and max.
                    numberOfNeurons = GenetikaParameters.random.Next(minNeurons, maxNeurons + 1);
                }
                else
                {
                    // Last layer
                    numberOfNeurons = numberOfOutputs;
                }

                this.hiddenLayers[i] = new NeuronLayer(numberOfNeurons, previousNeuronCount);
            }

            // Output Scaling
            // Because chances are that inputs are already scaled as they should be,
            // compared to other inputs, and scaling 2 of the same type of input differently
            // can confuse the AI. Instead, outputs tend to be of different types, so scaling
            // them can be more beneficial.
            foreach (var item in hiddenLayers[hiddenLayers.Length - 1].neurons)
            {
                for (int i = 0; i < item.weights.Length; i++)
                {
                    item.weights[i] *= (float)Math.Pow(10, RandomExt.Next(-3, 4));
                }
            }
        }

        /// <summary>
        /// Shallow copies all hidden layers.
        /// </summary>
        [JsonConstructor]
        internal NeuralNetwork()
        {
        }

        /// <summary>
        /// Deep copy constructor.
        /// </summary>
        /// <param name="source">Network to deep copy</param>
        public NeuralNetwork(NeuralNetwork source)
        {
            this.hiddenLayers = new NeuronLayer[source.hiddenLayers.Length];
            for (int i = 0; i < source.hiddenLayers.Length; i++)
            {
                this.hiddenLayers[i] = new NeuronLayer(source.hiddenLayers[i]);
            }
        }

        public int NumberOfInputs => hiddenLayers[0].Count;

        public int NumberOfOutputs => hiddenLayers[hiddenLayers.Length - 1].Count;

        public NeuronLayer RandomLayer()
        {
            int posA = GenetikaParameters.random.Next(0, hiddenLayers.Length);

            return hiddenLayers[posA];
        }

        /// <summary>
        /// Feed forward this neural network with a given input array.
        /// </summary>
        /// <param name="inputs">Inputs to network</param>
        public float[] FeedForward(params float[] inputs)
        {
            if (inputs.Length != hiddenLayers[0].neurons.Length)
            {
                throw new IndexOutOfRangeException($"Received {inputs.Length} number of inputs, expected {hiddenLayers[0].neurons.Length}.");
            }

            // First layer, parallel input to neurons.
            for (int i = 0; i < inputs.Length; i++)
            {
                hiddenLayers[0].neurons[i].Input(inputs[i]);
            }

            // Hidden layers feed forward.
            for (int i = 0; i < hiddenLayers.Length - 1; i++)
            {
                hiddenLayers[i].FeedForward(hiddenLayers[i + 1]);
            }

            return hiddenLayers[hiddenLayers.Length - 1].Output(NumberOfOutputs);
        }

        public float GetAverageWeightValue()
        {
            float sign = 0;
            for (int i = 0; i < hiddenLayers.Length; i++)
            {
                sign += hiddenLayers[i].GetAverageWeightValue();
            }
            sign = (sign >= 0) ? 1 : -1;

            float totalWeight = 0;
            for (int i = 0; i < hiddenLayers.Length; i++)
            {
                totalWeight += Math.Abs(hiddenLayers[i].GetAverageWeightValue());
            }
            totalWeight = totalWeight * sign / hiddenLayers.Length;

            return totalWeight;
        }

        public float GetMinimumWeightValue()
        {
            float weight = hiddenLayers[0].GetMinimumWeightValue();
            float tempWeight;
            for (int n = 1; n < hiddenLayers.Length; n++)
            {
                tempWeight = hiddenLayers[n].GetMinimumWeightValue();
                if (tempWeight < weight)
                    weight = tempWeight;
            }
            return weight;
        }

        public float GetMaximumWeightValue()
        {
            float weight = hiddenLayers[0].GetMaximumWeightValue();
            float tempWeight;
            for (int n = 1; n < hiddenLayers.Length; n++)
            {
                tempWeight = hiddenLayers[n].GetMaximumWeightValue();
                if (tempWeight > weight)
                    weight = tempWeight;
            }
            return weight;
        }

        public TableFormatter GetTableFormatter()
        {
            TableFormatter formatter = new TableFormatter();
            formatter.Add("Lrs", hiddenLayers.Length, "0", 3);
            float neuronAvg = (float)hiddenLayers.Sum(x => x.neurons.Length) / hiddenLayers.Length;
            formatter.Add("Avg N", neuronAvg, "0.0", 5);
            formatter.Add("Avg Wght", GetAverageWeightValue(), "+0.0;-0.0", 8);
            var neuronArray = hiddenLayers.Select(l => l.neurons.Length);
            formatter.Add("Neurons per Layer", string.Join(" ", neuronArray.ToArray()), 40);

            return formatter;
        }

        public TableFormatter GetTableFormatter(NeuralNetwork compare)
        {
            TableFormatter formatter = new TableFormatter();
            formatter.Add("Lrs", new KeyValuePair<float, float>(hiddenLayers.Length, compare.hiddenLayers.Length), "0", 3);
            float neuronAvg = (float)hiddenLayers.Sum(x => x.neurons.Length) / hiddenLayers.Length;
            float neuronAvgCompare = (float)compare.hiddenLayers.Sum(x => x.neurons.Length) / compare.hiddenLayers.Length;
            formatter.Add("Avg N", new KeyValuePair<float, float>(neuronAvg, neuronAvgCompare), "0.0", 5);
            formatter.Add("Avg Wght", new KeyValuePair<float, float>(GetAverageWeightValue(), compare.GetAverageWeightValue()), "+0.0;-0.0", 8);
            var neuronArray = hiddenLayers.Select(l => l.neurons.Length.ToString()).ToList();
            var neuronArrayCompare = compare.hiddenLayers.Select(l => l.neurons.Length.ToString()).ToList();
            List<string> neuronArrayMerged = new List<string>();
            if (neuronArray.Count > neuronArrayCompare.Count)
            {
                for (int i = 0; i < neuronArray.Count; i++)
                {
                    if (i < neuronArrayCompare.Count)
                    {
                        neuronArray[i] = neuronArray[i] + $"[{neuronArrayCompare[i]}]";
                    }
                    neuronArrayMerged.Add(neuronArray[i]);
                }
            }
            else
            {
                for (int i = 0; i < neuronArrayCompare.Count; i++)
                {
                    neuronArrayCompare[i] = $"[{neuronArrayCompare[i]}]";
                    if (i < neuronArray.Count)
                    {
                        neuronArrayCompare[i] = neuronArray[i] + neuronArrayCompare[i];
                    }
                    neuronArrayMerged.Add(neuronArrayCompare[i]);
                }
            }
            formatter.Add("Neurons per Layer", string.Join(" ", neuronArrayMerged.ToArray()), 40);

            return formatter;
        }
    }
}