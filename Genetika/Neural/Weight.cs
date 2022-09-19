using EdsLibrary.Extensions;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Genetika.Neural
{
    [JsonObject(MemberSerialization.OptIn)]
    public static class Weight
    {
        // Neuron.CalculateInput() and NeuronLayer.Output() rely on abs(value) <= 1,
        // as they multiply the result by 2 to avoid input loss (Default avg of 50% per layer)
        // as multiplying several times by an average of 0.5 brings down the value too much
        // when there are many layers.
        public static readonly float minimumValue = -1f;
        public static readonly float maximumValue = 1f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Generate()
        {
            // NOTE: Tests proved that having a value of range 0 to 1 is better in the long run than having one
            // that ranges from 0 to 1 to (reduced chance) 10 .
            // NOTE2: Further tests proved that using the below formula works wonders when the user is inputing
            // a scaled value (for example 0.00 to 1.00 for an output that requires 0 to 1000000).

            // Note: Square root is a bad idea, because some inputs may have to be
            // negated in the start, while when there's a lot of hidden layers,
            // square root will produce high values for all weights, thus
            // some inputs, no matter how irrelevant, will always be in play.
            //value = (float)Math.Pow(value, 1 / root);

            return GenetikaParameters.random.Next(minimumValue, maximumValue) 
                * (1f + (((float)(Math.Pow(1000000f, GenetikaParameters.random.Next(0, maximumValue)) - 1f) / (1000000f - 1f)) * 9f)); 
            // Maps [0 -> 1] to [0 -> 10];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Invert(float value)
        {
            return MathExt.Flip(value, minimumValue, maximumValue);
        }
    }
}
