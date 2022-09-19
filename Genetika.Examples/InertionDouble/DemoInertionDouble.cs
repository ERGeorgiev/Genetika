using EdsLibrary.Extensions;
using System;
using Console = Colorful.Console;
using System.Drawing;
using Genetika.Examples.Demonstration;

namespace Genetika.Examples.InertionDouble
{
    public class DemoInertionDouble : Demo<DynamicEntity>
    {
        public DemoInertionDouble(int steps, GenetikaParameters parameters) 
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
                float goal = GenetikaParameters.random.Next(-10000.0f, 10000.0f);
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
                entity.entityA.positionGoal = goal;
                entity.entityB.positionGoal = goal;
            }
        }
    }
}
