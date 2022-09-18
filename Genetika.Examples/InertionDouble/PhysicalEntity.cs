using EdsLibrary.Extensions;
using System;

namespace Genetika.Examples.InertionDouble
{
    public class PhysicalEntity
    {
        public float positionGoal = 100f;
        public float speed = 0;
        public float inertion = 0;
        public float startingPosition = 0;
        public float position;

        public PhysicalEntity()
        {
            position = startingPosition;
        }

        public float DistanceFromGoal
        {
            get
            {
                return positionGoal - position;
            }
        }

        /// <summary>
        /// Changes the position based on the velocity.
        /// </summary>
        /// <param name="newSpeed"></param>
        public virtual void Update(float newSpeed)
        {
            inertion /= 2;
            speed = newSpeed;
            position = speed + inertion + position;
            inertion += speed * 0.1f;
        }

        public virtual void Restart()
        {
            speed = 0;
            position = startingPosition;
            inertion = 0;
        }
    }
}
