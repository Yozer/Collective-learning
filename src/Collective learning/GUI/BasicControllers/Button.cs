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

        public delegate void ClickEventHandler();
        public event ClickEventHandler OnClick;
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
            _shape.Size = new Vector2f(tmpFR.Width + ButtonSettings.TextWidthMargin * 2f, ButtonSettings.TextHeightMargin * 2f);
            _shape.Position = position;

            _text.Position = new Vector2f(ButtonSettings.TextWidthMargin, (_shape.Size.Y - ButtonSettings.TextHeightMargin) / 2f);

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

        public override void ProcessClick(MouseButtonEventArgs args)
        {
            if (_shape.GetGlobalBounds().Contains(args.X, args.Y))
            {
                _sprite.Texture = _sprite.Texture.Equals(ButtonSettings.Texture) ? ButtonSettings.TextureOver : ButtonSettings.Texture;
                OnClick?.Invoke();
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