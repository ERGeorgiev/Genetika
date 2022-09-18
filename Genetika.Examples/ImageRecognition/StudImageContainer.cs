using EdsLibrary.Extensions;
using EvolveNN.Genetic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace EvolveNNExamples.Velocity
{
    public class StudImageContainer
    {
        public bool areStuds;
        public StudImage[] studImages = new StudImage[0];

        public void Add(string folderPath, bool areStuds, string fileType = "png")
        {
            string[] imagePaths = Directory.GetFiles(folderPath, $"*{ fileType }");
            List<StudImage> newImages = new List<StudImage>(imagePaths.Length + studImages.Length);

            newImages.AddRange(studImages);
            for (int i = 0; i < imagePaths.Length; i++)
            {
                Image image = Image.FromFile(imagePaths[i]);
                newImages.Add(new StudImage(image, areStuds));
            }

            studImages = newImages.ToArray();
        }

        public StudImage GetRandom()
        {
            return studImages[RandomExt.Next(0, studImages.Length - 1)];
        }
    }
}
