using System;
using System.IO;
using SFML.Graphics;
using SFML.System;

namespace Collective_learning.Simulation
{
    public class MapField : Drawable, IDisposable, IEquatable<MapField>
    {
        private static readonly Font MyFont = new Font(new Font(Path.Combine("Assets", "Arial.ttf")));

        private int _x, _y;
        private FieldType _type;

        private readonly RectangleShape _rectangle;
        private bool _darker;

        private int _resourceCount = SimulationOptions.ResourceCount;
        private readonly Text _text;


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
                _text.Position = _rectangle.Position;
            }
        }
        public int Y
        {
            get { return _y; }
            internal set
            {
                _y = value;
                _rectangle.Position = new Vector2f(X * SimulationOptions.FieldWidth, Y * SimulationOptions.FieldHeight);
                _text.Position = _rectangle.Position - _rectangle.Origin;
            }
        }

        public Vector2f Center => _rectangle.Position;

        public Color SpecialColor
        {
            set { _rectangle.FillColor = value == default(Color) ? SimulationOptions.FieldColors[_type] : value; _darker = false; }
        }

        public bool Darker
        {
            get { return _darker; }
            set
            {
                if(value && !_darker)
                    _rectangle.FillColor = new Color((byte) (_rectangle.FillColor.R * 0.69), (byte) (_rectangle.FillColor.G *0.69), (byte) (_rectangle.FillColor.B*0.69));
                _darker = value;
            }
        }

        public MapField()
        {
            _rectangle = new RectangleShape(new Vector2f(SimulationOptions.FieldWidth, SimulationOptions.FieldHeight));
            _rectangle.Origin = new Vector2f(SimulationOptions.FieldWidth / 2f, SimulationOptions.FieldHeight / 2f);
            _rectangle.OutlineColor = Color.Black;
            _rectangle.OutlineThickness = 1.0f;

            _text = new Text(_resourceCount.ToString(), MyFont, 12u) {Color = Color.Black};
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_rectangle);

            if (Type == FieldType.Food || Type == FieldType.Water)
            {
                _text.Draw(target, states);
            }
        }

        public void Consume()
        {
            if (Type != FieldType.Food && Type != FieldType.Water)
                return;

            --_resourceCount;
            _text.DisplayedString = _resourceCount.ToString();

            if(_resourceCount == 0)
                Type = FieldType.Empty;
        }

        public void Dispose()
        {
            _rectangle.Dispose();
            _text.Dispose();
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