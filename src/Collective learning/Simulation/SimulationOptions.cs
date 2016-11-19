using SFML.Graphics;

namespace Collective_learning.Simulation
{
    internal class SimulationOptions
    {
        public int AgentsCount { get; } = 1;


        public const float Eps = 10e-2f;

        public const float AgentSpeed = 500;
        public const float AgentRadius = 12;

        public const float FieldWidth = 48;
        public const float FieldHeight = 48;

        public static readonly Color AgentColor = new Color(99, 60, 5); // brown
        public static readonly Color EmptyColor = new Color(110, 104, 109); // grey
        public static readonly Color DangerColor = new Color(249, 39, 39); // red
        public static readonly Color FoodColor = new Color(252, 122, 35); // orange
        public static readonly Color StartColor = new Color(16, 183, 19); // green
        public static readonly Color WaterColor = new Color(37, 226, 247); // blue

    }
}