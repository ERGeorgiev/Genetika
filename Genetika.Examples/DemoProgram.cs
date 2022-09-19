using Genetika.Examples.Velocity;
using System;
using Genetika.Examples.Inertion;
using Genetika.Examples.Translation;
using EdsLibrary.Logging;
using Genetika.Examples.VelocityWithNoise;
using Genetika.Examples.InertionDouble;

namespace Genetika.Examples
{
    class DemoProgram
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Too dynamic.")]
        static void Main()
        {
            const int updates = 130;
            ConsoleMenu inputMenu = new ConsoleMenu();
            GenetikaParameters parameters = new GenetikaParameters();
            inputMenu.AddItem(ConsoleKey.V, "Velocity", () => new DemoVelocity(updates, parameters).Run());
            inputMenu.AddItem(ConsoleKey.I, "Inertion", () => new DemoInertion(updates, parameters).Run());
            inputMenu.AddItem(ConsoleKey.T, "Translation", () => new DemoTranslation(updates, parameters).Run());
            inputMenu.AddItem(ConsoleKey.N, "VelocityWithNoise", () => new DemoVelocityWithNoise(updates, parameters).Run());
            inputMenu.AddItem(ConsoleKey.D, "InertionDouble", () => new DemoInertionDouble(updates, parameters).Run());
            inputMenu.Display();

            Console.WriteLine("Demo finished. Press any key to exit...");
            Console.ReadKey();
        }
    }
}