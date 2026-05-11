using System.Collections.Generic;

namespace GraphPathfinder.Models
{
    /// <summary>
    /// Represents the result of a shortest path search in a graph.
    /// Contains information about the found path, its cost, number of algorithm iterations, and algorithm name.
    /// </summary>
    public class PathResult
    {
        /// <summary>
        /// List of vertices forming the found path, in order from start to end.
        /// </summary>
        public List<Node> Path { get; }

        /// <summary>
        /// Total distance (weight) of the found path.
        /// Equals <c>double.PositiveInfinity</c> if no path was found.
        /// </summary>
        public double TotalDistance { get; }

        /// <summary>
        /// Number of iterations performed by the algorithm to find the path.
        /// </summary>
        public int Iterations { get; }

        /// <summary>
        /// Error message if the path could not be found (equals <c>null</c> on success).
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// Returns <c>true</c> if the path was found successfully, otherwise — <c>false</c>.
        /// </summary>
        public bool IsSuccess => ErrorMessage == null;

        /// <summary>
        /// Name of the algorithm that found the path.
        /// </summary>
        public string AlgorithmName { get; }

        /// <summary>
        /// Initializes a new path search result with the full set of parameters.
        /// </summary>
        /// <param name="path">List of path vertices.</param>
        /// <param name="totalDistance">Total path distance.</param>
        /// <param name="iterations">Number of algorithm iterations.</param>
        /// <param name="algorithmName">Name of the algorithm.</param>
        public PathResult(List<Node> path, double totalDistance, int iterations, string algorithmName)
        {
            Path = path;
            TotalDistance = totalDistance;
            Iterations = iterations;
            ErrorMessage = null;
            AlgorithmName = algorithmName;
        }

        /// <summary>
        /// Initializes a new path search result containing only an error message.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public PathResult(string errorMessage)
        {
            Path = new List<Node>();
            TotalDistance = double.PositiveInfinity;
            Iterations = 0;
            ErrorMessage = errorMessage;
            AlgorithmName = "Unknown";
        }

        /// <summary>
        /// Initializes a new path search result with an error message and algorithm name.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="algorithmName">Name of the algorithm.</param>
        public PathResult(string errorMessage, string algorithmName)
        {
            Path = new List<Node>();
            TotalDistance = double.PositiveInfinity;
            Iterations = 0;
            ErrorMessage = errorMessage;
            AlgorithmName = algorithmName;
        }

        /// <summary>
        /// Initializes a new path search result with an error message, algorithm name, and iteration count.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="algorithmName">Name of the algorithm.</param>
        /// <param name="iterations">Number of algorithm iterations.</param>
        public PathResult(string errorMessage, string algorithmName, int iterations)
        {
            Path = new List<Node>();
            TotalDistance = double.PositiveInfinity;
            Iterations = iterations;
            ErrorMessage = errorMessage;
            AlgorithmName = algorithmName;
        }
    }
}