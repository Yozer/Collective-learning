using System;
using System.Collections.Generic;

namespace Collective_learning.Simulation.Interfaces
{
    public interface IKnowledge
    {
        IDictionary<MapField, DateTime> Positive { get; }
        IDictionary<MapField, DateTime> Negative { get; }
        IDictionary<MapField, DateTime> Blocked { get; }
        IDictionary<MapField, DateTime> KnownFields { get; }
    }
}