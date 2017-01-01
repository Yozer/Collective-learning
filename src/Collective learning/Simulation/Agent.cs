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
        public SimulationStatistics Statistics { get; } = new SimulationStatistics();
        public int Id { get; }
        public DateTime? CollidedAt { get; set; }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                _circleShape.FillColor = _selected ? SimulationOptions.SelectedAgentColor : SimulationOptions.AgentColor;
            }
        }

        public Agent(Map map, Knowledge globalKnowledge)
        {
            _map = map;
            CurrentField = map.StartField;
            _circleShape.FillColor = SimulationOptions.AgentColor;
            _circleShape.Origin = new Vector2f(_circleShape.Radius, _circleShape.Radius);
            _circleShape.Position = map.StartField.Center;
            _circleShape.OutlineThickness = 1.0f;
            _circleShape.OutlineColor = Color.Black;

            Knowledge = globalKnowledge ?? new Knowledge();
            Knowledge.KnownFields[map.StartField] = DateTime.Now;
            Id = _internalId++;
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            if (CollidedAt.HasValue && CollidedAt.Value.Add(SimulationOptions.SharingKnowledgePenalty) > DateTime.Now)
                _circleShape.FillColor = Color.Magenta;
            else
                Selected = _selected; // reset selection color

            target.Draw(_circleShape);
        }

        public void Update(float delta)
        {
            if (CollidedAt.HasValue)
            {
                if(CollidedAt.Value.Add(SimulationOptions.SharingKnowledgePenalty) > DateTime.Now)
                    return;
                else if (CollidedAt.Value.Add(SimulationOptions.NoSharingPriodAfterSharingKnowledge) < DateTime.Now)
                    CollidedAt = null;
            }

            if (TargetField == null)
            {
                ChooseTarget();
                InvalidatePathToTarget();
            }
            Move(delta);
        }

        private void ChooseTarget()
        {
            // if we don't know any positive fields or we want to explore
            MapField fieldToExplore;
            if ((Knowledge.Positive.Keys.All(t => t == CurrentField) || SimulationOptions.Random.NextDouble() <= SimulationOptions.ExplorationThreshold)
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
                TargetField = Knowledge.Positive.Keys.Where(t => t != CurrentField).MinBy(t => Math.Sqrt((t.X-CurrentField.X)*(t.X-CurrentField.X)+(t.Y-CurrentField.Y)*(t.Y-CurrentField.Y)));
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
                ChooseTarget();
                InvalidatePathToTarget();
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
                _circleShape.Position += _movementDirection*SimulationOptions.AgentSpeed*delta;
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
            else
            {
                Knowledge.KnownFields[newField] = DateTime.Now;
            }

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
            var unknown = _map.Fields.Cast<MapField>().Where(t => !Knowledge.KnownFields.ContainsKey(t) && t != CurrentField).ToList();
            if (unknown.Count == 0)
                return null;

            return unknown[SimulationOptions.Random.Next(0, unknown.Count)];
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
            TransferKnowledge(Knowledge.Positive, shareTo.Knowledge.Positive);
            TransferKnowledge(Knowledge.Blocked, shareTo.Knowledge.Blocked);
            TransferKnowledge(Knowledge.Negative, shareTo.Knowledge.Negative);
            TransferKnowledge(Knowledge.KnownFields, shareTo.Knowledge.KnownFields);
        }

        private void TransferKnowledge(IDictionary<MapField, DateTime> from, IDictionary<MapField, DateTime> to)
        {
            foreach (var entry in from)
            {
                if (!to.ContainsKey(entry.Key) || to[entry.Key] < entry.Value)
                {
                    to[entry.Key] = entry.Value;
                }
            }
        }

    }
}