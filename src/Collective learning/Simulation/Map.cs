using System;
using System.IO;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace Collective_learning.Simulation
{
    internal class Map : Drawable, IDisposable
    {
        private readonly MapField[,] _fields;
        public int Width { get; private set; }
        public int Height { get; private set; }

        private Map(int width, int height, MapField[,] fields)
        {
            Width = width;
            Height = height;
            _fields = fields;
        }

        internal static Map Load(StreamReader reader)
        {
            string line = reader.ReadLine();
            int[] dim = line.Split(' ').Select(int.Parse).ToArray();
            var fields = new MapField[dim[0], dim[1]];

            for (int x = 0; x < dim[0]; ++x)
            {
                line = reader.ReadLine();
                for (int y = 0; y < dim[1]; ++y)
                {
                    int type = line[y] - '0';
                    fields[x, y] = new MapField
                    {
                        X = x,
                        Y = y,
                        Type = (FieldType) type
                    };
                }
            }

            return new Map(dim[0], dim[1], fields);
        }

        internal static Map Load(string path)
        {
            using (var sr = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                return Load(sr);
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (MapField mapField in _fields)
            {
                target.Draw(mapField);
            }
        }
        public void Dispose()
        {
            foreach (var mapField in _fields)
            {
               mapField.Dispose();
            }
        }
        internal class MapField : Drawable, IDisposable
        {
            private const int Width = 48;
            private const int Height = 48;

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
                    if(_type == FieldType.Empty)
                        color = new Color(110, 104, 109);
                    else if(_type == FieldType.Danger)
                        color = new Color(249, 39, 39);
                    else if(_type == FieldType.Food)
                        color = new Color(252, 122, 35);
                    else if(_type == FieldType.Start)
                        color = new Color(16, 183, 19);
                    else if(_type == FieldType.Water)
                        color = new Color(37, 226, 247);
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
                    _rectangle.Position = new Vector2f(X*Width, Y*Height);
                }
            }
            public int Y
            {
                get { return _y; }
                internal set
                {
                    _y = value;
                    _rectangle.Position = new Vector2f(X*Width, Y*Height);
                }
            }

            public MapField()
            {
                _rectangle = new RectangleShape(new Vector2f(Width, Height));
                _rectangle.Origin = new Vector2f(0, 0);
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

    internal enum FieldColor
    {
        
    }

    internal enum FieldType
    {
        Empty = 0,
        Food,
        Water,
        Danger,
        Start
    }
}