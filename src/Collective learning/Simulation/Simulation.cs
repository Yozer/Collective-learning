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
            SimulationStatistics.FoodCount = _map.Fields.Cast<MapField>().Count(t => t.Type == FieldType.Food) * SimulationOptions.ResourceCount;
            SimulationStatistics.AllFoodCount = _map.Fields.Cast<MapField>().Count(t => t.Type == FieldType.Water) * SimulationOptions.ResourceCount;
            SimulationStatistics.AllWaterCount = _map.Fields.Cast<MapField>().Count(t => t.Type == FieldType.Water) * SimulationOptions.ResourceCount;
            SimulationStatistics.AllThreads = _map.Fields.Cast<MapField>().Count(t => t.Type == FieldType.Danger);
            SimulationStatistics.PopulationCount = _agents.Count;
        }

        private void InitAgents()
        {
            var globalKnowledge = SimulationOptions.KnowledgeSharingType == SharingType.Global ? new Knowledge(_map.Fields.Length) : null;
            for (int i = 0; i < _options.AgentsCount; ++i)
            {
                IAgent agent = new Agent(_map, globalKnowledge);
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
            ShareKnowledge();
        }

        private void ShareKnowledge()
        {
            if (SimulationOptions.KnowledgeSharingType == SharingType.AllBetweenTwo)
                ShareAllBetweenTwo();
            else if(SimulationOptions.KnowledgeSharingType == SharingType.RandomBetweenTwo)
                ShareRandomBetweenTwo();
        }

        private void ShareAllBetweenTwo()
        {
            foreach (var collision in GetAgentsThatCollide())
            {
                collision.Agent.ShareAllKnowledgeTo(collision.CollideWith);
                collision.CollideWith.ShareAllKnowledgeTo(collision.Agent);
            }
        }
        private void ShareRandomBetweenTwo()
        {
            foreach (var collision in GetAgentsThatCollide())
            {
                collision.Agent.ShareRandomKnowledgeTo(collision.CollideWith);
                collision.CollideWith.ShareRandomKnowledgeTo(collision.Agent);
            }
        }
        private IEnumerable<CollisionResult> GetAgentsThatCollide()
        {
            foreach (IAgent agent in _agents)
            {
                if (agent.CollidedAt != null)
                    continue;

                foreach (IAgent collideWith in _agents)
                {
                    if (collideWith.CollidedAt == null && agent.Id != collideWith.Id)
                    {
                        if (agent.Bounds.Collides(collideWith.Bounds))
                        {
                            agent.CollidedAt = DateTime.Now;
                            collideWith.CollidedAt = DateTime.Now;
                            yield return new CollisionResult(agent, collideWith);
                        }
                    }
                }
            }
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
            {
                mapField.Darker = false;
                mapField.SpecialColor = default(Color);
            }

            SimulationStatistics.DangerCount = SimulationStatistics.FoodCount = SimulationStatistics.WaterCount = 0;

            foreach (var agent in _agents)
            {
                if (_selectedAgent == null || agent.Equals(_selectedAgent))
                {
                    if (agent.TargetField != null)
                        agent.TargetField.SpecialColor = Color.Yellow;
                    foreach (var knownField in agent.Knowledge.KnownFields)
                    {
                        _map.Fields[knownField.Key.X, knownField.Key.Y].Darker = true;
                    }

                    SimulationStatistics.DangerCount += agent.Statistics.DangerCount;
                    SimulationStatistics.FoodCount += agent.Statistics.FoodCount;
                    SimulationStatistics.WaterCount += agent.Statistics.WaterCount;
                }
            }

            SimulationStatistics.DiscoveredCount = _map.Fields.Cast<MapField>().Count(t => t.Darker);
        }

        struct CollisionResult
        {
            public CollisionResult(IAgent agent, IAgent collideWith)
            {
                Agent = agent;
                CollideWith = collideWith;
            }

            public IAgent Agent { get; }
            public IAgent CollideWith { get; }

        }
    }
}