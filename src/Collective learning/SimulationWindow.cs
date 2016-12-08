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

            _simulationView = new View(new FloatRect(0, 0, WindowWidth, WindowHeight));
            _simulationView.Move(new Vector2f(-WindowWidth/4f - (WindowWidth * 3/4f - simulation.Width) / 2, -(WindowHeight - simulation.Height) / 2));

            _panel = new Panel
            {
                Position = new Vector2f(0, 0),
                Size = new Vector2f(WindowWidth/4f, WindowHeight)
            };
            InitGui();
            CreateEventHandlers();
        }

        private void InitGui()
        {
            Box box = new Box();
            var slider = new Slider("Szybkość symulacji", 0.1f, 40f, 3);
            slider.OnChange += UpdateSimulationSpeed;
            box.AddController(slider);

            _panel.AddBox(box);

            box = new Box(vertical: false);
            var button = new Button("Pauza");
            button.OnClick += PausedClicked;
            box.AddController(button);
            
            _panel.AddBox(box);
        }

        private void UpdateStatistics()
        {
            _panel.SimulationStatistics = _simulation.SimulationStatistics;
        }

        private void PausedClicked()
        {
            _simulation.Paused = !_simulation.Paused;
        }

        private void UpdateSimulationSpeed(float value)
        {
            SimulationOptions.AgentSpeed = value;
        }

        private void CreateEventHandlers()
        {
            Closed += (s, e) => Close();
            KeyPressed += OnKeyPressed;
            Resized += OnResized;
            MouseButtonPressed += _panel.ProcessClick;
            MouseButtonPressed += (sender, args) =>
            {
                if (!_panel.GetGlobalBounds().Contains(args.X, args.Y))
                    _simulation.ProcessClick(new Vector2f(args.X + _simulationView.Center.X - WindowWidth/2f, args.Y + _simulationView.Center.Y - WindowHeight/2f));
            };

            MouseWheelScrolled += Scroll;
        }

        private void Scroll(object sender, MouseWheelScrollEventArgs args)
        {
            _simulationView.Zoom(args.Delta < 0 ? 1.1f : 0.9f);
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
                    _simulationView.Move(new Vector2f(offset.X, offset.Y)*(_simulationView.Size.Y/WindowHeight));

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
            UpdateStatistics();
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