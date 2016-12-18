using System;
using SFML.Graphics;

namespace Collective_learning.Simulation.Interfaces
{
    interface IAgent : Drawable
    {
        void Update(float delta);
        MapField TargetField { get; }
        IKnowledge Knowledge { get; }
        bool Selected { get; set; }
        CircleShape Bounds { get; }
        SimulationStatistics Statistics { get; }
    }
}
