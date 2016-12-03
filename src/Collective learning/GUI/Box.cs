using System.Collections.Generic;
using Collective_learning.GUI.BasicControllers.Base;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Collective_learning.GUI
{
    public class Box : Drawable
    {
        protected List<BasicController> ObjectsList;
        private FloatRect _bounds;
        private readonly bool _vertical;
        private Vector2f _position;

        public FloatRect GlobalBound => _bounds;

        public Box(bool vertical, Vector2f pos)
        {
            _vertical = vertical;
            ObjectsList = new List<BasicController>();
            _position = new Vector2f(pos.X, pos.Y);

            _bounds = new FloatRect(_position.X, _position.Y, 0, 0);
        }

        public Box(bool vertical = true) : this(vertical, new Vector2f(0, 0))
        {
        }

        public void AddController(BasicController bs)
        {
            if (ObjectsList.Count == 0)
            {
                bs.SetPosition(_position);
                _bounds.Width += bs.GetGlobalBound().Width;
                _bounds.Height += bs.GetGlobalBound().Height;
            }
            else
            {
                bs.SetPosition(NextPos(ObjectsList[ObjectsList.Count - 1]));
                if (_vertical)
                {
                    _bounds.Height += bs.GetGlobalBound().Height;
                }
                else
                {
                    _bounds.Width += bs.GetGlobalBound().Width;
                }
            }

            ObjectsList.Add(bs);
        }

        private Vector2f NextPos(BasicController last)
        {
            Vector2f prev = last.GetPosition();
            FloatRect tmp = last.GetGlobalBound();
            var tmpVect = _vertical ? new Vector2f(_position.X, tmp.Height) : new Vector2f(tmp.Width, _position.Y);
            return prev + tmpVect;
        }

        public void SetPosition(Vector2f pos)
        {
            _bounds.Left = pos.X;
            _bounds.Top = pos.Y;
            Vector2f offset = pos - _position;
            _position = new Vector2f(pos.X, pos.Y);
            foreach (BasicController bs in ObjectsList)
            {
                var last = bs.GetPosition();
                bs.SetPosition(offset + last);
            }
        }

        public void Dragging(Vector2i point)
        {
            ObjectsList.ForEach(t => t.Drag(point));
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            ObjectsList.ForEach(target.Draw);
        }

        public void ProcessClick(MouseButtonEventArgs e)
        {
            ObjectsList.ForEach(t => t.ProcessClick(e));
        }
    }
}