﻿using System;
using System.Diagnostics;
using Collective_learning.Simulation;
using Collective_learning.Simulation.Factory;
using Collective_learning.Simulation.Interfaces;
using SFML.Graphics;
using SFML.System;

namespace Collective_learning
{
    public class Program
    {
        public static bool DrawWindow = false;
        public static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                SimulationOptions.KnowledgeSharingType = (SharingType) Enum.Parse(typeof(SharingType), args[0], true);
                SimulationOptions.OutputFile = args[1];
            }
            else
            {
                SimulationOptions.OutputFile = "data.txt";
            }

            ISimulation simulation = SimulationFactory.CreateDefault();
            var window = SimulationWindow.Create(simulation);

            Clock clock = new Clock();
            float totalTime = 0;
            int frames = 0;
            while (window.IsOpen)
            {
                if (totalTime > 1f)
                {
                    window.FPS.DisplayedString = frames.ToString();
                    totalTime -= 1f;
                    frames = 0;
                }

                float time = clock.Restart().AsSeconds();
                totalTime += time;

                window.DispatchEvents();
                window.Update(time);

                if (DrawWindow)
                {
                    window.Clear(Color.White);
                    window.Draw();
                    window.Display();
                }

                ++frames;
            }
        }
    }
}
