using Collective_learning.GUI.BasicControllers.Base;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Collective_learning.GUI.BasicControllers
{
    public class Slider : BasicController
    {
        private readonly Text _valueText;
        private readonly Text _text;

        private readonly RectangleShape _scroll;
        private readonly RectangleShape _bar;
        private readonly float _valueFrom;
        private readonly float _valueTo;

        private Vector2f _position;
        private Vector2i _lastDragPoint;

        public delegate void ChangedEventHandler(float value);

        public event ChangedEventHandler OnChange;

        public Slider(string text, float fromV, float toV, float initValue)
        {
            _valueFrom = fromV;
            _valueTo = toV;
            _position = new Vector2f(0, 0);

            _text = new Text(text, SliderSettings.DefaultFont, SliderSettings.TextSize)
            {
                Color = SliderSettings.TextColor,
                Style = SliderSettings.TextStyle
            };

            _valueText = new Text(initValue.ToString("0.000"), SliderSettings.DefaultFont, SliderSettings.TextSize)
            {
                Color = SliderSettings.TextColor,
                Style = SliderSettings.TextStyle
            };

            FloatRect valueFr = _valueText.GetGlobalBounds();

            _valueText.Position = new Vector2f(0, SliderSettings.TextHeightMargin);

            _bar = new RectangleShape
            {
                FillColor = SliderSettings.BarColor,
                OutlineColor = Color.Black,
                OutlineThickness = 1,
                Size = new Vector2f(SliderSettings.BarWidth, SliderSettings.BarHeight),
                Position = new Vector2f(SliderSettings.TextSize + valueFr.Width, SliderSettings.TextHeightMargin + SliderSettings.BarHeight)
            };

            _scroll = new RectangleShape
            {
                Origin = new Vector2f(SliderSettings.ScrollWidth/2f, 0),
                FillColor = SliderSettings.ScrollColor,
                OutlineColor = Color.Black,
                OutlineThickness = 1,
                Size = new Vector2f(SliderSettings.ScrollWidth, SliderSettings.ScrollHeight),
                Position = _bar.Position + (new Vector2f(0, -SliderSettings.BarHeight))
            };

            _scroll.Position += new Vector2f(((initValue - fromV)/(toV - fromV))*SliderSettings.BarWidth, 0);
            _text.Position = new Vector2f((_bar.Position.X + SliderSettings.BarWidth - _valueText.Position.X - _text.GetGlobalBounds().Width)/2, 0);
        }

        public override void Dispose()
        {
            _valueText.Dispose();
        }

        private void UpdateTextValue()
        {
            FloatRect bound = _bar.GetGlobalBounds();
            float from = bound.Left;
            float to = bound.Left + bound.Width;
            float actual = _scroll.Position.X;

            float percentage = (actual - from)/(to - from);
            float value = (_valueTo - _valueFrom)*percentage + _valueFrom;
            _valueText.DisplayedString = value.ToString("0.000");

            OnChange?.Invoke(value);
        }

        public override void Drag(Vector2i point)
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left) && ((_lastDragPoint == default(Vector2i) && _scroll.GetGlobalBounds().Contains(point.X, point.Y))
                                                             || (_lastDragPoint != default(Vector2i) && _bar.GetGlobalBounds().Left - 10 <= point.X && _bar.GetGlobalBounds().Left + _bar.Size.X + 10 >= point.X)))
            {
                if (_lastDragPoint != default(Vector2i))
                {
                    var offset = _lastDragPoint - point;
                    _scroll.Position -= new Vector2f(offset.X, 0);

                    FloatRect barBounds = _bar.GetGlobalBounds();
                    if (_scroll.Position.X < barBounds.Left)
                    {
                        _scroll.Position = new Vector2f(barBounds.Left, _scroll.Position.Y);
                    }
                    else if (_scroll.Position.X > (barBounds.Left + barBounds.Width))
                    {
                        _scroll.Position = new Vector2f(barBounds.Left + barBounds.Width, _scroll.Position.Y);
                    }
                    UpdateTextValue();
                }

                _lastDragPoint = point;
            }
            else
            {
                _lastDragPoint = default(Vector2i);
            }
        }

        public override void SetPosition(Vector2f pos)
        {
            Vector2f offset = pos - _position;
            _position = new Vector2f(pos.X, pos.Y);
            _valueText.Position = _valueText.Position + offset;
            _bar.Position += offset;
            _scroll.Position += offset;
            _text.Position += offset;
        }

        public override Vector2f GetPosition()
        {
            return new Vector2f(_position.X, _position.Y);
        }

        public override FloatRect GetGlobalBound()
        {
            FloatRect result = new FloatRect
            {
                Left = _position.X,
                Top = _position.Y
            };

            float height = _scroll.GetGlobalBounds().Top + _scroll.GetGlobalBounds().Height - result.Top;
            float width = _bar.GetGlobalBounds().Left + _bar.GetGlobalBounds().Width - result.Left + _scroll.GetGlobalBounds().Width/2;
            result.Height = height;
            result.Width = width;
            return result;
        }

        public override FloatRect GetLocalBound()
        {
            return _text.GetLocalBounds();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_text);
            target.Draw(_valueText);
            target.Draw(_bar);
            target.Draw(_scroll);
        }
    }
}