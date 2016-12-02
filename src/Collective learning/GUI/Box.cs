using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Collective_learning.GUI.BasicControllers.Interfaces;

namespace Collective_learning.GUI{

    public class Box : Drawable{

        protected List<BasicController> objectsList;
        private FloatRect _bounds;
        private bool vertical;

        private Vector2f _position;
        public Box(bool vertical ,Vector2f pos){
            this.vertical = vertical;
            objectsList = new List<BasicController>();
            _position = new Vector2f(pos.X,pos.Y);

            _bounds = new FloatRect();
            _bounds.Left = _position.X;
            _bounds.Top = _position.Y;
            _bounds.Width = 0;
            _bounds.Height = 0;

        }

        public Box(bool vertical = true):this(vertical,new Vector2f(0,0)){
        }


        public void AddController(BasicController bs){
            
            if(objectsList.Count == 0){
                bs.SetPosition(_position);
                _bounds.Width +=bs.GetGlobalBound().Width;
                _bounds.Height +=bs.GetGlobalBound().Height;
            }
            else {
                bs.SetPosition(NextPos(objectsList[objectsList.Count-1]));
                if(vertical){
                    _bounds.Height +=bs.GetGlobalBound().Height;
                }
                else {
                    _bounds.Width +=bs.GetGlobalBound().Width;
                }
            }

            objectsList.Add(bs);

        }

        private Vector2f NextPos(BasicController last){
            Vector2f prev = last.GetPosition();
            FloatRect tmp = last.GetGlobalBound();
            Vector2f tmpVect;
            if(vertical){
                tmpVect = new Vector2f(_position.X,tmp.Height);
            }
            else {
                tmpVect = new Vector2f(tmp.Width,_position.Y);
            }
            return prev+tmpVect;

        }

        public void SetPosition(Vector2f pos){
            _bounds.Left = pos.X;
            _bounds.Top = pos.Y;
            Vector2f offset = pos - _position;
            _position = new Vector2f(pos.X,pos.Y);
            foreach(BasicController bs in objectsList){
                var last = bs.GetPosition();
                bs.SetPosition(offset+last);
            }
        }

        public void Dragging(float x,float y,int offsetX){
            foreach(BasicController bs in objectsList){
                if(bs.GetGlobalBound().Contains(x,y)){
                    bs.Drag(x,y,offsetX);
                }
            }

        }
        public FloatRect GetGlobalBound(){
            return _bounds;
        }
        public void MMove(object sender,MouseMoveEventArgs args){
            foreach(BasicController bs in objectsList){
                bs.Move(args);
            }
        }

        public void OnClick(RenderWindow sender,MouseButtonEventArgs args){
            foreach(BasicController bs in objectsList){
                bs.OnClick(sender,args);
            }
        }

        public void Draw(RenderTarget target,RenderStates states){

            foreach(BasicController bs in objectsList){
                target.Draw(bs);
            }

        }


    }



}