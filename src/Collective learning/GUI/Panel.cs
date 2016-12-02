using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Collective_learning.GUI.BasicControllers;


namespace Collective_learning.GUI{

    public class Panel:View,Drawable{

        private RenderWindow _simulationWindow;
        private List<Box> boxList;
        RectangleShape shape;

        private FloatRect _bounds;

        public Panel(FloatRect fr,RenderWindow window):base(fr){
            _simulationWindow = window;
            _bounds = new FloatRect(0,0,0,0);
            boxList = new List<Box>();
            Box box = new Box(true);
            box.AddController(new Slider("test",0,4,1));
            box.AddController(new Slider("Try2 dluzszy",1,2,1));
            box.AddController(new Slider("test3",1,2,1.5f));
            AddBox(box);

            Box box2 = new Box(false);
            box2.AddController(new Button("test"));
            box2.AddController(new Button("Wyzyny2"));
            AddBox(box2);

            shape = new RectangleShape();

            FloatRect frs;

            foreach(Box bx in boxList){
                frs = bx.GetGlobalBound();
                System.Console.Write("Left:"+frs.Left);
                System.Console.Write(" Top:"+frs.Top);
                System.Console.Write(" Width:"+frs.Width);
                System.Console.WriteLine(" Height:"+frs.Height);

            }
        }
        
        private void AddBox(Box box){
            if(boxList == null){
                boxList = new List<Box>();
            }
            box.SetPosition(new Vector2f(_bounds.Left, _bounds.Height+_bounds.Top));
            boxList.Add(box);
            _bounds.Height+=box.GetGlobalBound().Height;
            System.Console.WriteLine(_bounds.Height);


        }


        public void Draw(RenderTarget target,RenderStates states){
            shape.Size = new Vector2f(Size.X,Size.Y);
            shape.FillColor = new Color(70,70,70);
            target.Draw(shape);
            
            foreach(Box box in boxList){
                target.Draw(box);
            }

        }

        public void Dragging(float x, float y, int offsetX){

            foreach(Box box in boxList){
                if(box.GetGlobalBound().Contains(x,y) ){
                    box.Dragging(x,y,offsetX);
                    break;
                }
            }

        }

        public void OnClick(object sender, MouseButtonEventArgs args){
            foreach(Box box in boxList){
                box.OnClick(_simulationWindow,args);
            }
        }

        public FloatRect GetBound(){
            

            return shape.GetGlobalBounds();
        }

    }


}