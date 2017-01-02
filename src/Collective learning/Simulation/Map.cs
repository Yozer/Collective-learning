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

        private class NodeComparer : IComparer<MapField>
        {
            public int Compare(MapField a, MapField b)
            {
                int result = a.fScore < b.fScore ? -1 : a.fScore == b.fScore ? 0 : 1;
                if (result == 0)
                {
                    result = a._x < b._x ? -1 : a._x == b._x ? 0 : 1;
                    if (result == 0)
                        result = a._y < b._y ? -1 : a._y == b._y ? 0 : 1;
                }

                return result;
            }
        }

        public Queue<MapField> FindPath(MapField start, MapField end, IKnowledge knowledge)
        {
            var closedset = new HashSet<MapField>();
            var openset = new HashSet<MapField>();
            var f_score = new SortedSet<MapField>(new NodeComparer());
            var came_from = new Dictionary<MapField, MapField>();

            foreach (MapField field in Fields)
            {
                field.fScore = field.gScore = float.MaxValue;
            }

            start.gScore = 0;
            start.fScore = Distance(start, end);
            openset.Add(start);
            f_score.Add(start);

            while (openset.Count > 0)
            {
                MapField x = f_score.Min;

                if (x == end)
                {
                    return ReconstructPath(came_from, end);
                }

                openset.Remove(x);
                f_score.Remove(x);
                closedset.Add(x);

                foreach (var y in GetNeighbors(x, knowledge))
                {
                    if (closedset.Contains(y))
                        continue;

                    float tentative_g_score = x.gScore + Distance(x, y);

                    if (!openset.Contains(y))
                    {
                        openset.Add(y);
                        f_score.Add(y);
                    }
                    else if (tentative_g_score >= y.gScore)
                        continue;

                    came_from[y] = x;

                    f_score.Remove(y);
                    y.gScore = tentative_g_score;
                    y.fScore = y.gScore + Distance(y, end);
                    f_score.Add(y);
                }
            }

            float closest = float.MaxValue;
            end = null;
            foreach (var entry in closedset)
            {
                // try to find closest unknown field to end
                if (!knowledge.KnownFields.ContainsKey(entry) && entry.fScore < closest)
                {
                    end = entry;
                    closest = entry.fScore;
                }
            }

            return end == null ? null : ReconstructPath(came_from, end);
        }

        private Queue<MapField> ReconstructPath(Dictionary<MapField, MapField> cameFrom, MapField end)
        {
            var result = new List<MapField> {end};
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
            //return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        private static Vector2i[] array =
        {
            new Vector2i(-1, -1),
            new Vector2i(-1, 0),
            new Vector2i(-1, 1),
            new Vector2i(0, -1),
            new Vector2i(0, 1),
            new Vector2i(1, -1),
            new Vector2i(1, 0),
            new Vector2i(1, 1),
        };
        private IEnumerable<MapField> GetNeighbors(MapField field, IKnowledge knowledge)
        {
            bool isKnownField = knowledge.KnownFields.ContainsKey(field);
            for (int i = 0; i < array.Length; ++i)
            {
                if (field.X + array[i].X >= 0 && field.X + array[i].X < Width && field.Y + array[i].Y >= 0 && field.Y + array[i].Y < Height)
                {
                    var neighbor = Fields[field.X + array[i].X, field.Y + array[i].Y];
                    if (!isKnownField)
                    {
                        if (knowledge.KnownFields.ContainsKey(neighbor) && neighbor.Type != FieldType.Danger && neighbor.Type != FieldType.Blocked)
                            yield return neighbor;
                    }
                    else
                    {
                        if (!knowledge.KnownFields.ContainsKey(neighbor) || (neighbor.Type != FieldType.Danger && neighbor.Type != FieldType.Blocked))
                            yield return neighbor;
                    }
                }
            }
            //var list = new List<Vector2i>(8)
            //{
            //    new Vector2i(field.X - 1, field.Y),
            //    new Vector2i(field.X, field.Y - 1),
            //    new Vector2i(field.X, field.Y + 1),
            //    new Vector2i(field.X + 1, field.Y),
            //};
            //var list = new List<Vector2i>(8)
            //{
            //    new Vector2i(field.X - 1, field.Y - 1),
            //    new Vector2i(field.X - 1, field.Y),
            //    new Vector2i(field.X - 1, field.Y + 1),
            //    new Vector2i(field.X, field.Y - 1),
            //    new Vector2i(field.X, field.Y + 1),
            //    new Vector2i(field.X + 1, field.Y - 1),
            //    new Vector2i(field.X + 1, field.Y),
            //    new Vector2i(field.X + 1, field.Y + 1),
            //};

            //var result = list
            //    .Where(t => t.X >= 0 && t.X < Width && t.Y >= 0 && t.Y < Height)
            //    .Select(t => Fields[t.X, t.Y]);

            //if (!knowledge.KnownFields.ContainsKey(field))
            //    result = result.Where(t => knowledge.KnownFields.ContainsKey(t) && t.Type != FieldType.Danger && t.Type != FieldType.Blocked); // from unknown we can only go to known
            //else
            //    result = result.Where(t => !knowledge.KnownFields.ContainsKey(t) || (t.Type != FieldType.Danger && t.Type != FieldType.Blocked)); // from known we can go anywhere but not negative

            //return result.ToList();
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