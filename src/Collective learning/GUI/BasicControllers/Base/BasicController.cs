using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Collective_learning.GUI.BasicControllers.Base
{

    public abstract class BasicController : Drawable, IDisposable
    {
        public abstract void Drag(float x, float y, int offsetX);
        public abstract void OnClick(RenderWindow window, MouseButtonEventArgs args);
        public abstract void Draw(RenderTarget target, RenderStates states);
        public abstract void SetPosition(Vector2f pos);
        public abstract Vector2f GetPosition();
        public abstract FloatRect GetGlobalBound();
        public abstract FloatRect GetLocalBound();
        public abstract void Move(MouseMoveEventArgs args);
        public abstract void Dispose();
    }

}