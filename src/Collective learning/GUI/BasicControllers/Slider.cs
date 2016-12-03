using System;
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
        private float _currentValue;

        private Vector2f _position;

        public Slider(string text, float fromV, float toV, float initValue)
        {
            _valueFrom = fromV;
            _valueTo = toV;
            _currentValue = initValue;
            _position = new Vector2f(0, 0);

            _text = new Text(text, SliderSettings.DefaultFont, SliderSettings.TextSize);
            _text.Color = SliderSettings.TextColor;
            _text.Style = SliderSettings.TextStyle;

            _valueText = new Text(initValue.ToString("0.000"), SliderSettings.DefaultFont, SliderSettings.TextSize);
            _valueText.Color = SliderSettings.TextColor;
            _valueText.Style = SliderSettings.TextStyle;

            FloatRect valueFr = _valueText.GetGlobalBounds();

            _valueText.Position = new Vector2f(0, SliderSettings.TextHeightMargin);

            _bar = new RectangleShape();
            _bar.FillColor = SliderSettings.BarColor;
            _bar.OutlineColor = Color.Black;
            _bar.OutlineThickness = 1;
            _bar.Size = new Vector2f(SliderSettings.BarWidth, SliderSettings.BarHeight);
            _bar.Position = new Vector2f(SliderSettings.TextSize + valueFr.Width, SliderSettings.TextHeightMargin + SliderSettings.BarHeight);

            _scroll = new RectangleShape();
            _scroll.Origin = new Vector2f(SliderSettings.ScrollWidth / 2f, 0);
            _scroll.FillColor = SliderSettings.ScrollColor;
            _scroll.OutlineColor = Color.Black;
            _scroll.OutlineThickness = 1;
            _scroll.Size = new Vector2f(SliderSettings.ScrollWidth, SliderSettings.ScrollHeight);
            _scroll.Position = _bar.Position + (new Vector2f(0, -SliderSettings.BarHeight));
            _scroll.Position += new Vector2f(((initValue - fromV) / (toV - fromV)) * SliderSettings.BarWidth, 0);

            _text.Position = new Vector2f((_bar.Position.X + SliderSettings.BarWidth - _valueText.Position.X - _text.GetGlobalBounds().Width) / 2, 0);
        }

        public override void Move(MouseMoveEventArgs args)
        {

        }

        public override void Dispose()
        {
            _valueText.Dispose();
        }

        public override void OnClick(RenderWindow window, MouseButtonEventArgs args)
        {
            FloatRect fr = _scroll.GetGlobalBounds();
            Vector2f pos = window.MapPixelToCoords(new Vector2i(args.X, args.Y));

        }

        private void UpdateTextValue()
        {
            FloatRect bound = _bar.GetGlobalBounds();
            float from = bound.Left;
            float to = bound.Left + bound.Width;
            float actual = _scroll.Position.X;

            float percantage = (actual - from) / (to - from);
            float value = (_valueTo - _valueFrom) * percantage + _valueFrom;
            _valueText.DisplayedString = value.ToString("0.000");


        }

        public override void Drag(float x, float y, int offsetX)
        {
            if (_scroll.GetGlobalBounds().Contains(x, y))
            {
                _scroll.Position += new Vector2f(offsetX, 0);
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
            FloatRect result = new FloatRect();
            result.Left = _position.X;
            result.Top = _position.Y;

            float height = _scroll.GetGlobalBounds().Top + _scroll.GetGlobalBounds().Height - result.Top;
            float width = _bar.GetGlobalBounds().Left + _bar.GetGlobalBounds().Width - result.Left + _scroll.GetGlobalBounds().Width / 2;
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

