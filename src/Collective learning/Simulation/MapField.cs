using System;
using SFML.Graphics;
using SFML.System;

namespace Collective_learning.Simulation
{
    internal class MapField : Drawable, IDisposable
    {
        private int _x, _y;
        private FieldType _type;

        private readonly RectangleShape _rectangle;

        public FieldType Type
        {
            get { return _type; }
            internal set
            {
                _type = value;
                Color color;
                if (_type == FieldType.Empty)
                    color = SimulationOptions.EmptyColor; // gray
                else if (_type == FieldType.Danger)
                    color = SimulationOptions.DangerColor; // red
                else if (_type == FieldType.Food)
                    color = SimulationOptions.FoodColor; // orange
                else if (_type == FieldType.Start)
                    color = SimulationOptions.StartColor; // green
                else if (_type == FieldType.Water)
                    color = SimulationOptions.WaterColor; // blue
                else
                    throw new ArgumentOutOfRangeException(nameof(Type));

                _rectangle.FillColor = color;
            }
        }

        public int X
        {
            get { return _x; }
            internal set
            {
                _x = value;
                _rectangle.Position = new Vector2f(X * SimulationOptions.FieldWidth, Y * SimulationOptions.FieldHeight);
            }
        }
        public int Y
        {
            get { return _y; }
            internal set
            {
                _y = value;
                _rectangle.Position = new Vector2f(X * SimulationOptions.FieldWidth, Y * SimulationOptions.FieldHeight);
            }
        }

        public Vector2f Center => _rectangle.Position;

        public MapField()
        {
            _rectangle = new RectangleShape(new Vector2f(SimulationOptions.FieldWidth, SimulationOptions.FieldHeight));
            _rectangle.Origin = new Vector2f(SimulationOptions.FieldWidth / 2f, SimulationOptions.FieldHeight / 2f);
            _rectangle.OutlineColor = Color.Black;
            _rectangle.OutlineThickness = 1.0f;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_rectangle);
        }

        public void Dispose()
        {
            _rectangle.Dispose();
        }
    }
}