using SFML.Graphics;

namespace Collective_learning.Simulation.Interfaces
{
    public interface ISimulation : Drawable
    {
        void Update(float delta);
        float Width { get; }
        float Height { get; }
    }
}