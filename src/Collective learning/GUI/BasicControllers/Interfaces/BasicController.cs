using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.IO;
using System.Linq;

namespace Collective_learning.GUI.BasicControllers.Interfaces{

    public abstract class BasicController : Drawable{
        public Text _text { get;  set; }


        public BasicController(){

        }


        abstract public void Drag(float x,float y,int offsetX);
        abstract public void OnClick(RenderWindow window,MouseButtonEventArgs args);
        abstract public void Draw(RenderTarget target, RenderStates states);
        abstract public void SetPosition(Vector2f pos);
        abstract public Vector2f GetPosition();
        abstract public FloatRect GetGlobalBound();
        abstract public FloatRect GetLocalBound();

        abstract public void Move(MouseMoveEventArgs args);
    }

}