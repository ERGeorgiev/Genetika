using Genetika.Genetic;
using System;
using Console = Colorful.Console;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using EdsLibrary.Logging;
using Genetika.Interfaces;

namespace Genetika.Examples.Demonstration
{
    public abstract class Demo<T> : GenetikaManager<T>
        where T : class, IEntity<T>
    {
        private readonly string saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Genetika\Saves\";
        private readonly string genomeSaveFolder = @"Genome\";
        private readonly string geneSaveFolder = @"Gene\";
        protected readonly ConsoleMenu consoleMenu = new ConsoleMenu();

        public Demo(
            int numberOfInputs,
            int numberOfOutputs,
            int updates,
            GenetikaParameters parameters,
            Func<T> createEntity = null)
            : base(numberOfInputs, numberOfOutputs, updates, parameters, createEntity)
        {
            consoleMenu.AddItem(ConsoleKey.Enter, "Run [CTRL:Multiplier; SHIFT:Timer]", MenuNextGeneration);
            consoleMenu.AddItem(ConsoleKey.P, "Print", PrintAllGenes);
            consoleMenu.AddItem(ConsoleKey.F, "Focus", MenuFocusGene);
            consoleMenu.AddItem(ConsoleKey.I, "Isolate", MenuIsolateGene);
            consoleMenu.AddItem(ConsoleKey.N, "Network", MenuPrintNeurons);
            consoleMenu.AddItem(ConsoleKey.S, "Save", MenuSave);
            consoleMenu.AddItem(ConsoleKey.L, "Load", MenuLoad);
            consoleMenu.AddItem(ConsoleKey.R, "Remove", MenuRemoveGene);
        }

        public float Run()
        {
            Simulate(Genome);
            consoleMenu.DisplayContinously();

            return Genome.BestFitness;
        }

        protected void MenuIsolateGene()
        {
            Console.Write("Enter Gene Id: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int id))
            {
                Gene<T> selectedGene = Genome.genes.Where(x => x.entity.Id == id).First();
                selectedGene.Restart();
                Simulate(selectedGene);
            }
            else
            {
                Console.WriteLine("Unable to parse integer.", Color.Red);
            }
        }

        protected void MenuFocusGene()
        {
            Console.Write("Enter Gene Id: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int id))
            {
                Gene<T> selectedGene = Genome.genes.Where(x => x.entity.Id == id).First();
                if (selectedGene == FocusGene)
                {
                    FocusGene = null;
                    Console.WriteLine("Gene no longer focused.", Color.LightGreen);
                }
                else
                {
                    FocusGene = selectedGene;
                    Console.WriteLine("Gene focused.", Color.LightGreen);
                }
            }
            else
            {
                Console.WriteLine("Unable to parse integer.", Color.Red);
            }
        }

        protected void MenuProtect()
        {
            Console.Write("Enter Gene Id: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int id))
            {
                Gene<T> selectedGene = Genome.genes.Where(x => x.entity.Id == id).First();
                Console.WriteLine($"Gene protection status set to: {selectedGene.ProtectedFromExtinction}", Color.LightGreen);
            }
            else
            {
                Console.WriteLine("Unable to parse integer.", Color.Red);
            }
        }

        protected void MenuRemoveGene()
        {
            Console.Write("Enter Gene Id: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int id))
            {
                Gene<T> selectedGene = Genome.genes.Where(x => x.entity.Id == id).First();
                Genome.genes.Remove(selectedGene);
                Console.WriteLine("Gene removed.", Color.LightGreen);
            }
            else
            {
                Console.WriteLine("Unable to parse integer.", Color.Red);
            }
        }

        protected void MenuPrintNeurons()
        {
            Console.Write("Enter Gene Id: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int id))
            {
                Gene<T> selectedGene = Genome.genes.Where(x => x.entity.Id == id).First();
                PrintNeurons(selectedGene);
            }
            else
            {
                Console.WriteLine("Unable to parse integer.", Color.Red);
            }
        }

        protected void MenuSave()
        {
            Console.Write("Enter Gene Id (leave blank to target the whole genome): ");
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                string path = saveLocation + genomeSaveFolder;
                string fileName = $@"Genome{DateTimeOffset.Now.ToUnixTimeSeconds()}.json";
                path += fileName;

                Genome.Save(path);
                Console.WriteLine($"Genome saved to '{path}'.", Color.LightGreen);
            }
            else if (int.TryParse(input, out int id))
            {
                Gene<T> selectedGene = Genome.genes.Where(x => x.entity.Id == id).First();

                string path = saveLocation + geneSaveFolder;
                string fileName = $@"Gene{DateTimeOffset.Now.ToUnixTimeSeconds()}.json";
                path += fileName;

                selectedGene.Save(path);
                Console.WriteLine($"Gene saved to '{path}'.", Color.LightGreen);
            }
            else
            {
                Console.WriteLine("Unable to parse integer.", Color.Red);
            }
        }

        protected void MenuLoad()
        {
            string[] genomeFiles = Array.Empty<string>();
            if (Directory.Exists(saveLocation + genomeSaveFolder))
            {
                Console.WriteLine("Available Genome files: ");
                genomeFiles = Directory.GetFiles(saveLocation + genomeSaveFolder);
                for (int i = 0; i < genomeFiles.Length; i++)
                {
                    Console.Write("[");
                    Console.Write(i, Color.White);
                    Console.Write("] ");
                    Console.WriteLine(genomeFiles[i]);
                }
            }

            string[] geneFiles = Array.Empty<string>();
            if (Directory.Exists(saveLocation + geneSaveFolder))
            {
                Console.WriteLine("Available Gene files: ");
                geneFiles = Directory.GetFiles(saveLocation + geneSaveFolder);
                for (int i = 0; i < geneFiles.Length; i++)
                {
                    Console.Write("[");
                    Console.Write(i + genomeFiles.Length, Color.White);
                    Console.Write("] ");
                    Console.WriteLine(geneFiles[i]);
                }
            }

            if (genomeFiles.Length == 0 && geneFiles.Length == 0)
            {
                Console.WriteLine("No save files found.", Color.Orange);
                return;
            }

            Console.Write("Enter file Id: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int id))
            {
                if (id < 0)
                {
                    Console.WriteLine("Invalid index.", Color.Red);
                    return;
                }
                else if (id < genomeFiles.Length)
                {
                    string file = genomeFiles[id];
                    Genome = Genome<T>.Load(file);

                    Console.WriteLine($"Genome '{file}' loaded.", Color.LightGreen);
                }
                else if (id < (genomeFiles.Length + geneFiles.Length))
                {
                    string file = geneFiles[id - genomeFiles.Length];
                    Gene<T> gene = Gene<T>.Load(file, CreateEntity());
                    Genome.genes.Add(gene);

                    Console.WriteLine($"Gene '{file}' loaded. Id: {gene.entity.Id}", Color.LightGreen);
                }
                else
                {
                    Console.WriteLine("Invalid index.", Color.Red);
                    return;
                }
            }
            else
            {
                Console.WriteLine("Unable to parse integer.", Color.Red);
            }
        }

        protected void MenuNextGeneration()
        {
            if (consoleMenu.Input.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                Console.Write("Multiplier: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int times) == false)
                {
                    Console.WriteLine("Unable to parse integer.", Color.Red);
                    return;
                }

                AdvanceGeneration(times);
            }
            else if (consoleMenu.Input.Modifiers.HasFlag(ConsoleModifiers.Shift))
            {
                Console.Write("Minutes: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int minutes) == false)
                {
                    Console.WriteLine("Unable to parse integer.", Color.Red);
                    return;
                }

                AdvanceGeneration(minutes * 60);
            }
            else
            {
                AdvanceGeneration();
            }
        }
    }
}
