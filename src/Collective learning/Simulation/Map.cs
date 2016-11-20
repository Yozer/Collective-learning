using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Graphics;

namespace Collective_learning.Simulation
{
    internal class Map : Drawable, IDisposable
    {
        public int Width { get; }
        public int Height { get; }
        public MapField StartField { get; }

        public MapField[,] Fields { get; set; }

        public Map(StreamReader reader, bool dispose = false)
        {
            try
            {
                string line = reader.ReadLine();

                int[] dim = line.Split(' ').Select(int.Parse).ToArray();
                Width = dim[0];
                Height = dim[1];
                Fields = new MapField[Width, Height];
                
                for (int y = 0; y < Height; ++y)
                {
                    line = reader.ReadLine();
                    for (int x = 0; x < Width; ++x)
                    {
                        Fields[x, y] = new MapField
                        {
                            X = x,
                            Y = y,
                            Type = SimulationOptions.FieldTypes[line[x]]
                        };

                        if (Fields[x, y].Type == FieldType.Start)
                            StartField = Fields[x, y];
                    }
                }
            }
            finally
            {
                if(dispose)
                    reader.Dispose();
            }
        }

        public Map(string path) : this(new StreamReader(new FileStream(path, FileMode.Open)), dispose: true)
        {
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (MapField mapField in Fields)
            {
                target.Draw(mapField);
            }
        }
        public void Dispose()
        {
            foreach (var mapField in Fields)
            {
               mapField.Dispose();
            }
        }
        
    }

    internal enum FieldType
    {
        Empty = 0,
        Food,
        Water,
        Danger,
        Start,
        Blocked
    }
}