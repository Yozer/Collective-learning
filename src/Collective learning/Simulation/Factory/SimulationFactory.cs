using System;
using System.IO;
using Collective_learning.Simulation.Interfaces;

namespace Collective_learning.Simulation.Factory
{
    static class SimulationFactory
    {
        public static ISimulation CreateDefault()
        {
            // read settings
            var map = new Map(Path.Combine("Assets", "map.txt"));
            return new Simulation(map, new SimulationOptions());
        }
    }
}