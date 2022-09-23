using Genetika.Examples.Demonstration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Genetika.Examples.Paint
{
    public class DemoPaint : Demo<ArtistEntity>
    {
        private static int runs = 0;

        public DemoPaint(int steps, GenetikaParameters parameters)
            : base(ArtistEntity.NumberOfInputs, ArtistEntity.NumberOfOutputs, steps, parameters)
        {
        }

        public override float Run()
        {
            var result = base.Run();
            var elites = this.Genome.GetElites(10);
            for (int i = 0; i < elites.Count; i++)
            {
                var elite = elites[i];
                var data = elite.entity.canvas;
                var bitmap = ToBitmap(data);
                var dir = @"C:\Users\ergeo\Projects\VisualStudio\Genetika\paintings\";
                Directory.CreateDirectory(dir);
                bitmap.Save(dir + $"{i}.jpg", ImageFormat.Jpeg);
                bitmap.Dispose();
            }
            runs++;
            return result;
        }

        public static Bitmap ToBitmap(byte[][] data)
        {
            Bitmap bmp = new Bitmap(CanvasEntity.canvasSize, CanvasEntity.canvasSize);

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    var colorVal = byte.MaxValue - data[i][j];
                    var color = Color.FromArgb(colorVal, colorVal, colorVal);
                    bmp.SetPixel(i, j, color);
                }
            }

            return bmp;
        }
    }
}
