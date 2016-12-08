using System;
using System.Collections.Generic;
using System.Linq;
using Collective_learning.Simulation.Interfaces;
using SFML.Graphics;

namespace Collective_learning.Simulation
{
    class Simulation : ISimulation
    {
        private readonly Map _map;
        private readonly SimulationOptions _options;
        private readonly List<IAgent> _agents = new List<IAgent>();
        public float Width => _map.Width*SimulationOptions.FieldWidth;
        public float Height => _map.Height*SimulationOptions.FieldHeight;
        public SimulationStatistics SimulationStatistics { get; set; } = new SimulationStatistics();
        public bool Paused { get; set; } = true;

        public Simulation(Map map, SimulationOptions options)
        {
            _map = map;
            _options = options;

            InitAgents();
        }

        private void InitAgents()
        {
            for (int i = 0; i < _options.AgentsCount; ++i)
            {
                IAgent agent = new Agent(_map);
                _agents.Add(agent);
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_map);
            _agents.ForEach(t => t.Draw(target, states));
        }

        public void Update(float delta)
        {
            if(Paused)
                return;

            SimulationStatistics.SimulationTime = SimulationStatistics.SimulationTime.Add(TimeSpan.FromSeconds(delta));
            SimulationStatistics.PopulationCount = _agents.Count;
            SimulationStatistics.DangerCount = SimulationStatistics.FoodCount = SimulationStatistics.WaterCount = 0;

            foreach (var mapField in _map.Fields)
            {
                mapField.SpecialColor = default(Color);
            }

            foreach (IAgent agent in _agents)
            {
                agent.Update(delta);
                if (agent.TargetField != null)
                    agent.TargetField.SpecialColor = Color.Yellow;
                foreach (var knownField in agent.Knowledge.KnownFields)
                {
                    _map.Fields[knownField.X, knownField.Y].Darker = true;
                }

                SimulationStatistics.DangerCount += agent.Knowledge.Negative.Count;
                SimulationStatistics.FoodCount += agent.Knowledge.Positive.Count(t => t.Type == FieldType.Food);
                SimulationStatistics.WaterCount += agent.Knowledge.Positive.Count - SimulationStatistics.FoodCount;
            }
        }
    }
}