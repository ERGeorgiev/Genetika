using Newtonsoft.Json;
using System;

namespace Genetika.Neural
{
    [JsonObject(MemberSerialization.OptIn)]
    public class NeuronLayer
    {
        [JsonProperty]
        public Neuron[] neurons;

        public NeuronLayer() { }

        /// <summary>
        /// A Neuron Layer holds neurons, each of which holds weights.
        /// </summary>
        /// <param name="numberOfNeurons">Number of neurons (inputs) in the layer.</param>
        /// <param name="numberOfWeights">Should be equal to the neuron count of the previous layer or inputs count.</param>
        public NeuronLayer(int numberOfNeurons, int numberOfWeights)
        {
            this.neurons = new Neuron[numberOfNeurons];
            for (int i = 0; i < this.neurons.Length; i++)
            {
                this.neurons[i] = new Neuron(numberOfWeights);
            }
        }

        public NeuronLayer(NeuronLayer source)
        {
            neurons = new Neuron[source.Count];
            for (int i = 0; i < source.Count; i++)
            {
                neurons[i] = new Neuron(source.neurons[i]);
            }
        }

        public int Count => neurons.Length;
        public static int MinNeurons { get; set; } = 3;

        public void Randomize()
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i].Randomize();
            }
        }

        public static NeuronLayer[] DeepCopy(NeuronLayer[] neuronLayers)
        {
            NeuronLayer[] copyLayers = new NeuronLayer[neuronLayers.Length];

            for (int i = 0; i < neuronLayers.Length; i++)
            {
                copyLayers[i] = DeepCopy(neuronLayers[i]);
            }

            return copyLayers;
        }

        public static NeuronLayer DeepCopy(NeuronLayer neuronLayer)
        {
            return new NeuronLayer(neuronLayer);
        }

        public float[] Output(int numberOfOutputs)
        {
            if (numberOfOutputs > neurons.Length)
            {
                AdjustLength(numberOfOutputs);
            }
            float neuronsPerOutput = neurons.Length / numberOfOutputs;
            int startingOutput;
            int endingOutput;
            int total;
            float[] outputs = new float[numberOfOutputs];

            for (int o = 0; o < numberOfOutputs; o++)
            {
                startingOutput = (int)Math.Round(neuronsPerOutput * o);
                endingOutput = (int)(Math.Round(neuronsPerOutput * o) + neuronsPerOutput);
                total = endingOutput - startingOutput;
                for (int n = startingOutput; n < endingOutput; n++)
                {
                    outputs[o] += (float)neurons[n].input;
                }
                outputs[o] /= total;
                outputs[o] *= 2;
                // Multiplied by two to avoid input loss with every layer in cases where
                // the weights average 0.5.
            }

            return outputs;
        }

        private void AdjustLength(int newLength)
        {
            int oldSize = neurons.Length;
            Array.Resize(ref neurons, newLength);
            for (int n = oldSize; n < neurons.Length; n++)
            {
                neurons[n].Randomize();
            }
        }

        public void FeedForward(NeuronLayer destination)
        {
            for (int i = 0; i < destination.neurons.Length; i++)
            {
                destination.neurons[i].Input(neurons);
            }
        }

        public float GetAverageWeightValue()
        {
            float sign = 0;
            for (int i = 0; i < neurons.Length; i++)
            {
                sign += neurons[i].GetAverageWeightValue();
            }
            sign = (sign >= 0) ? 1 : -1;

            float totalWeight = 0;
            for (int i = 0; i < neurons.Length; i++)
            {
                totalWeight += Math.Abs(neurons[i].GetAverageWeightValue());
            }
            totalWeight = totalWeight * sign / neurons.Length;

            return totalWeight;
        }

        public float GetMinimumWeightValue()
        {
            float weight = neurons[0].GetMinimumWeightValue();
            float tempWeight;
            for (int n = 1; n < neurons.Length; n++)
            {
                tempWeight = neurons[n].GetMinimumWeightValue();
                if (tempWeight < weight)
                    weight = tempWeight;
            }
            return weight;
        }

        public float GetMaximumWeightValue()
        {
            float weight = neurons[0].GetMaximumWeightValue();
            float tempWeight;
            for (int n = 1; n < neurons.Length; n++)
            {
                tempWeight = neurons[n].GetMaximumWeightValue();
                if (tempWeight > weight)
                    weight = tempWeight;
            }
            return weight;
        }
    }
}
