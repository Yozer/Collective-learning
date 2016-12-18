using System;
using System.Collections.Generic;
using System.Linq;
using Collective_learning.Simulation.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Collective_learning.Simulation
{
    class Simulation : ISimulation
    {
        private readonly Map _map;
        private readonly SimulationOptions _options;
        private readonly List<IAgent> _agents = new List<IAgent>();
        private IAgent _selectedAgent = null;

        public float Width => _map.Width*SimulationOptions.FieldWidth;
        public float Height => _map.Height*SimulationOptions.FieldHeight;
        public SimulationStatistics SimulationStatistics { get; set; } = new SimulationStatistics();
        public bool Paused { get; set; } = true;

        public Simulation(Map map, SimulationOptions options)
        {
            _map = map;
            _options = options;

            InitAgents();

            SimulationStatistics.AllFieldsCount = _map.Fields.Length;
            SimulationStatistics.PopulationCount = _agents.Count;
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

            _agents.ForEach(t => t.Update(delta));
            CalculateStatistics();
        }

        public void ProcessClick(Vector2f point)
        {
            IAgent clickedAgent = _agents.FirstOrDefault(t => t.Bounds.Contains(point));
            if (_selectedAgent != null)
            {
                _selectedAgent.Selected = false;
                _selectedAgent = null;
            }
            if (clickedAgent != null)
            {
                clickedAgent.Selected = true;
                _selectedAgent = clickedAgent;
            }

            CalculateStatistics();
        }

        private void CalculateStatistics()
        {
            foreach (var mapField in _map.Fields)
                mapField.SpecialColor = default(Color);

            SimulationStatistics.DangerCount = SimulationStatistics.FoodCount = SimulationStatistics.WaterCount = 0;
            var totalVisitedFields = new HashSet<MapField>();

            foreach (var agent in _agents)
            {
                if (_selectedAgent == null || agent == _selectedAgent)
                {
                    if (agent.TargetField != null)
                        agent.TargetField.SpecialColor = Color.Yellow;
                    foreach (var knownField in agent.Knowledge.KnownFields)
                    {
                        _map.Fields[knownField.X, knownField.Y].Darker = true;
                        totalVisitedFields.Add(knownField);
                    }

                    SimulationStatistics.DangerCount += agent.Statistics.DangerCount;
                    SimulationStatistics.FoodCount += agent.Statistics.FoodCount;
                    SimulationStatistics.WaterCount += agent.Statistics.WaterCount;
                }
            }

            SimulationStatistics.DiscoveredCount = totalVisitedFields.Count;
        }
    }
}