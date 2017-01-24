using System;
using System.Collections.Generic;
using System.Linq;
using Collective_learning.Simulation.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Collective_learning.Simulation
{
    internal class Agent : IAgent
    {
        private static int _internalId = 0;

        private readonly Map _map;
        private readonly CircleShape _circleShape = new CircleShape(SimulationOptions.AgentRadius);

        private Vector2f _movementDirection = new Vector2f(0, 0);
        private MapField _nextField;
        private bool _selected;

        public MapField CurrentField { get; private set; }
        public MapField TargetField { get; private set; }
        public MapField NextField
        {
            get { return _nextField; }
            private set
            {
                _nextField = value;
                if (_nextField != null)
                {
                    _movementDirection = _nextField.Center - Position;
                    _movementDirection /= (float) Math.Sqrt(_movementDirection.X*_movementDirection.X + _movementDirection.Y * _movementDirection.Y);
                    if((Math.Abs(_movementDirection.X) < 0.001 && Math.Abs(_movementDirection.Y) < 0.001) || (float.IsNaN(_movementDirection.X) || float.IsNaN(_movementDirection.Y)))
                        throw new InvalidOperationException("Movement vector is invalid");
                }
                else
                {
                    _movementDirection = new Vector2f(0, 0);
                }
            }
        }

        public Queue<MapField> Path { get; private set; } = new Queue<MapField>();
        public IKnowledge Knowledge { get; }
        public Vector2f Position => _circleShape.Position;
        public CircleShape Bounds => _circleShape;
        public SimulationStatistics Statistics { get; }
        public int Id { get; }
        public int? CollidedAt { get; set; }
        public int SimulationStep { get; private set; } = 0;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                _circleShape.FillColor = _selected ? SimulationOptions.SelectedAgentColor : SimulationOptions.AgentColor;
            }
        }

        public Agent(Map map, Knowledge globalKnowledge, SimulationStatistics statistics)
        {
            _map = map;
            CurrentField = map.StartField;
            _circleShape.FillColor = SimulationOptions.AgentColor;
            _circleShape.Origin = new Vector2f(_circleShape.Radius, _circleShape.Radius);
            _circleShape.Position = map.StartField.Center;
            _circleShape.OutlineThickness = 1.0f;
            _circleShape.OutlineColor = Color.Black;

            Knowledge = globalKnowledge ?? new Knowledge(map.Fields.Length)
            {
                UnknownFields = new HashSet<MapField>(map.Fields.Cast<MapField>().Where(t => t != CurrentField))
            };
            Statistics = statistics ?? new SimulationStatistics();
            Knowledge.KnownFields[map.StartField] = DateTime.Now;
            Id = _internalId++;

            CollidedAt = 1;
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            if (CollidedAt.HasValue && CollidedAt + SimulationOptions.SharingKnowledgePenalty > SimulationStep)
                _circleShape.FillColor = Color.Magenta;
            else
                Selected = _selected; // reset selection color

            target.Draw(_circleShape);
        }

        public void Update(float delta)
        {
            ++SimulationStep;
            if (CollidedAt.HasValue)
            {
                if(CollidedAt.Value + SimulationOptions.SharingKnowledgePenalty > SimulationStep)
                    return;
                else if (CollidedAt.Value + SimulationOptions.SharingKnowledgePenalty + SimulationOptions.NoSharingPeriodAfterSharingKnowledge < SimulationStep)
                    CollidedAt = null;
            }
            if (TargetField == null || Knowledge.Negative.ContainsKey(TargetField) || Knowledge.Blocked.ContainsKey(TargetField))
            {
                NextField = TargetField = null;
                ChooseTarget();
                InvalidatePathToTarget();
            }
            if ((NextField != null && (Knowledge.Negative.ContainsKey(NextField) || Knowledge.Blocked.ContainsKey(NextField))))
            {
                NextField = null;
                InvalidatePathToTarget();
            }
            if (TargetField != null && (Knowledge.UnknownFields.Count > 0 || Knowledge.Positive.Count > 0) && TargetField.Type == FieldType.Empty && Knowledge.KnownFields.ContainsKey(TargetField))
            {
                NextField = TargetField = null;
                ChooseTarget();
                InvalidatePathToTarget();
            }
            Move(delta);
        }

        private void ChooseTarget()
        {
            // if we don't know any positive fields or we want to explore
            MapField fieldToExplore;
            bool anyPositiveIsKnown = Knowledge.Positive.Count > 1 || (Knowledge.Positive.Count == 1 && Knowledge.Positive.Keys.All(t => t != CurrentField));
            if ((!anyPositiveIsKnown || SimulationOptions.Random.NextDouble() <= SimulationOptions.ExplorationThreshold)
                && (fieldToExplore = ChooseUnknownField()) != null)
            {
                TargetField = fieldToExplore;
            }
            else if (Knowledge.Positive.Count == 0)
            {
                var list = Knowledge.KnownFields.Keys.Where(t => t.Type != FieldType.Danger && t != CurrentField && t.Type != FieldType.Blocked).ToList();
                TargetField = list[SimulationOptions.Random.Next(0, list.Count)];
            }
            else
            {
                // just visit something positive, that is close to me
                //TargetField = Knowledge.Positive.Where(t => t != CurrentField).MinBy(t => _map.FindPath(CurrentField, t, Knowledge)?.Count ?? int.MaxValue);
                TargetField = Knowledge.Positive.Keys.Where(t => t != CurrentField).MinBy(t => Math.Sqrt((t._x-CurrentField._x)*(t._x-CurrentField._x)+(t._y-CurrentField._y)*(t._y-CurrentField._y)));
            }
        }

        private void InvalidatePathToTarget()
        {
            Path = _map.FindPath(CurrentField, TargetField, Knowledge);
            if (Path == null)
            {
                // we know entire neighborhood of this place and we still were unable to find a path thus we cannot reach that place.
                Knowledge.KnownFields[TargetField] = DateTime.Now;
                Knowledge.Blocked[TargetField] = DateTime.Now;
                Knowledge.UnknownFields.Remove(TargetField);
                ChooseTarget();
                InvalidatePathToTarget();
            }

            var movementDirection = Path.Peek().Center - Position;
            if (Math.Abs(movementDirection.X) < 0.001 && Math.Abs(movementDirection.Y) < 0.001)
            {
                var nextField = Path.Dequeue();
                _circleShape.Position = nextField.Center;
                CurrentField = nextField;
                NextField = null;
                UpdateKnowledge(CurrentField);

                if (CurrentField == TargetField)
                {
                    OnTargetReached();
                    if (TargetField == null)
                    {
                        NextField = TargetField = null;
                        ChooseTarget();
                        InvalidatePathToTarget();
                    }
                }
            }
        }

        private void Move(float delta)
        {
            // where should I move next?
            if (NextField == null)
            {
                if(Path.Count > 0)
                    NextField = Path.Dequeue(); // ok I know where to move next
                else if (TargetField != CurrentField) // hmm, I didn't reach my target and I don't know where should I move next, InvalidatePath
                {
                    InvalidatePathToTarget();
                    if (Path.Count > 0)
                        NextField = Path.Dequeue(); // now I know where to go
                }

                // after choosing next field correctly
                if (NextField != null)
                {
                    // update knowledge
                    // check if we can move there
                    if (NextField.Type == FieldType.Blocked)
                    {
                        UpdateKnowledge(NextField);
                        // if that was our target, choose new one
                        if (TargetField == NextField)
                        {
                            ChooseTarget();
                        }

                        InvalidatePathToTarget();
                        NextField = null;
                        return;
                    }
                }
            }

            if (NextField != null)
            {
                _circleShape.Position += _movementDirection*SimulationOptions.AgentSpeed;
                var diff = _circleShape.Position - NextField.Center;

                if (diff.X*_movementDirection.X + diff.Y*_movementDirection.Y > 0)
                {
                    _circleShape.Position = NextField.Center;
                    CurrentField = NextField;
                    NextField = null;
                    UpdateKnowledge(CurrentField);

                    if (CurrentField == TargetField)
                    {
                        OnTargetReached();
                    }
                }
            }
        }

        private void OnTargetReached()
        {
            if (TargetField.Consume(this))
            {
                // we just consumed water/food. Take it base to our camp
                TargetField = _map.StartField;
            }
            else
            {
                // we didn't collected anything, let's call ChooseTarget later
                TargetField = null;
            }
        }

        protected void UpdateKnowledge(MapField newField)
        {
            if (Knowledge.KnownFields.ContainsKey(newField))
            {
                Knowledge.Positive.Remove(newField);
                Knowledge.Negative.Remove(newField);
                Knowledge.Blocked.Remove(newField);
            }
            Knowledge.KnownFields[newField] = DateTime.Now;
            Knowledge.UnknownFields.Remove(newField);

            if (newField.Type == FieldType.Food || newField.Type == FieldType.Water)
                Knowledge.Positive[newField] = DateTime.Now;
            else if (newField.Type == FieldType.Danger)
            {
                Knowledge.Negative[newField] = DateTime.Now;
                ++Statistics.DangerCount;
            }
            else if (newField.Type == FieldType.Blocked)
                Knowledge.Blocked[newField] = DateTime.Now;
        }

        private MapField ChooseUnknownField()
        {
            if (Knowledge.UnknownFields.Count == 0)
                return null;
            var unknown = Knowledge.UnknownFields.ToArray();
            int index = SimulationOptions.Random.Next(0, Math.Min(8, unknown.Length));
            return QuickSelect(unknown, index + 1);
            //return unknown[SimulationOptions.Random.Next(0, unknown.Count)];
        }
        MapField QuickSelect(MapField[] arrayElements, int kthSmallest)
        {
            int startPos = 0;
            int endPos = arrayElements.Length - 1;

            while (endPos > startPos)
            {
                int pivotIndx = QuickSelectPartition(arrayElements, startPos, endPos);

                if (pivotIndx == kthSmallest)
                {
                    break;
                }

                if (pivotIndx > kthSmallest)
                {
                    endPos = pivotIndx - 1;
                }
                else
                {
                    startPos = pivotIndx + 1;
                }
            }

            return arrayElements[kthSmallest - 1];
        }

        int QuickSelectPartition(MapField[] listToSort, int leftIndxPtr, int rightIndxPtr)
        {
            int pivotIndx = (leftIndxPtr + rightIndxPtr) / 2;
            MapField pivotValue = listToSort[pivotIndx];
            float pivotDistance = Map.Distance(CurrentField, pivotValue);

            Swap(listToSort, pivotIndx, rightIndxPtr);
            for (int lpStIndx = leftIndxPtr; lpStIndx <= rightIndxPtr; lpStIndx++)
            {
                if (Map.Distance(CurrentField, listToSort[lpStIndx]) < pivotDistance)
                {
                    Swap(listToSort, lpStIndx, leftIndxPtr);
                    leftIndxPtr++;
                }
            }

            Swap(listToSort, leftIndxPtr, rightIndxPtr);
            return leftIndxPtr;
        }

        private void Swap(MapField[] arr, int i, int j)
        {
            MapField temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        public bool Equals(IAgent other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals((IAgent)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void ShareAllKnowledgeTo(IAgent shareTo)
        {
            TransferKnowledge(Knowledge.KnownFields.ToList(), shareTo);
        }

        public void ShareRandomKnowledgeTo(IAgent shareTo)
        {
            int knowledgeCountToShare = SimulationOptions.Random.Next(SimulationOptions.ShareRandomKnowledgeMin, SimulationOptions.ShareRandomKnowledgeMax + 1);
            var knowledgeToTransfer = Knowledge.KnownFields.ToList().Shuffle().Take(knowledgeCountToShare).ToList();
            TransferKnowledge(knowledgeToTransfer, shareTo);
        }

        private void TransferKnowledge(IList<KeyValuePair<MapField, DateTime>> knowledgeToTransfer, IAgent shareTo)
        {
            foreach (var pair in knowledgeToTransfer)
            {
                // don't know that or he's knowledge is outdated
                if (!shareTo.Knowledge.KnownFields.ContainsKey(pair.Key) || shareTo.Knowledge.KnownFields[pair.Key] < pair.Value)
                {
                    if (shareTo.Knowledge.KnownFields.ContainsKey(pair.Key))
                    {
                        shareTo.Knowledge.Positive.Remove(pair.Key);
                        shareTo.Knowledge.Negative.Remove(pair.Key);
                        shareTo.Knowledge.Blocked.Remove(pair.Key);
                    }

                    shareTo.Knowledge.KnownFields[pair.Key] = pair.Value;
                    shareTo.Knowledge.UnknownFields.Remove(pair.Key);

                    if (pair.Key.Type == FieldType.Food || pair.Key.Type == FieldType.Water)
                        shareTo.Knowledge.Positive[pair.Key] = pair.Value;
                    else if (pair.Key.Type == FieldType.Danger)
                        shareTo.Knowledge.Negative[pair.Key] = pair.Value;
                    else if (pair.Key.Type == FieldType.Blocked)
                        shareTo.Knowledge.Blocked[pair.Key] = pair.Value;
                }
            }
        }
    }
}