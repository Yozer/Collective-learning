using System;
using SFML.Graphics;

namespace Collective_learning.Simulation.Interfaces
{
    public interface IAgent : Drawable, IEquatable<IAgent>
    {
        void Update(float delta);
        MapField TargetField { get; }
        IKnowledge Knowledge { get; }
        bool Selected { get; set; }
        CircleShape Bounds { get; }
        SimulationStatistics Statistics { get; }
        int Id { get; }
        int? CollidedAt { get; set; }
        int SimulationStep { get; }
        void ShareAllKnowledgeTo(IAgent shareTo);
        void ShareRandomKnowledgeTo(IAgent shareTo);
    }
}
