using System;
using Collective_learning.Simulation.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Collective_learning.GUI.BasicControllers;
using Collective_learning.GUI;

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
        private float _lastX;
        private readonly Panel _panel;

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

            _simulationView = new View(new FloatRect(0, 0, 3 * WindowWidth / 4, WindowHeight));
            _simulationView.Move(new Vector2f(-(WindowWidth - simulation.Width) / 2, -(WindowHeight - simulation.Height) / 2));
            _simulationView.Viewport = new FloatRect(0.25f, 0, 0.75f, 1);
            _panel = new Panel(new FloatRect(0, 0, WindowWidth / 4f, WindowHeight), this);
            _panel.Viewport = new FloatRect(0, 0, 0.25f, 1);

        }

        private void CreateEventHandlers()
        {
            Closed += (s, e) => Close();
            KeyPressed += OnKeyPressed;
            Resized += OnResized;
            MouseButtonPressed += OnClick;

            MouseWheelScrolled += Scroll;
        }

        private void Scroll(object sender, MouseWheelScrollEventArgs args)
        {

            if (args.Delta > 0)
            {
                _simulationView.Zoom(1.1f);
            }
            else
            {
                _simulationView.Zoom(0.9f);
            }
        }

        private void OnClick(object sender, MouseButtonEventArgs args)
        {
            _panel.OnClick(sender, args);
        }

        private void OnResized(object sender, SizeEventArgs sizeEventArgs)
        {
            _simulationView.Size = new Vector2f(sizeEventArgs.Width, sizeEventArgs.Height);

        }

        private void OnKeyPressed(object sender, KeyEventArgs args)
        {
            if (args.Code == Keyboard.Key.Escape)
                Close();
        }


        private void HandleDragging()
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left) && ((_lastDragPoint == default(Vector2i) && HasFocus()) || _lastDragPoint != default(Vector2i)))
            {
                if (_lastDragPoint != default(Vector2i))
                {
                    var pos = Mouse.GetPosition(this);
                    var offset = _lastDragPoint - pos;
                    _simulationView.Move(new Vector2f(offset.X, offset.Y));

                }
                _lastDragPoint = Mouse.GetPosition(this);

                if (Math.Abs(_lastX - default(int)) > 0.001)
                {
                    var pos1 = Mouse.GetPosition(this);
                    var pos = MapPixelToCoords(pos1);
                    var offset = pos.X - _lastX;
                    int offset1 = MapCoordsToPixel(new Vector2f(offset, 0)).X;

                    if (_panel.Bounds.Contains(pos.X, pos.Y))
                    {
                        _panel.Dragging(pos.X, pos.Y, offset1);
                    }
                }
                _lastX = MapPixelToCoords(Mouse.GetPosition(this)).X;

            }
            else
            {
                _lastDragPoint = default(Vector2i);
                _lastX = default(int);
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
            SetView(_panel);
            Draw(_panel);
        }
    }
}