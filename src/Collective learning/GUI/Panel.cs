using System;
using System.Collections.Generic;
using System.Linq;
using Collective_learning.GUI.BasicControllers;
using Collective_learning.Simulation;
using SFML.Graphics;
using SFML.System;
using SFML.Window;


namespace Collective_learning.GUI
{
    public class Panel : RectangleShape
    {
        private readonly List<Box> _boxList;
        private FloatRect _bounds;
        private readonly Text _statisticsText;
        private SimulationStatistics _simulationStatistics = new SimulationStatistics();

        public sealed override Vector2f Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
                if(_statisticsText != null)
                    _statisticsText.Position = new Vector2f(10, Size.Y - 10);}
        }

        public SimulationStatistics SimulationStatistics
        {
            get { return _simulationStatistics; }
            set { _simulationStatistics = value; UpdateStatisticsString(); }
        }

        public Panel()
        {
            _bounds = new FloatRect(10, 10, 0, 0);
            _boxList = new List<Box>();
            FillColor = new Color(70, 70, 70);

            _statisticsText = new Text(string.Empty, SliderSettings.DefaultFont, SliderSettings.TextSize - 6)
            {
                Color = SliderSettings.TextColor,
                Style = SliderSettings.TextStyle
            };
            UpdateStatisticsString();
            _statisticsText.Origin = new Vector2f(0, (_statisticsText.DisplayedString.ToCharArray().Count(t => t == '\n') + 1)*_statisticsText.CharacterSize);
        }
        private void UpdateStatisticsString()
        {
            _statisticsText.DisplayedString = $"Pożywienie: {SimulationStatistics.FoodCount}\nWoda: {SimulationStatistics.WaterCount}\n" +
                                              $"Zagrożenia: {SimulationStatistics.DangerCount}\nCzas symulacji: {SimulationStatistics.SimulationTime:mm\\:ss\\:fff}\n" +
                                              $"Liczba odkrytych pól: {SimulationStatistics.DiscoveredCount}/{SimulationStatistics.AllFieldsCount}\n" +
                                              $"Liczebność populacji: {SimulationStatistics.PopulationCount}\n";
        }

        public void AddBox(Box box)
        {
            box.SetPosition(new Vector2f(_bounds.Left, _bounds.Height + _bounds.Top + 15));
            _boxList.Add(box);
            _bounds.Height += box.GlobalBound.Height + 15;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
            _boxList.ForEach(target.Draw);
            _statisticsText.Draw(target, states);
        }

        public void Dragging(Vector2i point)
        {
            _boxList.ForEach(t => t.Dragging(point));
        }

        public void ProcessClick(object sender, MouseButtonEventArgs e)
        {
            _boxList.ForEach(t => t.ProcessClick(e));
        }
    }
}