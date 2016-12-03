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
        private List<Box> _boxList;
        private FloatRect _bounds;

        public Panel()
        {
            _bounds = new FloatRect(10, 10, 0, 0);
            _boxList = new List<Box>();
            FillColor = new Color(70, 70, 70);
        }

        public void AddBox(Box box)
        {
            if (_boxList == null)
            {
                _boxList = new List<Box>();
            }
            box.SetPosition(new Vector2f(_bounds.Left, _bounds.Height + _bounds.Top + 15));
            _boxList.Add(box);
            _bounds.Height += box.GlobalBound.Height + 15;
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

        public void ProcessClick(object sender, MouseButtonEventArgs e)
        {
            _boxList.ForEach(t => t.ProcessClick(e));
        }
    }
}