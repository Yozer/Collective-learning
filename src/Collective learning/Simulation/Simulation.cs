using System.Collections.Generic;
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
        public bool Paused { get; set; }

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
            }
        }


    }
}