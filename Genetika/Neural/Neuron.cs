using Newtonsoft.Json;

namespace Genetika.Neural
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Neuron : WeightContainer
    {
        public double input = 0;

        public Neuron() : base() { }

        public Neuron(int weights) : base(weights) { }

        /// <summary>
        /// Deep copy constructor.
        /// </summary>
        /// <param name="source">Object to copy.</param>
        public Neuron(Neuron source) : base(source) { }

        public void Input(Neuron[] neurons)
        {
            double output = 0;

            for (int i = 0; i < neurons.Length; i++)
            {
                output += neurons[i].input * weights[i];
            }

            input = CalculateInput(output, neurons.Length);
        }

        public void Input(params double[] inputs)
        {
            double output = 0;

            for (int i = 0; i < inputs.Length; i++)
                output += inputs[i] * weights[i];

            input = CalculateInput(output, inputs.Length);
        }
        
        /// <param name="sum">Summarized and weighted input.</param>
        /// <param name="numberOfInputs">Divisor.</param>
        public static double CalculateInput(double sumInputs, int numberOfInputs)
        {
            //return (float)Math.Tanh(sumInputs / numberOfInputs);
            return 2 * sumInputs / numberOfInputs;
            // TODO: Think about this, do I need to 2*?
            // Multiplied by two to avoid input loss with every layer in cases where
            // the weights average 0.5 .
        }

        public static Neuron DeepCopy(Neuron source) => (Neuron)WeightContainer.DeepCopy(source);

        public static Neuron ShallowCopy(Neuron source) => (Neuron)WeightContainer.ShallowCopy(source);
    }
}
