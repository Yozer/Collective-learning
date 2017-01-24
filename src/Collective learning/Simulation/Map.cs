using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Collective_learning.Simulation.Interfaces;
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

        public Map(string path) : this(new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)), dispose: true)
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

                GetNeighbors(x, knowledge);
                for(int i = 0; i < Size; ++i)
                {
                    MapField y = Array[i];
                    if (closedset.Contains(y))
                        continue;

                    float tentative_g_score = x.gScore + Distance(x, y);

                    if (!openset.Contains(y))
                    {
                        openset.Add(y);
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

        public static float Distance(MapField a, MapField b)
        {
            //return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
            return (float)Math.Sqrt((a._x - b._x) * (a._x - b._x) + (a._y - b._y) * (a._y - b._y));
        }

        private static readonly MapField[] Array = new MapField[8];

        private static int Size = 0;
        private void GetNeighbors(MapField field, IKnowledge knowledge)
        {
            MapField neighbor = null;
            Size = 0;
            bool subX = field._x - 1 >= 0;
            bool subY = field._y - 1 >= 0;
            bool upY = field._y + 1 < Height;
            bool upX = field._x + 1 < Width;

            

            if (subX)
            {
                if (subY)
                {
                    neighbor = Fields[field._x - 1, field._y - 1];
                    if (!knowledge.KnownFields.ContainsKey(neighbor) || (neighbor.Type != FieldType.Danger && neighbor.Type != FieldType.Blocked))
                        Array[Size++] = neighbor;
                }

                neighbor = Fields[field._x - 1, field._y];
                if (!knowledge.KnownFields.ContainsKey(neighbor) || (neighbor.Type != FieldType.Danger && neighbor.Type != FieldType.Blocked))
                    Array[Size++] = neighbor;

                if (upY)
                {
                    neighbor = Fields[field._x - 1, field._y + 1];
                    if (!knowledge.KnownFields.ContainsKey(neighbor) || (neighbor.Type != FieldType.Danger && neighbor.Type != FieldType.Blocked))
                        Array[Size++] = neighbor;
                }
            }

            

            if (upY)
            {
                neighbor = Fields[field._x, field._y + 1];
                if (!knowledge.KnownFields.ContainsKey(neighbor) || (neighbor.Type != FieldType.Danger && neighbor.Type != FieldType.Blocked))
                    Array[Size++] = neighbor;
            }
            if (subY)
            {
                neighbor = Fields[field._x, field._y - 1];
                if (!knowledge.KnownFields.ContainsKey(neighbor) || (neighbor.Type != FieldType.Danger && neighbor.Type != FieldType.Blocked))
                    Array[Size++] = neighbor;
            }
            

            if (upX)
            {
                if (subY)
                {
                    neighbor = Fields[field._x + 1, field._y - 1];
                    if (!knowledge.KnownFields.ContainsKey(neighbor) || (neighbor.Type != FieldType.Danger && neighbor.Type != FieldType.Blocked))
                        Array[Size++] = neighbor;
                }

                neighbor = Fields[field._x + 1, field._y];
                if (!knowledge.KnownFields.ContainsKey(neighbor) || (neighbor.Type != FieldType.Danger && neighbor.Type != FieldType.Blocked))
                    Array[Size++] = neighbor;

                if (upY)
                {
                    neighbor = Fields[field._x + 1, field._y + 1];
                    if (!knowledge.KnownFields.ContainsKey(neighbor) || (neighbor.Type != FieldType.Danger && neighbor.Type != FieldType.Blocked))
                        Array[Size++] = neighbor;
                }
            }
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