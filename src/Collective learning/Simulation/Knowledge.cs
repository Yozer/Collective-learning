using System;
using System.Collections.Generic;
using Collective_learning.Simulation.Interfaces;

namespace Collective_learning.Simulation
{
    public class Knowledge : IKnowledge
    {
        public IDictionary<MapField, DateTime> Positive { get; } = new Dictionary<MapField, DateTime>();
        public IDictionary<MapField, DateTime> Negative { get; } = new Dictionary<MapField, DateTime>();
        public IDictionary<MapField, DateTime> Blocked { get; } = new Dictionary<MapField, DateTime>();
        public IDictionary<MapField, DateTime> KnownFields { get; } = new Dictionary<MapField, DateTime>();
    }
}
