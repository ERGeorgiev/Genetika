using EdsLibrary.Logging.Table;
using Genetika.Genetic.Fitness;
using Genetika.Interfaces;
using System.Collections.Generic;

namespace Genetika.Examples.Paint
{
    public class ArtistEntity : CanvasEntity, IEntity<ArtistEntity>
    {
        private static int Count = 0;

        public const int NumberOfInputs = 2;
        public const int NumberOfOutputs = 2;

        public ArtistEntity()
        {
            Id = Count++;
        }

        public int Id { get; private set; }

        public bool AccumulativeFitness => true;

        public FitnessLogic FitnessLogic => FitnessLogic.PreferZero;

        public float GetFitness()
        {
            return JudgePainting;
        }

        public void Update(float[] outputs)
        {
            base.Update(outputs[0], outputs[1]);
        }

        public float[] GenerateInput()
        {
            return new float[] { positionX, positionY };
        }

        public TableFormatter GetTableFormatter()
        {
            TableFormatter formatter = new TableFormatter();
            formatter.Add("Id", Id, "0", 4);
            formatter.Add("X", positionX, "+0;-0", 1);
            formatter.Add("Y", positionY, "+0;-0", 1);

            return formatter;
        }

        public TableFormatter GetTableFormatter(ArtistEntity compare)
        {
            TableFormatter formatter = new TableFormatter();
            formatter.Add("Id", new KeyValuePair<float, float>(Id, compare.Id), "0", 4);
            formatter.Add("X", new KeyValuePair<float, float>(positionX, compare.positionX), "+0.0;-0.0", 8);
            formatter.Add("Y", new KeyValuePair<float, float>(positionY, compare.positionY), "+0.0;-0.0", 8);

            return formatter;
        }
    }
}
