using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Collective_learning.GUI.BasicControllers.Interfaces;

namespace Collective_learning.GUI.BasicControllers{

    public class Slider:BasicController{

        private Text _valueText;

        private RectangleShape _scroll;
        private RectangleShape _bar;
        private float valueFrom;
        private float valueTo;
        private float currentValue;

        private Vector2f _position;

        public Slider(String text,float fromV,float toV,float initValue){
            valueFrom = fromV;
            valueTo = toV;
            currentValue = initValue;
            _position = new Vector2f(0,0);

            _text = new Text(text,SliderSettings.DefaultFont,SliderSettings.TextSize);
            _text.Color = SliderSettings.TextColor;
            _text.Style = SliderSettings.TextStyle;
            
            _valueText = new Text(initValue.ToString("0.000"),SliderSettings.DefaultFont,SliderSettings.TextSize);
            _valueText.Color = SliderSettings.TextColor;
            _valueText.Style = SliderSettings.TextStyle;

            FloatRect valueFR = _valueText.GetGlobalBounds();

            _valueText.Position = new Vector2f(0,SliderSettings.TextHeightMargin);

            _bar = new RectangleShape();
            _bar.FillColor = SliderSettings.BarColor;
            _bar.OutlineColor = Color.Black;
            _bar.OutlineThickness = 1;
            _bar.Size = new Vector2f(SliderSettings.BarWidth,SliderSettings.BarHeight);
            _bar.Position = new Vector2f(SliderSettings.TextSize+ valueFR.Width,SliderSettings.TextHeightMargin+SliderSettings.BarHeight);

            _scroll = new RectangleShape();
            _scroll.Origin = new Vector2f(SliderSettings.ScrollWidth/2,0);
            _scroll.FillColor = SliderSettings.ScrollColor;
            _scroll.OutlineColor = Color.Black;
            _scroll.OutlineThickness = 1;
            _scroll.Size = new Vector2f(SliderSettings.ScrollWidth,SliderSettings.ScrollHeight);
            _scroll.Position = _bar.Position + ( new Vector2f(0,-SliderSettings.BarHeight));
            _scroll.Position += new Vector2f(((initValue-fromV)/(toV-fromV))*SliderSettings.BarWidth,0);

            _text.Position = new Vector2f((_bar.Position.X+SliderSettings.BarWidth -_valueText.Position.X-_text.GetGlobalBounds().Width)/2,0);
        }


        override public void Move(MouseMoveEventArgs args){
            
        }
        override public void OnClick(RenderWindow window,MouseButtonEventArgs args)
        {
                FloatRect fr = _scroll.GetGlobalBounds();

                Vector2f pos = window.MapPixelToCoords(new Vector2i(args.X,args.Y));

        }

        private void UpdateTextValue(){
            FloatRect bound = _bar.GetGlobalBounds();
            float from = bound.Left;
            float to = bound.Left+bound.Width;
            float actual = _scroll.Position.X;

            float percantage = (actual-from)/(to-from);
            float value = (valueTo-valueFrom)*percantage+valueFrom;
            _valueText.DisplayedString = value.ToString("0.000");


        }

        override public void Drag(float x,float y, int offsetX){
            if(_scroll.GetGlobalBounds().Contains(x,y)){
                _scroll.Position += new Vector2f(offsetX,0);
                FloatRect barBounds = _bar.GetGlobalBounds();
                if(_scroll.Position.X<barBounds.Left){
                    _scroll.Position = new Vector2f(barBounds.Left,_scroll.Position.Y);
                }
                else if(_scroll.Position.X>(barBounds.Left+barBounds.Width)){
                    _scroll.Position = new Vector2f(barBounds.Left+barBounds.Width,_scroll.Position.Y);

                }
                UpdateTextValue();
            }
        }

        override public void SetPosition(Vector2f pos){
            FloatRect valueFR = _valueText.GetGlobalBounds();
            Vector2f offset = pos - _position;
            _position = new Vector2f(pos.X,pos.Y);
            _valueText.Position = _valueText.Position+offset;
            _bar.Position +=offset;
            _scroll.Position +=offset;
            _text.Position +=offset;


        }
        override public Vector2f GetPosition(){

            return new Vector2f(_position.X,_position.Y);
        }
        override public FloatRect GetGlobalBound(){
            FloatRect result = new FloatRect();
            result.Left = _position.X;
            result.Top = _position.Y;

            float height = _scroll.GetGlobalBounds().Top+_scroll.GetGlobalBounds().Height-result.Top;
            float width = _bar.GetGlobalBounds().Left+_bar.GetGlobalBounds().Width-result.Left+_scroll.GetGlobalBounds().Width/2;
            result.Height = height;
            result.Width = width;
            return result;
        }
        override public FloatRect GetLocalBound(){
            
         return _text.GetLocalBounds();   
        }

        override public void Draw(RenderTarget target, RenderStates states){
            target.Draw(_text);
            target.Draw(_valueText);
            target.Draw(_bar);
            target.Draw(_scroll);
        }


    }

}

