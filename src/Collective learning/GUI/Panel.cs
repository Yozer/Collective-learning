using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Collective_learning.GUI.BasicControllers;


namespace Collective_learning.GUI
{
    public class Panel : RectangleShape
    {
        private readonly RenderWindow _simulationWindow;
        private List<Box> _boxList;
        private FloatRect _bounds;

        public Panel(RenderWindow window)
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

            FillColor = new Color(70, 70, 70);
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

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
            _boxList.ForEach(target.Draw);
        }

        public void Dragging(Vector2i point)
        {
            _boxList.ForEach(t => t.Dragging(point));
        }

        public void OnClick(object sender, MouseButtonEventArgs args)
        {
            _boxList.ForEach(t => t.OnClick(_simulationWindow, args));
        }
    }
}