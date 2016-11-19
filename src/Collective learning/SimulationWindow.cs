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
        private readonly View _simulationView;

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

            _simulationView = new View(new FloatRect(0, 0, WindowWidth, WindowHeight));
            _simulationView.Move(new Vector2f(-(WindowWidth - simulation.Width) / 2, - (WindowHeight - simulation.Height) / 2));
        }

        private void CreateEventHandlers()
        {
            Closed += (s, e) => Close();
            KeyPressed += OnKeyPressed;
            Resized += OnResized;
        }

        private void OnResized(object sender, SizeEventArgs sizeEventArgs)
        {
            _simulationView.Size = new Vector2f(sizeEventArgs.Width, sizeEventArgs.Height);
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
                    _simulationView.Move(new Vector2f(offset.X, offset.Y));
                }
                _lastDragPoint = Mouse.GetPosition();
            }
            else
            {
                _lastDragPoint = default(Vector2i);
            }
        }
        internal void Update(float delta)
        {
            HandleDragging();
            _simulation.Update(delta);
        }
        internal void Draw()
        {
            SetView(_simulationView);
            Draw(_simulation);
        }
    }
}