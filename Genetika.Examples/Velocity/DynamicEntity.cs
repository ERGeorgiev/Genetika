using Genetika.Interfaces;
using System.Collections.Generic;
using System;
using Genetika.Genetic.Fitness;
using EdsLibrary.Logging.Table;

namespace Genetika.Examples.Velocity
{
    public class DynamicEntity : PhysicalEntity, IEntity<DynamicEntity>
    {
        private static int Count = 0;

        public const int NumberOfInputs = 2;
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
            return new float[] { speed, distance };
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
