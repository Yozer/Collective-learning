using System;
using System.Collections.Generic;
using Collective_learning.Simulation.Interfaces;

namespace Collective_learning.Simulation
{
    public class Knowledge : IKnowledge
    {
        public Dictionary<MapField, DateTime> Positive { get; }
        public Dictionary<MapField, DateTime> Negative { get; }
        public Dictionary<MapField, DateTime> Blocked { get; }
        public Dictionary<MapField, DateTime> KnownFields { get; }

        public Knowledge(int size)
        {
            ++size;
            Positive = new Dictionary<MapField, DateTime>(size);
            Negative = new Dictionary<MapField, DateTime>(size);
            Blocked = new Dictionary<MapField, DateTime>(size);
            KnownFields = new Dictionary<MapField, DateTime>(size);
        }
    }
}
