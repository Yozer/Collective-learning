using System.Collections.Generic;
using SFML.System;

namespace Collective_learning.Simulation.Interfaces
{
    interface IKnowledge
    {
        ISet<MapField> Positive { get; }
        ISet<MapField> Negative { get; }
        ISet<MapField> Blocked { get; }
        ISet<MapField> KnownFields { get; }
    }
}