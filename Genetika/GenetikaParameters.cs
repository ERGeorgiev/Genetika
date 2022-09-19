using Genetika.Genetic.Crossover;
using Genetika.Genetic.Fitness;
using Genetika.Genetic.Mutation;
using Genetika.Genetic.Selection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace Genetika
{
    [Serializable]
    public class GenetikaParameters
    {
        public static Random random = new Random();

        public int population = 1000;
        public decimal geneReplaceRatio = 0.4m;
        public decimal elitismRatio = 0.2m;
        [JsonConverter(typeof(StringEnumConverter))]
        public FitnessScalingType scalingType = FitnessScalingType.None;
        [JsonConverter(typeof(StringEnumConverter))]
        public SelectionType selectionType = SelectionType.Tournament;
        [JsonConverter(typeof(StringEnumConverter))]
        public CrossoverType crossoverType = CrossoverType.SinglePoint;

        [JsonProperty]
        public static float MutationRate { get => Mutate.MutationRate; set => Mutate.MutationRate = value; }

        public void Save(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (StreamWriter file = new StreamWriter(filePath))
            {
                file.Write(ToJson());
            }
        }

        public static GenetikaParameters Load(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                GenetikaParameters parameters = FromJson(json);
                return parameters;
            }
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

        public static GenetikaParameters FromJson(string json) => JsonConvert.DeserializeObject<GenetikaParameters>(json);
    }
}