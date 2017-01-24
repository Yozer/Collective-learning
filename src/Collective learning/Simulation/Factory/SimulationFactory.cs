using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Collective_learning.Simulation.Interfaces;

namespace Collective_learning.Simulation.Factory
{
    static class SimulationFactory
    {
        public static ISimulation CreateDefault()
        {
            // read settings
            ReadSettings();
            var map = new Map(Path.Combine("Assets", "map_big1.txt"));
            return new Simulation(map);
        }

        private static void ReadSettings(string settingsFile = "settings.ini")
        {
            var settings = File.ReadAllLines(settingsFile)
                .Select(t => t.Split('=').Select(x => x.Trim()).ToArray())
                .ToDictionary(t => t[0], t => t[1], StringComparer.OrdinalIgnoreCase);

            SimulationOptions.AgentsCount = Convert.ToInt32(settings[nameof(SimulationOptions.AgentsCount)]);
            SimulationOptions.ExplorationThreshold = Convert.ToSingle(settings[nameof(SimulationOptions.ExplorationThreshold)], CultureInfo.InvariantCulture);
            SimulationOptions.SharingKnowledgePenalty = Convert.ToInt32(settings[nameof(SimulationOptions.SharingKnowledgePenalty)]);
            SimulationOptions.NoSharingPeriodAfterSharingKnowledge = Convert.ToInt32(settings[nameof(SimulationOptions.NoSharingPeriodAfterSharingKnowledge)]);
            SimulationOptions.ChanceToShareKnowledge = Convert.ToSingle(settings[nameof(SimulationOptions.ChanceToShareKnowledge)], CultureInfo.InvariantCulture);
            SimulationOptions.KnowledgeSharingType = (SharingType) Enum.Parse(typeof(SharingType), settings[nameof(SimulationOptions.KnowledgeSharingType)]);
        }
    }
}