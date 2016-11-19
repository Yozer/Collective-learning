using Collective_learning.Simulation.Factory;
using Collective_learning.Simulation.Interfaces;
using SFML.System;

namespace Collective_learning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ISimulation simulation = SimulationFactory.CreateDefault();
            var window = SimulationWindow.Create(simulation);

            Clock clock = new Clock();
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Update(clock.Restart().AsSeconds());
                window.Clear();
                window.Draw();
                window.Display();
            }
        }
    }
}
