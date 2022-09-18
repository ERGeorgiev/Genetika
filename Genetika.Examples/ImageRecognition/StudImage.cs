using EdsLibrary.Extensions;
using EvolveNN.Genetic;
using System;
using System.Drawing;
using System.IO;

namespace EvolveNNExamples.Velocity
{
    public class StudImage : FloatImage
    {
        public bool isStud;

        public StudImage(Image image, bool isStud) : base(image)
        {
            this.isStud = isStud;
        }
    }
}
