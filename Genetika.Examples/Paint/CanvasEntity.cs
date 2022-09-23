using System;
using System.Diagnostics;
using System.IO;

namespace Genetika.Examples.Paint
{
    public class CanvasEntity
    {
        public static readonly int canvasSize = 64;
        public byte[][] canvas = new byte[canvasSize][];
        public float positionX = 1;
        public float positionY = 1;

        public CanvasEntity()
        {
            for (int i = 0; i < canvas.Length; i++)
            {
                canvas[i] = new byte[canvasSize];
            }
        }

        public float JudgePainting
        {
            get
            {
                var memStream = new MemoryStream();
                memStream.Capacity = canvasSize * canvasSize;

                var writeTime = Stopwatch.StartNew();
                for (int i = 0; i < canvas.Length; i++)
                {
                    memStream.Write(canvas[i], 0, 8);
                }
                writeTime.Stop();

                var readCanvas = new byte[canvasSize][];
                memStream.Seek(0, SeekOrigin.Begin);
                var readTime = Stopwatch.StartNew();
                for (int i = 0; i < canvas.Length; i++)
                {
                    var buffer = new byte[canvasSize];
                    memStream.Read(buffer, 0, 8);
                    readCanvas[i] = buffer;
                }
                readTime.Stop();

                var disposeTime = Stopwatch.StartNew();
                memStream.Dispose();
                disposeTime.Stop();

                var negativeScore = new TimeSpan();
                negativeScore += writeTime.Elapsed;
                negativeScore -= readTime.Elapsed;
                negativeScore -= disposeTime.Elapsed;

                return negativeScore.Ticks;
            }
        }

        public virtual void Update(float xChange, float yChange)
        {
            var oldX = positionX;
            var oldY = positionY;
            positionX += xChange;
            positionX %= canvasSize;
            positionY += yChange;
            positionY %= canvasSize;

            for (float i = oldX; i < length; i++)
            {
                WriteToCanvas(positionX, positionY, 10);
            }
        }

        private void WriteToCanvas(float x, float y, byte strength)
        {
            x %= canvasSize;
            y %= canvasSize;
            var indexX = Math.Abs((int)x);
            var indexY = Math.Abs((int)y);
            canvas[indexX][indexY] += strength;
        }

        public virtual void Restart()
        {
            positionX = 0;
            positionY = 0;
        }
    }
}
