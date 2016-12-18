using System;

namespace Collective_learning.Simulation
{
    public class SimulationStatistics
    {
        public TimeSpan SimulationTime { get; set; }
        public int FoodCount { get; set; }
        public int WaterCount { get; set; }
        public int PopulationCount { get; set; }
        public int DangerCount { get; set; }
        public int DiscoveredCount { get; set; }
    }
}