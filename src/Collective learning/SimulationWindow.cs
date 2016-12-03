using System;
using Collective_learning.Simulation.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Collective_learning.GUI;
using Collective_learning.GUI.BasicControllers;
using Collective_learning.Simulation;

namespace Collective_learning
{
    public sealed class SimulationWindow : RenderWindow
    {
        private readonly ISimulation _simulation;
        private readonly View _simulationView;
        private readonly Panel _panel;

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
            _simulationView.Move(new Vector2f(-WindowWidth/4f - (WindowWidth * 3/4f - simulation.Width) / 2, -(WindowHeight - simulation.Height) / 2));

            _panel = new Panel(this)
            {
                Position = new Vector2f(0, 0),
                Size = new Vector2f(WindowWidth/4f, WindowHeight)
            };
            InitGui();
        }

        private void InitGui()
        {
            Box box = new Box();
            var slider = new Slider("Szybkość symulacji", 0.1f, 40f, 3);
            slider.OnChange += UpdateSimulationSpeed;
            box.AddController(slider);

            _panel.AddBox(box);
        }

        private void UpdateSimulationSpeed(float value)
        {
            Simulation.SimulationOptions.AgentSpeed = SimulationOptions.FieldHeight*value*1.5f;
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
            _simulationView.Zoom(args.Delta > 0 ? 1.1f : 0.9f);
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


        private void HandleDragging(Vector2i mousePosition)
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left) && ((_lastDragPoint == default(Vector2i) && !_panel.GetGlobalBounds().Contains(mousePosition.X, mousePosition.Y)) || _lastDragPoint != default(Vector2i)))
            {
                if (_lastDragPoint != default(Vector2i))
                {
                    var offset = _lastDragPoint - mousePosition;
                    _simulationView.Move(new Vector2f(offset.X, offset.Y));

                }
                _lastDragPoint = Mouse.GetPosition(this);
            }
            else
            {
                _lastDragPoint = default(Vector2i);
            }
        }
        internal void Update(float delta)
        {
            var mousePoint = Mouse.GetPosition(this);
            if (HasFocus())
            {
                if(_panel.GetGlobalBounds().Contains(mousePoint.X, mousePoint.Y))
                    _panel.Dragging(mousePoint);
                else
                    HandleDragging(mousePoint);
            }

            _simulation.Update(delta);
        }
        internal void Draw()
        {
            SetView(_simulationView);
            Draw(_simulation);
            SetView(DefaultView);
            Draw(_panel);
        }
    }
}