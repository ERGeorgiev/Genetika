using EdsLibrary.Extensions;
using Genetika.Interfaces;
using System;
using System.Collections.Generic;
using Genetika.Genetic.Fitness;
using EdsLibrary.Logging.Table;

namespace Genetika.Examples.InertionDouble
{
    public class DynamicEntity : IEntity<DynamicEntity>
    {
        private static int Count = 0;

        public const int NumberOfInputs = 4;
        public const int NumberOfOutputs = 2;

        public PhysicalEntity entityA = new PhysicalEntity();
        public PhysicalEntity entityB = new PhysicalEntity();

        public DynamicEntity()
        {
            Id = Count++;
        }

        public int Id { get; private set; }

        public bool AccumulativeFitness => true;

        public FitnessLogic FitnessLogic => FitnessLogic.PreferZero;

        public float GetFitness()
        {
            return entityA.DistanceFromGoal + entityB.DistanceFromGoal;
        }

        public void Update(float[] outputs)
        {
            entityA.Update(outputs[0]);
            entityB.Update(outputs[1]);
        }

        public float[] GenerateInput()
        {
            float distanceA = entityA.positionGoal - entityA.position;
            float distanceB = entityB.positionGoal - entityB.position;
            return new float[] { entityA.speed, distanceA, entityB.speed, distanceB };
        }

        public void Restart()
        {
            entityA.Restart();
            entityB.Restart();
        }

        public TableFormatter GetTableFormatter()
        {
            TableFormatter formatter = new TableFormatter();
            formatter.Add("Id", Id, "0", 4);
            formatter.Add("SpeedA", entityA.speed, "+0.0;-0.0", 9);
            formatter.Add("InertionA", entityA.inertion, "+0.0;-0.0", 9);
            formatter.Add("PositionA", entityA.position, "+0.0;-0.0", 9);
            formatter.Add("GoalA", entityA.positionGoal, "+0.0;-0.0", 9);
            formatter.Add("SpeedB", entityB.speed, "+0.0;-0.0", 9);
            formatter.Add("InertionB", entityB.inertion, "+0.0;-0.0", 9);
            formatter.Add("PositionB", entityB.position, "+0.0;-0.0", 9);
            formatter.Add("GoalB", entityB.positionGoal, "+0.0;-0.0", 9);

            return formatter;
        }

        public TableFormatter GetTableFormatter(DynamicEntity compare)
        {
            TableFormatter formatter = new TableFormatter();
            formatter.Add("Id", new KeyValuePair<float, float>(Id, compare.Id), "0", 4);
            formatter.Add("SpeedA", new KeyValuePair<float, float>(entityA.speed, compare.entityA.speed), "+0.0;-0.0", 9);
            formatter.Add("InertionA", new KeyValuePair<float, float>(entityA.inertion, compare.entityA.inertion), "+0.0;-0.0", 9);
            formatter.Add("PositionA", new KeyValuePair<float, float>(entityA.position, compare.entityA.position), "+0.0;-0.0", 9);
            formatter.Add("GoalA", new KeyValuePair<float, float>(entityA.positionGoal, compare.entityA.positionGoal), "+0.0;-0.0", 9);
            formatter.Add("SpeedB", new KeyValuePair<float, float>(entityB.speed, compare.entityB.speed), "+0.0;-0.0", 9);
            formatter.Add("InertionB", new KeyValuePair<float, float>(entityB.inertion, compare.entityB.inertion), "+0.0;-0.0", 9);
            formatter.Add("PositionB", new KeyValuePair<float, float>(entityB.position, compare.entityB.position), "+0.0;-0.0", 9);
            formatter.Add("GoalB", new KeyValuePair<float, float>(entityB.positionGoal, compare.entityB.positionGoal), "+0.0;-0.0", 9);

            return formatter;
        }
    }
}
