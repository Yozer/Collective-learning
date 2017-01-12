using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Collective_learning.Simulation
{
    internal class SimulationOptions
    {
        public int AgentsCount { get; } = 500;

        public const float AgentSpeed = 1f;

        public static int SimulationSpeed = 0;
        public static float ExplorationThreshold = 0.2f;
        public static int SharingKnowledgePenalty = 300;
        public static int NoSharingPeriodAfterSharingKnowledge = 1000;
        public static float ChanceToShareKnowledge = 0.7f;


        public static int ShareRandomKnowledgeMin = 2;
        public static int ShareRandomKnowledgeMax = 1000;

        public static readonly SharingType KnowledgeSharingType = SharingType.Global;

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

        public static Random Random { get; } = new Random();
    }

    internal enum SharingType
    {
        NoSharing,
        Global,
        AllBetweenTwo,
        RandomBetweenTwo
    }
}