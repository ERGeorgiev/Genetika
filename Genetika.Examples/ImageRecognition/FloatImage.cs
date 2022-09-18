using EdsLibrary.Extensions;
using EvolveNN.Genetic;
using System;
using System.Drawing;
using System.IO;

namespace EvolveNNExamples.Velocity
{
    public class FloatImage
    {
        public Image image;
        public float[] value;

        public FloatImage(Image image)
        {
            this.image = image;
            this.value = image.ToFloatArray();
        }
    }
}
