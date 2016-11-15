using Collective_learning.Simulation.Factory;
using Collective_learning.Simulation.Interfaces;

namespace Collective_learning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ISimulation simulation = SimulationFactory.CreateDefault();
            var window = SimulationWindow.Create(simulation);

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Update();
                window.Clear();
                window.Draw();
                window.Display();
            }
        }
    }
}
