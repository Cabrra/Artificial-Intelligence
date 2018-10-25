using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using FullSailAFI.SteeringBehaviors.Core;

namespace FullSailAFI.SteeringBehaviors
{
    public interface ITaskForce
    {
        ColorValue Color { get; set; }
        float AlignmentStrength { get; set; }
        float CohesionStrength { get; set; }
        float SeparationStrength { get; set; }
        List<MovingObject> Boids { get; }
        Vector3 AveragePosition { get; set; }
        float FlockRadius { get; set; }

        void AddNewShip(Vector3 position);
        void TrimShip();
        void SetShipAmount(int Count);
        bool ContainsShip(MovingObject ship);
        void RemoveAllShips();
        void Update(float deltaTime);
        void CopyFrom(ITaskForce copy);
    }
}
