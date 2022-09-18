using EdsLibrary.Extensions;
using Genetika.Examples.Demonstration;
using System;
using System.Drawing;
using Console = Colorful.Console;

namespace Genetika.Examples.Inertion
{
    public class DemoInertion : Demo<DynamicEntity>
    {
        public DemoInertion(int steps, AnnParameters parameters) 
            : base(DynamicEntity.NumberOfInputs, DynamicEntity.NumberOfOutputs, steps, parameters)
        {
            this.consoleMenu.AddItem(ConsoleKey.G, "goal", MenuSetGoal);
        }

        private void MenuSetGoal()
        {
            Console.Write("Enter Goal value: ");
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                float goal = AnnParameters.random.Next(-10000.0f, 10000.0f);
                SetGoal(goal);
                Console.WriteLine("Goal set.", Color.LightGreen);
            }
            else if (int.TryParse(input, out int goal))
            {
                SetGoal(goal);
                Console.WriteLine("Goal set.", Color.LightGreen);
            }
            else
            {
                Console.WriteLine("Unable to parse integer.", Color.Red);
            }
        }

        private void SetGoal(float goal)
        {
            foreach (var entity in dynamicEntities)
            {
                entity.positionGoal = goal;
            }
        }
    }
}
