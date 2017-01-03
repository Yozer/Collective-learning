using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Collective_learning.Simulation
{
    internal class SimulationOptions
    {
        public int AgentsCount { get; } = 100;

        public static float AgentSpeed
        {
            get { return _agentSpeed; }
            set { _agentSpeed = value*1.5f*FieldWidth; }
        }

        public static float ExplorationThreshold = 0.2f;
        public static TimeSpan SharingKnowledgePenalty = TimeSpan.FromSeconds(5);
        public static TimeSpan NoSharingPeriodAfterSharingKnowledge = TimeSpan.FromSeconds(10);
        public static int ShareRandomKnowledgeMin = 2;
        public static int ShareRandomKnowledgeMax = 5;

        public static readonly SharingType KnowledgeSharingType = SharingType.AllBetweenTwo;
        public static readonly SimulationType SimulationType = SimulationType.Fast;


        public const float AgentRadius = 4;
        public static readonly Color AgentColor = new Color(99, 60, 5); // brown
        public static readonly Color SelectedAgentColor = new Color(166, 103, 8); // light brown

        public const float FieldWidth = 14;
        public const float FieldHeight = 14;
        public const int ResourceCount = 5;

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

    internal enum SimulationType
    {
        Fast,
        Pretty
    }

    internal enum SharingType
    {
        NoSharing,
        Global,
        AllBetweenTwo,
        RandomBetweenTwo
    }
}