using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Collective_learning.Simulation.Interfaces;
using SFML.Graphics;
using SFML.System;

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

        public Queue<MapField> FindPath(MapField start, MapField end, IKnowledge knowledge)
        {
            var closedset = new HashSet<MapField>();
            var openset = new HashSet<MapField>();
            var g_score = new Dictionary<MapField, float>();
            var h_score = new Dictionary<MapField, float>();
            var f_score = new Dictionary<MapField, float>();
            var came_from = new Dictionary<MapField, MapField>();

            openset.Add(start);
            g_score.Add(start, 0);

            while (openset.Count > 0)
            {
                MapField x = openset.First();
                foreach (MapField field in openset)
                {
                    if (f_score.ContainsKey(field) && f_score[field] < f_score[x])
                        x = field;
                }

                if (x == end)
                {
                    return ReconstructPath(came_from, end);
                }

                openset.Remove(x);
                closedset.Add(x);

                foreach (var y in GetNeighbors(x, knowledge))
                {
                    if (closedset.Contains(y)) continue;

                    float tentative_g_score = g_score[x] + Distance(x, y);
                    bool tentative_is_better = false;

                    if (!openset.Contains(y))
                    {
                        openset.Add(y);
                        h_score[y] = Distance(y, end);
                        tentative_is_better = true;
                    }
                    else if (tentative_g_score < g_score[y])
                        tentative_is_better = true;

                    if (tentative_is_better)
                    {
                        came_from[y] = x;
                        g_score[y] = tentative_g_score;
                        f_score[y] = g_score[y] + h_score[y];
                    }
                }
            }

            float closest = float.MaxValue;
            end = null;
            foreach (var entry in f_score)
            {
                // try to find closest unknown field to end
                if (!knowledge.KnownFields.Contains(entry.Key) && entry.Value < closest)
                {
                    end = entry.Key;
                    closest = entry.Value;
                }
            }

            return end == null ? null : ReconstructPath(came_from, end);
        }

        private Queue<MapField> ReconstructPath(Dictionary<MapField, MapField> cameFrom, MapField end)
        {
            var result = new List<MapField>();
            result.Add(end);
            while (cameFrom.ContainsKey(end))
            {
                end = cameFrom[end];
                result.Add(end);
            }

            result.Reverse();
            return new Queue<MapField>(result.Skip(1));
        }

        private float Distance(MapField a, MapField b)
        {
            return (float) Math.Sqrt((a.X - b.X)*(a.X - b.X) + (a.Y - b.Y)*(a.Y - b.Y));
        }

        private List<MapField> GetNeighbors(MapField field, IKnowledge knowledge)
        {
            var list = new List<Vector2i>(8)
            {
                new Vector2i(field.X - 1, field.Y - 1),
                new Vector2i(field.X - 1, field.Y),
                new Vector2i(field.X - 1, field.Y + 1),
                new Vector2i(field.X, field.Y - 1),
                new Vector2i(field.X, field.Y + 1),
                new Vector2i(field.X + 1, field.Y - 1),
                new Vector2i(field.X + 1, field.Y),
                new Vector2i(field.X + 1, field.Y + 1),
            };

            var result = list
                .Where(t => t.X >= 0 && t.X < Width && t.Y >= 0 && t.Y < Height)
                .Select(t => Fields[t.X, t.Y]);

            if (!knowledge.KnownFields.Contains(field))
                result = result.Where(t => knowledge.KnownFields.Contains(t) && !knowledge.Negative.Contains(t) && !knowledge.Blocked.Contains(t)); // from unknown we can only go to known
            else
                result = result.Where(t => !knowledge.KnownFields.Contains(t) || (!knowledge.Negative.Contains(t) && !knowledge.Blocked.Contains(t))); // from known we can go anywhere but not negative

            return result.ToList();
        }
    }

    public enum FieldType
    {
        Empty = 0,
        Food,
        Water,
        Danger,
        Start,
        Blocked
    }
}