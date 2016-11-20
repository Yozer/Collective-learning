using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using SFML.Graphics;

namespace Collective_learning.Simulation.Interfaces
{
    interface IAgent : Drawable
    {
        void Update(float delta);
        MapField TargetField { get; }
        IKnowledge Knowledge { get; }
    }
}
