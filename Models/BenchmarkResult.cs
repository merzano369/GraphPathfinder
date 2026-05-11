namespace GraphPathfinder.Models
{
    /// <summary>
    /// Represents a benchmark result for a single pathfinding algorithm.
    /// Used for comparing the performance of different algorithms on the same data.
    /// </summary>
    public class BenchmarkResult
    {
        /// <summary>
        /// Name of the algorithm for which the benchmark result was obtained.
        /// </summary>
        public string AlgorithmName { get; }

        /// <summary>
        /// Number of iterations performed by the algorithm to find the path.
        /// </summary>
        public int Iterations { get; }

        /// <summary>
        /// Returns <c>true</c> if the algorithm successfully found a path, otherwise — <c>false</c>.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Initializes a new benchmark result with the specified parameters.
        /// </summary>
        /// <param name="algorithmName">Name of the algorithm.</param>
        /// <param name="iterations">Number of algorithm iterations.</param>
        /// <param name="isSuccess">Whether the algorithm succeeded.</param>
        public BenchmarkResult(string algorithmName, int iterations, bool isSuccess)
        {
            AlgorithmName = algorithmName;
            Iterations = iterations;
            IsSuccess = isSuccess;
        }
    }
}