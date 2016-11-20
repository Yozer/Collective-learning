using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Collective_learning.Simulation.Interfaces
{
    interface IAgent : Drawable
    {
        void Update(float delta);
        Queue<MapField> Path { get; set; }

    }
}
