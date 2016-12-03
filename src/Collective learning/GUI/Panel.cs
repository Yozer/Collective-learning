using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Collective_learning.GUI.BasicControllers;


namespace Collective_learning.GUI
{
    public class Panel : View, Drawable
    {
        private readonly RenderWindow _simulationWindow;
        private readonly RectangleShape _shape;
        private List<Box> _boxList;
        private FloatRect _bounds;

        public FloatRect Bounds => _shape.GetGlobalBounds();

        public Panel(FloatRect fr, RenderWindow window) : base(fr)
        {
            _simulationWindow = window;
            _bounds = new FloatRect(0, 0, 0, 0);
            _boxList = new List<Box>();
            Box box = new Box();
            box.AddController(new Slider("test", 0, 4, 1));
            box.AddController(new Slider("Try2 dluzszy", 1, 2, 1));
            box.AddController(new Slider("test3", 1, 2, 1.5f));
            AddBox(box);

            Box box2 = new Box(vertical: false);
            box2.AddController(new Button("test"));
            box2.AddController(new Button("Wyzyny2"));
            AddBox(box2);

            _shape = new RectangleShape();

        }

        private void AddBox(Box box)
        {
            if (_boxList == null)
            {
                _boxList = new List<Box>();
            }
            box.SetPosition(new Vector2f(_bounds.Left, _bounds.Height + _bounds.Top));
            _boxList.Add(box);
            _bounds.Height += box.GlobalBound.Height;
        }


        public void Draw(RenderTarget target, RenderStates states)
        {
            _shape.Size = new Vector2f(Size.X, Size.Y);
            _shape.FillColor = new Color(70, 70, 70);
            target.Draw(_shape);

            _boxList.ForEach(target.Draw);
        }

        public void Dragging(float x, float y, int offsetX)
        {
            _boxList.FirstOrDefault(t => t.GlobalBound.Contains(x, y))?.Dragging(x, y, offsetX);
        }

        public void OnClick(object sender, MouseButtonEventArgs args)
        {
            _boxList.ForEach(t => t.OnClick(_simulationWindow, args));
        }
    }
}