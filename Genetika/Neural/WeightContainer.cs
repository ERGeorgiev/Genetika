using EdsLibrary.Extensions;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Genetika.Neural
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WeightContainer
    {
        [JsonProperty]
        public float[] weights;

        public float this[int i]
        {
            get
            {
                return weights[i];
            }
            set { weights[i] = value; }
        }

        public WeightContainer() { }

        public WeightContainer(int weights)
        {
            this.weights = new float[weights];
            for (int i = 0; i < this.weights.Length; i++)
                this.weights[i] = Weight.Generate();
        }

        /// <summary>
        /// Deep Copy Constructor
        /// </summary>
        public WeightContainer(WeightContainer source)
        {
            weights = new float[source.weights.Length];
            for (int i = 0; i < source.weights.Length; i++)
            {
                weights[i] = source.weights[i];
            }
        }

        public void Randomize()
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Weight.Generate();
            }
        }

        public void Set(float value)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = value;
            }
        }

        public void Flip()
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Weight.Invert(weights[i]);
            }
        }

        public void RandomizeSingle()
        {
            weights[RandomWeightIndex()] = Weight.Generate();
        }

        public void NullifySingle()
        {
            weights[RandomWeightIndex()] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RandomWeightIndex()
        {
            return GenetikaParameters.random.Next(0, weights.Length);
        }

        public void NullifyAll()
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = 0;
            }
        }

        public static WeightContainer DeepCopy(WeightContainer source)
        {
            WeightContainer container = new WeightContainer(source);
            return container;
        }

        public static WeightContainer ShallowCopy(WeightContainer source)
        {
            WeightContainer container = new WeightContainer
            {
                weights = source.weights
            };
            return container;
        }

        public float GetAverageWeightValue()
        {
            float sign = 0;
            for (int w = 0; w < weights.Length; w++)
            {
                sign += weights[w];
            }
            sign = (sign >= 0) ? 1 : -1;

            float totalWeight = 0;
            for (int w = 0; w < weights.Length; w++)
            {
                totalWeight += System.Math.Abs(weights[w]);
            }
            totalWeight = totalWeight * sign / weights.Length;

            return totalWeight;
        }

        public float GetMinimumWeightValue()
        {
            float weight = weights[0];
            for (int w = 1; w < weights.Length; w++)
            {
                if (weights[w] < weight)
                    weight = weights[w];
            }
            return weight;
        }

        public float GetMaximumWeightValue()
        {
            float weight = weights[0];
            for (int w = 1; w < weights.Length; w++)
            {
                if (weights[w] > weight)
                    weight = weights[w];
            }
            return weight;
        }
    }
}
