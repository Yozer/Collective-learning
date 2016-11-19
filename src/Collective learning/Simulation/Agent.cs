using System;
using System.Collections.Generic;
using Collective_learning.Simulation.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Collective_learning.Simulation
{
    internal class Agent : IAgent
    {
        private Vector2f _movementDirection = new Vector2f(0, 0);
        private MapField _nextField;

        private readonly CircleShape _circleShape = new CircleShape(SimulationOptions.AgentRadius);

        public MapField CurrentField { get; private set; }

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

        public Queue<MapField> Path { get; set; } = new Queue<MapField>();
        public Vector2f Position => _circleShape.Position;

        public Agent(MapField startField)
        {
            CurrentField = startField;
            _circleShape.FillColor = SimulationOptions.AgentColor;
            _circleShape.Origin = new Vector2f(_circleShape.Radius, _circleShape.Radius);
            _circleShape.Position = startField.Center;
            _circleShape.OutlineThickness = 1.0f;
            _circleShape.OutlineColor = Color.Black;
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_circleShape);
        }


        public void Update(float delta)
        {
            if(NextField == null && Path.Count > 0)
                NextField = Path.Dequeue();

            if (NextField != null)
            {
                _circleShape.Position += _movementDirection * SimulationOptions.AgentSpeed * delta;
                var diff = _circleShape.Position - NextField.Center;

                if (diff.X * _movementDirection.X + diff.Y * _movementDirection.Y > 0)
                {
                    _circleShape.Position = NextField.Center;
                    CurrentField = NextField;
                    NextField = null;
                }
            }
        }
    }
}