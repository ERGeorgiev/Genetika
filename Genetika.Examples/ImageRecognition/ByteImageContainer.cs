using EdsLibrary.Extensions;
using EvolveNN.Genetic;
using System;
using System.Drawing;
using System.IO;

namespace EvolveNNExamples.Velocity
{
    public class ByteImageContainer
    {
        public ByteImage[] byteImages;

        public ByteImageContainer(string folderPath, string fileType = "png")
        {
            string[] imagePaths = Directory.GetFiles(folderPath, $"?{ fileType }");
            byteImages = new ByteImage[imagePaths.Length];
            for (int i = 0; i < imagePaths.Length; i++)
            {
                Image image = Image.FromFile(imagePaths[i]);
                byteImages[i] = new ByteImage(image);
            }
        }
    }
}
