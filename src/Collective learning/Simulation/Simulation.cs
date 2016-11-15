using Collective_learning.Simulation.Interfaces;
using SFML.Graphics;

namespace Collective_learning.Simulation
{
    class Simulation : ISimulation
    {
        private readonly Map _map;

        public Simulation(Map map)
        {
            _map = map;
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_map);
        }

        public void Update()
        {
        }
    }
}