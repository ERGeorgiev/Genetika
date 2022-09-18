using Genetika.Genetic.Fitness;

namespace Genetika.Interfaces
{
    public interface IEntity<T> : ITablePrint<T>
    {
        int Id { get; }
        bool AccumulativeFitness { get; }
        FitnessLogic FitnessLogic { get; }

        float GetFitness();
        void Update(float[] outputs);
        float[] GenerateInput();
        void Restart();
    }
}