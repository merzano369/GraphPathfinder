using System.Collections.Generic;
using GraphPathfinder.Models;

namespace GraphPathfinder.Algorithms
{
    /// <summary>
    /// Base abstract class for all shortest path algorithms.
    /// Implements common path reconstruction functionality.
    /// </summary>
    public abstract class BasePathAlgorithm : IPathAlgorithm
    {
        /// <summary>
        /// Gets the name of the algorithm.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Finds the shortest path between two graph vertices.
        /// Implementation depends on the specific algorithm.
        /// </summary>
        /// <param name="graph">Graph to search in.</param>
        /// <param name="start">Start vertex of the path.</param>
        /// <param name="target">Target vertex of the path.</param>
        /// <returns>Search result containing the path and execution statistics.</returns>
        public abstract PathResult FindPath(Graph graph, Node start, Node target);

        /// <summary>
        /// Reconstructs the path from the start vertex to the target using the predecessor dictionary.
        /// </summary>
        /// <param name="previous">Dictionary mapping each vertex to its predecessor on the path.</param>
        /// <param name="target">Target vertex to start path reconstruction from.</param>
        /// <param name="maxNodes">Maximum number of vertices in the graph (for cycle protection).</param>
        /// <param name="totalCost">Total cost of the found path.</param>
        /// <param name="iterations">Number of algorithm iterations.</param>
        /// <param name="algorithmName">Name of the algorithm.</param>
        /// <returns>Search result with the reconstructed path.</returns>
        protected PathResult BuildPathResult(Dictionary<Node, Node> previous, Node target, int maxNodes, double totalCost, int iterations, string algorithmName)
        {
            var path = new List<Node>();
            var currentNode = target;

            while (currentNode != null)
            {
                if (path.Count > maxNodes)
                {
                    return new PathResult("Critical error: Cycle detected during path reconstruction.");
                }

                path.Add(currentNode);
                previous.TryGetValue(currentNode, out var nextNode);
                currentNode = nextNode;
            }

            path.Reverse();
            var result = new PathResult(path, totalCost, iterations, algorithmName);
            return result;
        }
    }
}