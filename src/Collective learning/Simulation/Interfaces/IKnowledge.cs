using System;
using System.Collections.Generic;

namespace Collective_learning.Simulation.Interfaces
{
    public interface IKnowledge
    {
        Dictionary<MapField, DateTime> Positive { get; }
        Dictionary<MapField, DateTime> Negative { get; }
        Dictionary<MapField, DateTime> Blocked { get; }
        Dictionary<MapField, DateTime> KnownFields { get; }
    }
}