using Genetika.Interfaces;
using System.Collections.Generic;
using System;
using Genetika.Genetic.Fitness;
using EdsLibrary.Extensions;
using EdsLibrary.Logging.Table;

namespace Genetika.Examples.VelocityWithNoise
{
    public class DynamicEntity : PhysicalEntity, IEntity<DynamicEntity>
    {
        private static int Count = 0;

        public const int NumberOfInputs = 15;
        public const int NumberOfOutputs = 1;

        public DynamicEntity()
        {
            Id = Count++;
        }

        public int Id { get; private set; }

        public bool AccumulativeFitness => true;

        public FitnessLogic FitnessLogic => FitnessLogic.PreferZero;

        public float GetFitness()
        {
            return DistanceFromGoal;
        }

        public void Update(float[] outputs)
        {
            base.Update(outputs[0]);
        }

        public float[] GenerateInput()
        {
            float speed = this.speed;
            float distance = positionGoal - position;
            List<float> values = new List<float>() { speed, distance };
            for (int i = 0; i < 10; i++)
            {
                var newVal = RandomExt.Next(-1000, 1000);
                values.Add(newVal);
            }
            for (int i = 0; i < 3; i++)
            {
                var newVal = 0;
                values.Add(newVal);
            }

            return values.ToArray();
        }

        public TableFormatter GetTableFormatter()
        {
            TableFormatter formatter = new TableFormatter();
            formatter.Add("Id", Id, "0", 4);
            formatter.Add("Speed", speed, "+0.0;-0.0", 8);
            formatter.Add("Position", position, "+0.0;-0.0", 8);
            formatter.Add("Goal", positionGoal, "+0.0;-0.0", 8);

            return formatter;
        }

        public TableFormatter GetTableFormatter(DynamicEntity compare)
        {
            TableFormatter formatter = new TableFormatter();
            formatter.Add("Id", new KeyValuePair<float, float>(Id, compare.Id), "0", 4);
            formatter.Add("Speed", new KeyValuePair<float, float>(speed, compare.speed), "+0.0;-0.0", 8);
            formatter.Add("Position", new KeyValuePair<float, float>(position, compare.position), "+0.0;-0.0", 8);
            formatter.Add("Goal", new KeyValuePair<float, float>(positionGoal, compare.positionGoal), "+0.0;-0.0", 8);

            return formatter;
        }
    }
}
