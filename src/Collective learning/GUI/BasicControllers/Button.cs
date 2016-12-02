using System;
using System.IO;
using SFML.Window;
using SFML.System;
using SFML.Graphics;
using Collective_learning.GUI.BasicControllers.Interfaces;

namespace Collective_learning.GUI.BasicControllers{

    public class Button : BasicController{

        
        public Sprite _sprite;

        
        private RectangleShape _shape;

        public Vector2f Scale
        {
            get
            {
                return _text.Scale;
            }

            set
            {
                _shape.Scale = value;
                _sprite.Scale = new Vector2f(value.X/_sprite.Scale.X,value.Y/_sprite.Scale.Y);
                _text.Scale = value;
            }
        }

        public string Text
        {
            get
            {
                return _text.DisplayedString;
            }
        }


        public Button(String text){
            _text = new Text(text,ButtonSettings.DefaultFont,ButtonSettings.TextSize);
            _text.Color = ButtonSettings.TextColor;
            _text.Style = ButtonSettings.TextStyle;
            Vector2f position = new Vector2f(0f,0f);
            _text.Position = position;

            FloatRect tmpFR = _text.GetGlobalBounds();
            _shape = new RectangleShape();
            _shape.FillColor = ButtonSettings.ShapeColor;
            _shape.Size = new Vector2f(tmpFR.Width+ButtonSettings.TextWidthMargin*2,ButtonSettings.TextHeightMargin*2);
            _shape.Position = position;

            _text.Position = new Vector2f(ButtonSettings.TextWidthMargin,ButtonSettings.TextHeightMargin-tmpFR.Height/2);

            _shape.OutlineColor = ButtonSettings.ShapeOutlineColor;
            _shape.OutlineThickness = 1;

            _sprite = new Sprite();
            _sprite.Texture = ButtonSettings.Texture;
            _sprite.Texture.Smooth = true;
            _sprite.Scale = new Vector2f(_shape.Size.X/ButtonSettings.TextureWidth,_shape.Size.Y/ButtonSettings.TextureHeight);
            _sprite.Position = _shape.Position;

        }




        override public void Draw(RenderTarget target, RenderStates states){
            //target.Draw(_shape);
            target.Draw(_sprite);
            target.Draw(_text);
        }

        override public Vector2f GetPosition(){
            return _shape.Position;
        }

        override public void SetPosition(Vector2f pos){
            Vector2f position = pos - _shape.Position;
            _shape.Position += position;
            FloatRect tmpFR = _text.GetGlobalBounds();
            _sprite.Position += position;
            _text.Position += position;

        }

        override public void Move(MouseMoveEventArgs args){
            if(_sprite.GetGlobalBounds().Contains(args.X,args.Y)){
                _sprite.Texture = ButtonSettings.TextureOver;
            }
            else {
                _sprite.Texture = ButtonSettings.Texture;
            }

        }

        override public void OnClick(RenderWindow window,MouseButtonEventArgs args)
        {
            Vector2f arg = window.MapPixelToCoords(new Vector2i(args.X,args.Y));
            if(_sprite.GetGlobalBounds().Contains(arg.X,arg.Y)){
                if(_sprite.Texture.Equals(ButtonSettings.Texture))
                    _sprite.Texture = ButtonSettings.TextureOver;
                else{
                    _sprite.Texture = ButtonSettings.Texture;       
                }
            }

        }

        override public void Drag(float x,float y, int offsetX){

        }

        override public FloatRect GetLocalBound(){
            return _shape.GetLocalBounds();
        }

        override public FloatRect GetGlobalBound(){
            return _shape.GetGlobalBounds();
        }

        public void Dispose()
        {
            _text.Dispose();
        }

    }



}