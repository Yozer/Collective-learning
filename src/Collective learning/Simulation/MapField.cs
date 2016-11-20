using System;
using SFML.Graphics;
using SFML.System;

namespace Collective_learning.Simulation
{
    public class MapField : Drawable, IDisposable, IEquatable<MapField>
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
                _rectangle.FillColor = SimulationOptions.FieldColors[_type];
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

        public Color SpecialColor
        {
            set { _rectangle.FillColor = value == default(Color) ? SimulationOptions.FieldColors[_type] : value; }
        }

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

        public bool Equals(MapField other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MapField))
                return false;
            return Equals((MapField)obj);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = ((hash + X) << 5) - (hash + X);
            hash = ((hash + Y) << 5) - (hash + Y);
            return hash;
        }

        public static bool operator ==(MapField a, MapField b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;
            return a.Equals(b);
        }

        public static bool operator !=(MapField a, MapField b)
        {
            return !(a == b);
        }
    }
}