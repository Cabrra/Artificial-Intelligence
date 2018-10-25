using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using FullSailAFI.SteeringBehaviors.Core;

namespace FullSailAFI.SteeringBehaviors.StudentAI
{
    public class Flock
    {
        public float AlignmentStrength { get; set; }
        public float CohesionStrength { get; set; }
        public float SeparationStrength { get; set; }
        public List<MovingObject> Boids { get; protected set; }
        public Vector3 AveragePosition { get; set; }
        protected Vector3 AverageForward { get; set; }
        public float FlockRadius { get; set; }

        #region Constructors
        public Flock()
        {
        }
        #endregion

        #region TODO Suggested helper methods

        private void CalculateAverages()
        {
            Vector3 avgPos, avgFor;
            avgPos = Vector3.Empty;
            avgFor = Vector3.Empty;
            
            float counter = 0;
            foreach (MovingObject myP in Boids)
            {
                avgPos += myP.Position;
                avgFor += myP.Velocity;
                counter++;
            }

            AveragePosition = Vector3.Multiply(avgPos, 1/counter);
            AverageForward = Vector3.Multiply(avgFor, 1/counter);

            return;
        }

        private Vector3 CalculateAlignmentAcceleration(MovingObject boid)
        {

          Vector3 vector = Vector3.Multiply(AverageForward, 1/boid.MaxSpeed);
          if (vector.Length() > 1)
              vector.Normalize();
           return vector * AlignmentStrength;
        }

        private Vector3 CalculateCohesionAcceleration(MovingObject boid)
        {
           Vector3 vector = Vector3.Subtract(AveragePosition, boid.Position);
            float distance = vector.Length();
            vector.Normalize();

            if (distance < FlockRadius)
               vector = Vector3.Multiply(vector, (distance / FlockRadius));

            return vector * CohesionStrength;

        }

        private Vector3 CalculateSeparationAcceleration(MovingObject boid)
        {
            Vector3 sum = Vector3.Empty;

            foreach (MovingObject otherBoid in Boids)
            {
                Vector3 vector = Vector3.Subtract(boid.Position, otherBoid.Position);
                float distance = vector.Length();
                float safeDistance = boid.SafeRadius + otherBoid.SafeRadius;
                if (distance < safeDistance)
                {
                    vector.Normalize();
                    vector = Vector3.Multiply(vector, (safeDistance - distance) / safeDistance);
                    sum += vector;
                }
                if (sum.Length() > 1.0f)
                    sum.Normalize();

                
            }
            return sum * SeparationStrength;
        }

        #endregion

        #region TODO

        public virtual void Update(float deltaTime)
        {

              CalculateAverages();

             foreach (MovingObject myP in Boids)
             {
                 Vector3 accel = CalculateAlignmentAcceleration(myP);
                 accel = Vector3.Add(accel, CalculateCohesionAcceleration(myP));
                 accel = Vector3.Add(accel, CalculateSeparationAcceleration(myP));

                 float accMultiply = myP.MaxSpeed;
                 accel = Vector3.Multiply(accel, accMultiply * deltaTime);

                myP.Velocity = Vector3.Add( myP.Velocity, accel);

                 if (myP.Velocity.Length() > myP.MaxSpeed)
                 {
                     myP.Velocity.Normalize();
                     myP.Velocity = Vector3.Multiply(myP.Velocity, myP.MaxSpeed);
                 }
                 myP.Update(deltaTime);
             }

        }
        #endregion
    }
}
