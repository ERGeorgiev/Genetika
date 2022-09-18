using Genetika.Genetic.Mutation;
using Genetika.Neural;
using Newtonsoft.Json;
using System.IO;

namespace Genetika.Genetic
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GeneSlim
    {
        [JsonProperty]
        protected float fitness = 0;

        [JsonProperty]
        public NeuralNetwork network;

        [JsonConstructor]
        public GeneSlim()
        {
        }

        public GeneSlim(int numberOfInputs, int numberOfOutputs)
        {
            this.network = new NeuralNetwork(numberOfInputs, numberOfOutputs);
        }

        /// <summary>
        /// Creates a deep copy of the SimpleGene.
        /// </summary>
        /// <param name="source">Source gene to copy.</param>
        public GeneSlim(GeneSlim source)
        {
            this.network = new NeuralNetwork(source.network);
        }

        public float Fitness
        {
            get => fitness;
            set => fitness = value;
        }

        public float[] GenerateOutput(float[] inputs)
        {
            return network.FeedForward(inputs);
        }

        public virtual void Save(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (StreamWriter file = new StreamWriter(filePath))
            {
                file.Write(ToJson());
            }
        }

        public static GeneSlim Load(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                GeneSlim gene = FromJson(json);
                return gene;
            }
        }

        public virtual string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

        public static GeneSlim FromJson(string json) => JsonConvert.DeserializeObject<GeneSlim>(json);
    }
}