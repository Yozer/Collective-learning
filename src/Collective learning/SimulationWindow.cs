using System;
using System.Diagnostics;
using System.IO;
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
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private StreamWriter _writer = new StreamWriter(new FileStream(SimulationOptions.OutputFile, FileMode.Create, FileAccess.Write));

        private const string WindowTitle = "Collective Learning 0.1";
        private const uint WindowWidth = 1920u;
        private const uint WindowHeight = 1080u;

        private Vector2i _lastDragPoint;
        private float _accumulator = 0f;

        public Text FPS { get; }

        internal static SimulationWindow Create(ISimulation simulation)
        {
            var contextSettings = new ContextSettings
            {
                AntialiasingLevel = 0
            };

            return new SimulationWindow(simulation, contextSettings);
        }

        private SimulationWindow(ISimulation simulation, ContextSettings contextSettings)
            : base(new VideoMode(WindowWidth, WindowHeight), WindowTitle, Styles.Default, contextSettings)
        {
            _simulation = simulation;

            SetVisible(true);
            SetVerticalSyncEnabled(false);
            //SetFramerateLimit(60);
            SetActive();

            _simulationView = new View(new FloatRect(0, 0, WindowWidth, WindowHeight));
            _simulationView.Move(new Vector2f(-WindowWidth/5f - (WindowWidth * 4/5f - simulation.Width) / 2, -(WindowHeight - simulation.Height) / 2));

            _panel = new Panel
            {
                Position = new Vector2f(0, 0),
                Size = new Vector2f(WindowWidth/5f, WindowHeight)
            };
            InitGui();
            CreateEventHandlers();

            FPS = new Text("0", ButtonSettings.DefaultFont, 15);
            FPS.Position = new Vector2f(5, 5);
            FPS.Color = Color.Yellow;
            _stopwatch.Start();

            _writer.WriteLine("step;food;water;danger");
            UpdateStatistics(0f);

        }

        private void InitGui()
        {
            Box box = new Box();
            var slider = new Slider("Szybkość symulacji", 0f, 1000f, SimulationOptions.SimulationSpeed, "0");
            slider.OnChange += value => SimulationOptions.SimulationSpeed = (int)Math.Round(value);
            box.AddController(slider);
            slider = new Slider("Próg eksploracji", 0.0f, 1f, SimulationOptions.ExplorationThreshold, "0.000");
            slider.OnChange += value => SimulationOptions.ExplorationThreshold = value;
            box.AddController(slider);

            slider = new Slider("Kara za dzielenie się wiedzą", 0, 5000, SimulationOptions.SharingKnowledgePenalty, "0");
            slider.OnChange += value => SimulationOptions.SharingKnowledgePenalty = (int)Math.Round(value);
            box.AddController(slider);

            slider = new Slider("Okres nie dzielenia się wiedzą", 0, 5000, SimulationOptions.NoSharingPeriodAfterSharingKnowledge, "0");
            slider.OnChange += value => SimulationOptions.NoSharingPeriodAfterSharingKnowledge = (int)Math.Round(value);
            box.AddController(slider);

            slider = new Slider("Szansa na przekazanie wiedzy", 0.0f, 1f, SimulationOptions.ChanceToShareKnowledge, "0.000");
            slider.OnChange += value => SimulationOptions.ChanceToShareKnowledge = value;
            box.AddController(slider);

            _panel.AddBox(box);

            box = new Box(vertical: false);
            var button = new Button("Pauza");
            button.OnClick += PausedClicked;
            box.AddController(button);
            
            _panel.AddBox(box);
        }

        private void UpdateStatistics(float delta)
        {
            _panel.SimulationStatistics = _simulation.SimulationStatistics;
            Console.Clear();
            Console.WriteLine($"Step:        {_simulation.SimulationStatistics.SimulationStep,6}");
            Console.WriteLine($"Food:        {_simulation.SimulationStatistics.FoodCount,4}/{_simulation.SimulationStatistics.AllFoodCount,4}");
            Console.WriteLine($"Water:       {_simulation.SimulationStatistics.WaterCount,4}/{_simulation.SimulationStatistics.AllWaterCount,4}");
            Console.WriteLine($"Danger:      {_simulation.SimulationStatistics.DangerCount,6}");
            Console.WriteLine($"All dangers: {_simulation.SimulationStatistics.AllThreats,3}");

            _writer?.WriteLine($"{_simulation.SimulationStatistics.SimulationStep};{_simulation.SimulationStatistics.FoodCount};{_simulation.SimulationStatistics.WaterCount};{_simulation.SimulationStatistics.DangerCount}");
        }

        private void PausedClicked()
        {
            _simulation.Paused = !_simulation.Paused;
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
            else if (args.Code == Keyboard.Key.P)
                Program.DrawWindow = !Program.DrawWindow;
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
            if (Program.DrawWindow)
            {
                var mousePoint = Mouse.GetPosition(this);
                if (HasFocus())
                {
                    if (_panel.GetGlobalBounds().Contains(mousePoint.X, mousePoint.Y))
                        _panel.Dragging(mousePoint);
                    else
                        HandleDragging(mousePoint);
                }
            }

            if (_simulation.Paused && _writer != null)
            {
                UpdateStatistics(delta);
                _writer.Dispose();
                _writer = null;
            }

            if (_accumulator >= SimulationOptions.SimulationSpeed)
            {
                _accumulator -= SimulationOptions.SimulationSpeed;
                _simulation.Update(delta);
            }

            if (_stopwatch.ElapsedMilliseconds > 50)
            {
                UpdateStatistics(delta);
                _stopwatch.Restart();
            }

            _accumulator += delta * 1000;
        }
        internal void Draw()
        {
            SetView(_simulationView);
            Draw(_simulation);
            SetView(DefaultView);
            Draw(_panel);
            Draw(FPS);
        }
    }
}