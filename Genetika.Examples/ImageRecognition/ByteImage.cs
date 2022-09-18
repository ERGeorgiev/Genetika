using EdsLibrary.Extensions;
using EvolveNN.Genetic;
using System;
using System.Drawing;
using System.IO;

namespace EvolveNNExamples.Velocity
{
    public class ByteImage
    {
        public Image image;
        public byte[] value;

        public ByteImage(Image image)
        {
            this.image = image;
            this.value = image.ToByteArray();
        }
    }
}
