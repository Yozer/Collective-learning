using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Collective_learning.Simulation
{
    internal class SimulationOptions
    {
        public int AgentsCount { get; } = 15;

        public static float AgentSpeed
        {
            get { return _agentSpeed; }
            set { _agentSpeed = value*1.5f*FieldWidth; }
        }

        public const float ExplorationThreshold = 0.9f;


        public const float AgentRadius = 9;
        public static readonly Color AgentColor = new Color(99, 60, 5); // brown

        public const float FieldWidth = 38;
        public const float FieldHeight = 38;

        public static readonly Dictionary<char, FieldType> FieldTypes = new Dictionary<char, FieldType>
        {
            ['E'] = FieldType.Empty,
            ['F'] = FieldType.Food,
            ['W'] = FieldType.Water,
            ['D'] = FieldType.Danger,
            ['S'] = FieldType.Start,
            ['B'] = FieldType.Blocked
        };

        public static readonly Dictionary<FieldType, Color> FieldColors = new Dictionary<FieldType, Color>
        {
            [FieldType.Blocked] = new Color(110, 104, 109), // grey
            [FieldType.Danger] = new Color(249, 39, 39), // red
            [FieldType.Food] = new Color(252, 122, 35), // orange
            [FieldType.Start] = new Color(16, 183, 19), // green
            [FieldType.Water] = new Color(37, 226, 247), // blue
            [FieldType.Empty] = Color.White
        };

        private static float _agentSpeed = 3*FieldWidth*1.5f;

        public static Random Random { get; } = new Random();
    }
}