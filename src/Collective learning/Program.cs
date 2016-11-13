using System;
using SFML.Graphics;
using SFML.Window;

namespace Collective_learning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var window = new RenderWindow(new VideoMode(1280u, 768u), "SFML Example");

            window.Closed += (s, e) => window.Close();
            window.SetVisible(true);
            window.SetVerticalSyncEnabled(true);
            window.SetFramerateLimit(60);
            window.SetActive();


            CircleShape cs = new CircleShape(100.0f);
            cs.FillColor = Color.Green;

            while (window.IsOpen)
            {
                // Dispatch events to work with native event loop
                window.DispatchEvents();
                window.Clear();
                window.Draw(cs);
                window.Display();
            }
        }
    }
}
