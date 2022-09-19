using Genetika.Genetic.Crossover;
using Genetika.Genetic.Fitness;
using Genetika.Genetic.Selection;
using Genetika.Neural;
using Genetika.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Genetika.Genetic.Mutation;

namespace Genetika.Genetic
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Genome<T>
        where T : class, IEntity<T>
    {
        [JsonProperty]
        public readonly int numberOfInputs;
        [JsonProperty]
        public readonly int numberOfOutputs;
        [JsonProperty]
        public readonly GenetikaParameters parameters = new GenetikaParameters();
        [JsonProperty]
        public List<Gene<T>> genes;

        [JsonConstructor]
        public Genome()
        {
        }

        public Genome(int numberOfInputs, int numberOfOutputs, List<Gene<T>> genes, GenetikaParameters parameters = null)
        {
            this.parameters = parameters ?? this.parameters;
            this.numberOfInputs = numberOfInputs;
            this.numberOfOutputs = numberOfOutputs;
            this.genes = genes;
        }

        public Genome(int numberOfInputs, int numberOfOutputs, List<T> entities, GenetikaParameters parameters = null)
        {
            this.parameters = parameters ?? this.parameters;
            this.numberOfInputs = numberOfInputs;
            this.numberOfOutputs = numberOfOutputs;
            this.genes = new List<Gene<T>>(entities.Count);
            for (int i = 0; i < entities.Count; i++)
            {
                genes.Add(new Gene<T>(this.numberOfInputs, this.numberOfOutputs, entities[i]));
            }
        }

        public int Elitism => (int)(genes.Count * parameters.elitismRatio);

        [JsonProperty]
        public float BestFitness { get; private set; }

        [JsonProperty]
        public int Updates { get; private set; }

        public int Size
        {
            get { return genes.Count; }
        }

        [JsonProperty]
        public int Generation { get; private set; } = 1;

        public void Update(int times = 1)
        {
            for (int t = 0; t < times; t++)
            {
                Updates++;
                Parallel.For(0, genes.Count, i =>
                {
                    if (genes[i].entity != default)
                    {
                        genes[i].Update();
                    }
                    else
                    {
                        genes.Remove(genes[i]);
                    }
                });
            }
        }

        public bool RunValidityScan()
        {
            var geneDupes = genes.GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList()
                .Count;
            if (geneDupes > 0)
                Console.WriteLine($"Duplicate genes found: {geneDupes}");

            var entityDupes = genes.GroupBy(x => x.entity)
                .Where(e => e.Count() > 1)
                .Select(e => e.Key)
                .ToList()
                .Count;
            if (entityDupes > 0)
                Console.WriteLine($"Duplicate Entities found: {entityDupes}");

            var neuralDupes = genes.GroupBy(x => x.network)
                .Where(n => n.Count() > 1)
                .Select(n => n.Key)
                .ToList()
                .Count;
            if (neuralDupes > 0)
                Console.WriteLine($"Duplicate Neural Networks found: {neuralDupes}");

            return geneDupes == 0 && entityDupes == 0 && neuralDupes == 0;
        }

        public void Restart()
        {
            Updates = 0;
            BestFitness = 0;
            for (int i = 0; i < genes.Count; i++)
            {
                genes[i].Restart();
            }
        }

        public (Gene<T> childA, Gene<T> childB) Crossover()
        {
            Gene<T> parentA = GeneticSelect<T>.Gene(this, parameters.selectionType);
            Gene<T> parentB = GeneticSelect<T>.Gene(this, parameters.selectionType);
            Gene<T> childA = new Gene<T>(parentA, null);
            Gene<T> childB = new Gene<T>(parentB, null);
            GeneticCrossover<T>.NeuronLayer(childA, childB, parameters.crossoverType);

            return (childA, childB);
        }

        private IEnumerable<Gene<T>> Crossover(int count)
        {
            var children = new List<Gene<T>>(count);

            for (int i = 0; i < count; i++)
            {
                var (childA, childB) = Crossover();
                children.Add(childA);
                children.Add(childB);
            }

            return children.Take(count);
        }

        private IEnumerable<Gene<T>> Crossover(List<T> freeEntities)
        {
            var children = Crossover(freeEntities.Count).ToList();

            for (int i = 0; i < children.Count; i++)
            {
                children[i].entity = freeEntities[i];
            }

            return children;
        }

        public void Reproduce()
        {
            List<Gene<T>> newGeneration = new List<Gene<T>>(genes.Count);
            FitnessScaling<T>.ScaleFitness(this, parameters.scalingType);

            // Elites
            List<Gene<T>> elites = GetElites(Elitism);
            newGeneration.AddRange(elites);

            // Setup
            List<Gene<T>> nonElites = genes.Skip(elites.Count).ToList();
            int replaceCount = (int)Math.Ceiling(genes.Count * parameters.geneReplaceRatio);
            int selectCount = genes.Count - replaceCount;
            List<Gene<T>> replaceEntities = nonElites.Take(replaceCount).ToList();
            List<Gene<T>> crossoverEntities = nonElites.Skip(replaceCount).ToList();

            // Crossover
            if (parameters.crossoverType != CrossoverType.None)
            {
                List<Gene<T>> crossoverChildren = Crossover(crossoverEntities.Select(x => x.entity).ToList()).ToList();
                for (int i = 0; i < crossoverChildren.Count; i++)
                {
                    Mutate.Try(crossoverChildren[i].network);
                }

                newGeneration.AddRange(crossoverChildren);
            }
            else
            {
                replaceEntities.AddRange(crossoverEntities);
            }

            // Replace
            for (int i = 0; i < replaceEntities.Count; i++)
            {
                replaceEntities[i] = new Gene<T>(numberOfInputs, numberOfOutputs, replaceEntities[i].entity);
            }
            newGeneration.AddRange(replaceEntities);

            // Protected
            List<Gene<T>> prot = genes.Where(g => g.ProtectedFromExtinction && (newGeneration.Contains(g) == false)).ToList();
            int newGenCount = newGeneration.Count;
            newGeneration.RemoveAll(g => prot.Select(p => p.entity).Contains(g.entity));
            newGeneration.AddRange(prot);

            genes = newGeneration;

            Generation++;
        }

        public int FitnessCompare(float a, float b)
        {
            if (float.IsNaN(a) || float.IsInfinity(a))
            {
                return 1;
            }
            else if (float.IsNaN(b) || float.IsInfinity(b))
            {
                return -1;
            }

            switch (genes[0].entity.FitnessLogic)
            {
                case FitnessLogic.PreferHigher:
                    return b.CompareTo(a);
                case FitnessLogic.PreferLower:
                    return a.CompareTo(b);
                case FitnessLogic.PreferZero:
                    return Math.Abs(a).CompareTo(Math.Abs(b));
                default:
                    throw new NotImplementedException();
            }
        }

        public void Attach(List<T> newEntities)
        {
            // Remove extra genes.
            genes.Sort((a, b) => {
                if (a.entity == null && b.entity == null) return 0;
                else if (a.entity == null) return 1;
                else if (b.entity == null) return -1;
                else return FitnessCompare(a.Fitness, b.Fitness);
            });
            if (genes.Count > newEntities.Count)
            {
                genes = genes.Take(newEntities.Count).ToList();
            }

            for (int i = 0; i < genes.Count; i++)
            {
                genes[i].entity = newEntities[i];
            }

            // Add more genes if needed.
            if (genes.Count < newEntities.Count)
            {
                int oldGenesCount = genes.Count;
                int newGenesCount = newEntities.Count - genes.Count;
                for (int i = 0; i < newGenesCount; i++)
                {
                    genes.Add(new Gene<T>(numberOfInputs, numberOfOutputs, newEntities[oldGenesCount + i]));
                }
            }
        }

        public List<Gene<T>> GetElites(int count)
        {
            genes.Sort((a, b) => FitnessCompare(a.Fitness, b.Fitness));
            if (FitnessCompare(genes.FirstOrDefault().Fitness, BestFitness) > 0)
            {
                BestFitness = genes[0].Fitness;
            }

            return genes.Take(count).ToList();
        }

        public void Save(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (StreamWriter file = new StreamWriter(filePath))
            {
                file.Write(ToJson());
            }
        }

        public static Genome<T> Load(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<Genome<T>>(json);
            }
        }

        /// <summary>
        /// Returns the <see cref="NeuralNetwork"/> from all genes.
        /// </summary>
        /// <returns>A collectiong of <see cref="NeuralNetwork"/> taken from the genes.</returns>
        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

        /// <summary>
        /// Creates <see cref="NeuralNetwork[]"/> to use with the <see cref="Genome{T}"/> constructor.
        /// </summary>
        /// <param name="json">Json to deserialize from.</param>
        /// <returns>A <see cref="NeuralNetwork[]"/>.</returns>
        public static Genome<T> FromJson(string json) => JsonConvert.DeserializeObject<Genome<T>>(json);
    }
}