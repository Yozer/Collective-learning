using System.Collections.Generic;
using Collective_learning.Simulation.Interfaces;

namespace Collective_learning.Simulation
{
    public class Knowledge : IKnowledge
    {
        public ISet<MapField> Positive { get; } = new HashSet<MapField>();
        public ISet<MapField> Negative { get; } = new HashSet<MapField>();
        public ISet<MapField> Blocked { get; } = new HashSet<MapField>();
        public ISet<MapField> KnownFields { get; } = new HashSet<MapField>();
    }
}
