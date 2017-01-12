using System;

namespace Collective_learning.Simulation
{
    public class SimulationStatistics
    {
        public int SimulationStep { get; set; }
        public int FoodCount { get; set; }
        public int WaterCount { get; set; }
        public int PopulationCount { get; set; }
        public int DangerCount { get; set; }
        public int AllFieldsCount { get; set; }
        public int AllFoodCount { get; set; }
        public int AllWaterCount { get; set; }
        public int AllThreats { get; set; }
    }
}