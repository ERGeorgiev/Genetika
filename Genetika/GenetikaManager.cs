using EdsLibrary.Logging;
using Genetika.Genetic;
using Genetika.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Genetika
{
    public abstract class GenetikaManager<T>
        where T : class, IEntity<T>
    {
        private Genome<T> genome;

        protected readonly int numberOfInputs;
        protected readonly int numberOfOutputs;
        protected readonly Func<T> createEntity;
        protected int updates;
        protected Gene<T> focusGene;
        protected List<T> dynamicEntities;

        public GenetikaManager(
            int numberOfInputs,
            int numberOfOutputs,
            int updates,
            GenetikaParameters parameters,
            Func<T> createEntity = null)
        {
            this.numberOfInputs = numberOfInputs;
            this.numberOfOutputs = numberOfOutputs;
            this.updates = updates;
            this.createEntity = createEntity ?? this.CreateEntityDefault;
            this.dynamicEntities = new List<T>(parameters.population);
            for (int i = 0; i < parameters.population; i++)
            {
                CreateEntity();
            }
            this.genome = new Genome<T>(this.numberOfInputs, this.numberOfOutputs, this.dynamicEntities, parameters);
        }

        public ILogger Logger { get; set; } = new ConsoleLogger();

        protected Genome<T> Genome
        {
            get => genome;
            set
            {
                genome = value;
                if (genome.genes.Any(x => x.entity == null))
                {
                    RefreshEntities();
                }
            }
        }

        protected Gene<T> FocusGene
        {
            get => focusGene;
            set
            {
                if (focusGene != null && focusGene != default)
                {
                    focusGene.ProtectedFromExtinction = false;
                }
                focusGene = value;
                if (focusGene != null && focusGene != default)
                {
                    focusGene.ProtectedFromExtinction = true;
                }
            }
        }

        protected void RefreshEntities()
        {
            dynamicEntities = new List<T>(genome.Size);
            for (int i = 0; i < genome.Size * 2; i++)
            {
                CreateEntity();
            }
            genome.Attach(dynamicEntities);
        }

        protected T CreateEntity()
        {
            var newEntity = createEntity.Invoke();
            dynamicEntities.Add(newEntity);

            return newEntity;
        }

        private T CreateEntityDefault()
        {
            return Activator.CreateInstance<T>();
        }

        protected void Simulate(Genome<T> genome)
        {
            PreSimulation();
            Logger.Log($"# GENERATION: { genome.Generation }", LogPriority.Medium, ConsoleColor.White);


            Logger.LogTableHeaders(genome.genes[0].GetTableFormatter(), LogPriority.Medium);
            for (int u = 0; u < updates; u++)
            {
                PreUpdate();
                genome.Update();

                Gene<T> elite = genome.GetElites(1).FirstOrDefault();
                if (elite == null)
                {
                    Logger.Log($"No genes found.", LogPriority.High);
                    return;
                }

                LogPriority logPriority = (u == 0 || u == updates - 1) ? LogPriority.Medium : LogPriority.Low;
                TableFormatter formatter = (FocusGene == default) ? elite.GetTableFormatter() : elite.GetTableFormatter(FocusGene);
                Logger.LogTableValues(formatter, logPriority);

                PostUpdate();
            }

            PostSimulation();
        }

        protected void PrintNeurons(Gene<T> gene)
        {
            if (FocusGene == default)
            {
                TableFormatter formatter = gene.network.GetTableFormatter();
                Logger?.LogTableHeaders(formatter, LogPriority.Low);
                Logger?.LogTableValues(formatter, LogPriority.Low);
            }
            else
            {
                TableFormatter formatter = gene.network.GetTableFormatter(FocusGene.network);
                Logger?.LogTableHeaders(formatter, LogPriority.Low);
                Logger?.LogTableValues(formatter, LogPriority.Low);
            }
        }

        protected void PrintAllGenes()
        {
            TableFormatter formatter = genome.genes[0].GetTableFormatter();
            Logger?.LogTableHeaders(formatter, LogPriority.Medium);
            foreach (var gene in genome.genes)
            {
                formatter = gene.GetTableFormatter();
                Logger?.LogTableValues(formatter, LogPriority.Medium);
            }
            Logger.Log($"Total Genes: {genome.genes.Count}", LogPriority.Medium);
        }

        protected void RunValidityScan()
        {
            if (genome.RunValidityScan() == false)
                Logger.LogError("Genome is invalid.");
        }

        protected void ValidateFocusGene()
        {
            if (FocusGene != default && genome.genes.Contains(FocusGene) == false)
            {
                Logger.Log("The focused gene has gone extinct.", LogPriority.Medium, ConsoleColor.DarkYellow);
                FocusGene = default;
            }
        }

        public void Simulate(Gene<T> gene)
        {
            Genome<T> isolatedGenome = new Genome<T>(numberOfInputs, numberOfOutputs, new List<Gene<T>> { gene });
            Simulate(isolatedGenome);
        }

        public void Simulate(GeneSlim geneSlim)
        {
            // Do not use CreateEntity() here, as the entity should not enter the entities collection, as it is temporary.
            var gene = new Gene<T>(geneSlim, createEntity.Invoke());
            Simulate(gene);
        }

        public void AdvanceGeneration(double seconds)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalSeconds < seconds)
            {
                AdvanceGeneration();
            }

            sw.Stop();
        }

        public void AdvanceGeneration(int repeat = 1)
        {
            for (int i = 0; i < repeat; i++)
            {
                if (genome.Updates > 0)
                {
                    genome.Reproduce();
                }

                RunValidityScan();
                ValidateFocusGene();
                genome.Restart();
                if (repeat > 3)
                {
                    if (i != 0 && i < (repeat - 1))
                    {
                        LogPriority oldPriority = Logger.LogPriority;
                        Logger.LogPriority = LogPriority.Medium;
                        Simulate(genome);
                        Logger.LogPriority = oldPriority;
                    }
                    else
                    {
                        Simulate(genome);
                    }
                }
                else
                {
                    Simulate(genome);
                }
            }
        }

        public virtual void PreSimulation() { }

        public virtual void PostSimulation() { }

        public virtual void PreUpdate() { }

        public virtual void PostUpdate() { }
    }
}
