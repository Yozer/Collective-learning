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

        public Simulation(Map map, SimulationOptions options)
        {
            _map = map;
            _options = options;

            InitAgents();
        }

        private void InitAgents()
        {
            //for (int i = 0; i < _options.AgentsCount; ++i)
            //{
                IAgent agent = new Agent(_map.StartField);
                for (int x = 0; x < 100; ++x)
                {
                    agent.Path.Enqueue(_map.Fields[11, 1]);
                    agent.Path.Enqueue(_map.Fields[11, 2]);
                    agent.Path.Enqueue(_map.Fields[10, 2]);
                    agent.Path.Enqueue(_map.Fields[10, 3]);
                    agent.Path.Enqueue(_map.Fields[9, 3]);
                    agent.Path.Enqueue(_map.Fields[8, 3]);
                    agent.Path.Enqueue(_map.Fields[7, 3]);
                    agent.Path.Enqueue(_map.Fields[6, 3]);
                    agent.Path.Enqueue(_map.Fields[6, 4]);
                    agent.Path.Enqueue(_map.Fields[6, 5]);
                    agent.Path.Enqueue(_map.Fields[6, 6]);
                    agent.Path.Enqueue(_map.Fields[7, 6]);
                    agent.Path.Enqueue(_map.Fields[8, 6]);
                    agent.Path.Enqueue(_map.Fields[9, 6]);
                    agent.Path.Enqueue(_map.Fields[10, 6]);
                    agent.Path.Enqueue(_map.Fields[11, 6]);
                    agent.Path.Enqueue(_map.Fields[11, 5]);
                    agent.Path.Enqueue(_map.Fields[11, 4]);
                    agent.Path.Enqueue(_map.Fields[11, 3]);
                    agent.Path.Enqueue(_map.Fields[11, 2]);
                    agent.Path.Enqueue(_map.Fields[11, 1]);
                    agent.Path.Enqueue(_map.Fields[11, 0]);
                }

            _agents.Add(agent);
            agent = new Agent(_map.StartField);
            for (int x = 0; x < 100; ++x)
            {
                for (int y = 10; y >= 0; --y)
                    agent.Path.Enqueue(_map.Fields[y, 0]);
                for (int y = 1; y <= 11; ++y)
                    agent.Path.Enqueue(_map.Fields[y, 0]);
                for (int y = 1; y <= 10; ++y)
                    agent.Path.Enqueue(_map.Fields[11, y]);
                for (int y = 9; y >= 0; --y)
                    agent.Path.Enqueue(_map.Fields[11, y]);

                agent.Path.Enqueue(_map.Fields[11, 1]);
                agent.Path.Enqueue(_map.Fields[11, 2]);
                agent.Path.Enqueue(_map.Fields[10, 2]);
                agent.Path.Enqueue(_map.Fields[10, 3]);
                agent.Path.Enqueue(_map.Fields[9, 3]);
                agent.Path.Enqueue(_map.Fields[8, 3]);
                agent.Path.Enqueue(_map.Fields[7, 3]);
                agent.Path.Enqueue(_map.Fields[6, 3]);
                agent.Path.Enqueue(_map.Fields[6, 4]);
                agent.Path.Enqueue(_map.Fields[6, 5]);
                agent.Path.Enqueue(_map.Fields[6, 6]);
                agent.Path.Enqueue(_map.Fields[7, 6]);
                agent.Path.Enqueue(_map.Fields[8, 6]);
                agent.Path.Enqueue(_map.Fields[9, 6]);
                agent.Path.Enqueue(_map.Fields[10, 6]);
                agent.Path.Enqueue(_map.Fields[11, 6]);
                agent.Path.Enqueue(_map.Fields[11, 5]);
                agent.Path.Enqueue(_map.Fields[11, 4]);
                agent.Path.Enqueue(_map.Fields[11, 3]);
                agent.Path.Enqueue(_map.Fields[11, 2]);
                agent.Path.Enqueue(_map.Fields[11, 1]);
                agent.Path.Enqueue(_map.Fields[11, 0]);
            }

            _agents.Add(agent);

            agent = new Agent(_map.StartField);
            for (int x = 0; x < 100; ++x)
            {
                for (int y = 10; y >= 0; --y)
                    agent.Path.Enqueue(_map.Fields[y, 0]);
                for (int y = 1; y <= 11; ++y)
                    agent.Path.Enqueue(_map.Fields[y, 0]);
                for (int y = 1; y <= 10; ++y)
                    agent.Path.Enqueue(_map.Fields[11, y]);
                for (int y = 9; y >= 0; --y)
                    agent.Path.Enqueue(_map.Fields[11, y]);
            }
            _agents.Add(agent);
            //}
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_map);
            _agents.ForEach(t => t.Draw(target, states));
        }

        public void Update(float delta)
        {
            _agents.ForEach(t => t.Update(delta));
        }


    }
}