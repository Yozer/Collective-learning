using System;
using Collective_learning.Simulation.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Collective_learning
{
    public sealed class SimulationWindow : RenderWindow
    {
        private readonly ISimulation _simulation;
        private readonly View _view;

        private const string WindowTitle = "Collective Learning 0.1";
        private const uint WindowWidth = 1280u;
        private const uint WindowHeight = 768u;

        private Vector2i _lastDragPoint;

        internal static SimulationWindow Create(ISimulation simulation)
        {
            var contextSettings = new ContextSettings
            {
                AntialiasingLevel = 16
            };

            return new SimulationWindow(simulation, contextSettings);
        }

        private SimulationWindow(ISimulation simulation, ContextSettings contextSettings) 
            : base(new VideoMode(WindowWidth, WindowHeight), WindowTitle, Styles.Default, contextSettings)
        {
            _simulation = simulation;

            SetVisible(true);
            SetVerticalSyncEnabled(true);
            SetFramerateLimit(60);
            SetActive();
            CreateEventHandlers();

            _view = new View(new FloatRect(0, 0, WindowWidth, WindowHeight));
        }

        private void CreateEventHandlers()
        {
            Closed += (s, e) => Close();
            KeyPressed += OnKeyPressed;
            Resized += OnResized;
        }

        private void OnResized(object sender, SizeEventArgs sizeEventArgs)
        {
            _view.Size = new Vector2f(sizeEventArgs.Width, sizeEventArgs.Height);
        }

        private void OnKeyPressed(object sender, KeyEventArgs args)
        {
            if(args.Code == Keyboard.Key.Escape)
                Close();
        }

        private void HandleDragging()
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                if (_lastDragPoint != default(Vector2i))
                {
                    var offset = _lastDragPoint - Mouse.GetPosition();
                    _view.Move(new Vector2f(offset.X, offset.Y));
                }
                _lastDragPoint = Mouse.GetPosition();
            }
            else
            {
                _lastDragPoint = default(Vector2i);
            }
        }
        internal void Update()
        {
            HandleDragging();
            _simulation.Update();
        }
        internal void Draw()
        {
            SetView(_view);
            Draw(_simulation);
        }
    }
}