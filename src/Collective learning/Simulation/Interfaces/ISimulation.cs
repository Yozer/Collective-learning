using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Collective_learning.Simulation.Interfaces
{
    public interface ISimulation : Drawable
    {
        void Update(float delta);
        float Width { get; }
        float Height { get; }
        SimulationStatistics SimulationStatistics { get; }
        bool Paused { get; set; }
        void ProcessClick(Vector2f point);
    }
}