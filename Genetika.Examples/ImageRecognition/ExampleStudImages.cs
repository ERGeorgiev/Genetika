//using EdsLibrary.Extensions;
//using EvolveNN.Genetic;
//using System;
//using Console = Colorful.Console;
//using System.Collections.Generic;
//using System.Linq;
//using System.Drawing;
//using System.IO;

//namespace EvolveNNExamples.Velocity
//{
//    public class ExampleStudImages
//    {
//        private const int runs = 10;

//        /// <summary>
//        /// Produces and array of best fitnesses over several generations.
//        /// </summary>
//        public static void Run()
//        {
//            RandomExt.Seed("Stud Example");
//            string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            
//            string pathStuds = currentDirectory + @"\StubImages\Images\Studs\";
//            string pathNotStuds = currentDirectory + @"\StubImages\Images\NotStuds\";

//            StudImageContainer studs = new StudImageContainer();
//            studs.Add(pathStuds, true);
//            studs.Add(pathNotStuds, false);
//            int population = 100;

//            ImageEntity[] entities = new ImageEntity[population];
//            for (int i = 0; i < population; i++)
//            {
//                entities[i] = new ImageEntity(studs);
//            }

//            Genome.MinLayers = 5;
//            Genome.MaxLayers = Genome.MinLayers;
//            Genome genome = new Genome(entities);
//            genome.Activate();

//            float[] bestFitness = new float[runs];
//            string fit;
//            string[] output;
//            Gene elite = null;
//            Gene tempElite = null;
//            for (int i = 1; i <= runs; i++)
//            {
//                var curL = Console.CursorLeft;
//                var curT = Console.CursorTop;
//                Console.WriteLine($"GENERATION: { i }", Color.White);
//                for (int u = 1; u <= 10; u++)
//                {
//                    genome.Update(1);
//                    Console.Write($"- [{ u }]");
//                    tempElite = genome.GetElites(1)[0];
//                    fit = Math.Round(tempElite.Fitness, 2).ToString("0.00");
//                    output = new string[tempElite.HiddenLayers.Length];
//                    for (int l = 0; l < tempElite.HiddenLayers.Length; l = l + 5)
//                    {
//                        output[l] = Math.Round(tempElite.HiddenLayers[l].Output(1)[0], 2).ToString("0.00");
//                    }
//                    output[output.Length - 1] = Math.Round(tempElite.HiddenLayers[tempElite.HiddenLayers.Length - 1].Output(1)[0], 2).ToString("0.00");
//                    Console.Write($" Fit: { fit }");
//                    Console.WriteLine($"; Out: [{ string.Join(", ", output.Where(o => string.IsNullOrEmpty(o) == false)) }]");
//                }
//                fit = Math.Round(bestFitness[i - 1], 2).ToString("0.00");
//                string min = Math.Round(tempElite.GetMinimumWeightValue(), 2).ToString("0.00");
//                string max = Math.Round(tempElite.GetMaximumWeightValue(), 2).ToString("0.00");
//                string avg = Math.Round(tempElite.GetAverageWeightValue(), 2).ToString("0.00");
//                var layers = tempElite.HiddenLayers;
//                var neurons = layers.Select(l => l.neurons.Length);
//                string neu = string.Join(", ", neurons.ToArray());
//                Console.Write($"  [W] = ");
//                Console.Write($"{ avg } [{ min } ; { max }]");
//                Console.Write($"; [N] = ");
//                Console.WriteLine($"[{ neu }];");
//                if (elite?.Fitness == tempElite.Fitness && i < runs)
//                {
//                    ConsoleExt.ClearRows(curT, Console.CursorTop);
//                    Console.SetCursorPosition(curL, curT);
//                }
//                else elite = tempElite;
//                genome.Reproduce(entities);
//            }
//        }
//    }
//}
