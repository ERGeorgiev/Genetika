using Genetika.Genetic.Fitness;
using Genetika.Interfaces;
using Genetika.Neural;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using EdsLibrary.Logging.Table;

namespace Genetika.Genetic
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Gene<T> : GeneSlim, ITablePrint<Gene<T>>
        where T : class, IEntity<T>
    {
        [JsonProperty]
        private float totalFitness = 0;

        public T entity;

        [JsonConstructor]
        internal Gene()
        {
        }

        public Gene(GeneSlim gene, T entity)
        {
            this.fitness = gene.Fitness;
            this.network = gene.network;
            this.entity = entity;
        }

        public Gene(int numberOfInputs, int numberOfOutputs, T entity)
        {
            this.network = new NeuralNetwork(numberOfInputs, numberOfOutputs);
            this.entity = entity;
        }

        /// <summary>
        /// Creates a deep copy of the Gene. Does not attach to the same entity.
        /// </summary>
        /// <param name="source">Source gene to copy.</param>
        /// <param name="entity">Entity to attach to.</param>
        public Gene(Gene<T> source, T entity)
        {
            this.network = new NeuralNetwork(source.network);
            this.entity = entity;
        }

        [JsonProperty]
        public float Updates { get; private set; } = 0;

        [JsonProperty]
        public bool ProtectedFromExtinction { get; set; } = false;

        public void Update()
        {
            entity.Update(GenerateOutput());
            float newFitness = entity.GetFitness();
            if (entity.FitnessLogic == FitnessLogic.PreferZero)
            {
                newFitness = Math.Abs(newFitness);
            }
            Updates++;
            totalFitness += newFitness;
            fitness = entity.AccumulativeFitness ? (totalFitness / Updates) : newFitness;
        }

        public void Restart()
        {
            Updates = 0;
            totalFitness = default;
            fitness = default;
            entity.Restart();
        }

        public float[] GenerateOutput() => GenerateOutput(entity.GenerateInput());

        public TableFormatter GetTableFormatter()
        {
            TableFormatter formatter = new TableFormatter();
            string fitnessName;
            if (entity.AccumulativeFitness)
            {
                fitnessName = "Fit Avg";
            }
            else
            {
                fitnessName = "Fitness";
            }

            formatter.Add(fitnessName, Fitness, "+0.00;-0.00", 7);
            formatter.Add(entity.GetTableFormatter());

            return formatter;
        }

        public TableFormatter GetTableFormatter(Gene<T> compare)
        {
            TableFormatter formatter = new TableFormatter();
            string fitnessName;
            if (entity.AccumulativeFitness)
            {
                fitnessName = "Fit Avg";
            }
            else
            {
                fitnessName = "Fitness";
            }

            formatter.Add(fitnessName, new KeyValuePair<float, float>(Fitness, compare.Fitness), "+0.00;-0.00", 7);
            formatter.Add(entity.GetTableFormatter(compare.entity));

            return formatter;
        }

        public override void Save(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (StreamWriter file = new StreamWriter(filePath))
            {
                file.Write(ToJson());
            }
        }

        public static Gene<T> Load(string filePath, T entity)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                Gene<T> gene = FromJson(json, entity);
                return gene;
            }
        }

        public override string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

        public static Gene<T> FromJson(string json, T entity) => new Gene<T>(JsonConvert.DeserializeObject<Gene<T>>(json), entity);
    }
}