using System;
using Collective_learning.GUI.BasicControllers.Base;
using SFML.Window;
using SFML.System;
using SFML.Graphics;

namespace Collective_learning.GUI.BasicControllers
{
    public class Button : BasicController
    {
        private readonly Sprite _sprite;
        private readonly RectangleShape _shape;
        private readonly Text _text;

        public Vector2f Scale
        {
            get
            {
                return _text.Scale;
            }
            set
            {
                _shape.Scale = value;
                _sprite.Scale = new Vector2f(value.X / _sprite.Scale.X, value.Y / _sprite.Scale.Y);
                _text.Scale = value;
            }
        }
        public string DisplayText => _text.DisplayedString;


        public Button(string text)
        {
            _text = new Text(text, ButtonSettings.DefaultFont, ButtonSettings.TextSize);
            _text.Color = ButtonSettings.TextColor;
            _text.Style = ButtonSettings.TextStyle;
            Vector2f position = new Vector2f(0f, 0f);
            _text.Position = position;

            FloatRect tmpFR = _text.GetGlobalBounds();
            _shape = new RectangleShape();
            _shape.FillColor = ButtonSettings.ShapeColor;
            _shape.Size = new Vector2f(tmpFR.Width + ButtonSettings.TextWidthMargin * 2, ButtonSettings.TextHeightMargin * 2);
            _shape.Position = position;

            _text.Position = new Vector2f(ButtonSettings.TextWidthMargin, ButtonSettings.TextHeightMargin - tmpFR.Height / 2);

            _shape.OutlineColor = ButtonSettings.ShapeOutlineColor;
            _shape.OutlineThickness = 1;

            _sprite = new Sprite();
            _sprite.Texture = ButtonSettings.Texture;
            _sprite.Texture.Smooth = true;
            _sprite.Scale = new Vector2f(_shape.Size.X / ButtonSettings.TextureWidth, _shape.Size.Y / ButtonSettings.TextureHeight);
            _sprite.Position = _shape.Position;

        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_sprite);
            target.Draw(_text);
        }

        public override Vector2f GetPosition()
        {
            return _shape.Position;
        }

        public override void SetPosition(Vector2f pos)
        {
            Vector2f position = pos - _shape.Position;
            _shape.Position += position;
            _sprite.Position += position;
            _text.Position += position;

        }

        public override void OnClick(RenderWindow window, MouseButtonEventArgs args)
        {
            Vector2f arg = window.MapPixelToCoords(new Vector2i(args.X, args.Y));
            if (_sprite.GetGlobalBounds().Contains(arg.X, arg.Y))
            {
                if (_sprite.Texture.Equals(ButtonSettings.Texture))
                    _sprite.Texture = ButtonSettings.TextureOver;
                else
                {
                    _sprite.Texture = ButtonSettings.Texture;
                }
            }

        }

        public override FloatRect GetLocalBound()
        {
            return _shape.GetLocalBounds();
        }

        public override FloatRect GetGlobalBound()
        {
            return _shape.GetGlobalBounds();
        }

        public override void Dispose()
        {
            _text.Dispose();
            _sprite.Dispose();
            _shape.Dispose();
        }
    }
}