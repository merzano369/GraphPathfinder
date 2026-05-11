using System.Collections.Generic;
using GraphPathfinder.Models;

namespace GraphPathfinder.Algorithms
{
    /// <summary>
    /// Implementation of the Bellman-Ford algorithm for finding the shortest path in a graph.
    /// Supports negative edge weights and can detect negative weight cycles.
    /// </summary>
    public class BellmanFord : BasePathAlgorithm
    {
        /// <summary>
        /// Constant for floating-point number comparison with tolerance.
        /// </summary>
        private const double Epsilon = 1e-9;

        /// <summary>
        /// Returns the algorithm name "Bellman-Ford".
        /// </summary>
        public override string Name => "Bellman-Ford";

        /// <summary>
        /// Finds the shortest path between the start and target vertices using the Bellman-Ford algorithm.
        /// The algorithm supports negative edge weights and detects negative weight cycles.
        /// </summary>
        /// <param name="graph">Graph to search in.</param>
        /// <param name="start">Start vertex of the path.</param>
        /// <param name="target">Target vertex of the path.</param>
        /// <returns>Search result with the reconstructed path and statistics.</returns>
        /// <exception cref="InvalidOperationException">Thrown if a negative weight cycle is detected in the graph.</exception>
        public override PathResult FindPath(Graph graph, Node start, Node target)
        {
            var distances = new Dictionary<Node, double>();
            var previous = new Dictionary<Node, Node>();

            foreach (var node in graph.Nodes)
            {
                distances[node] = double.PositiveInfinity;
            }

            distances[start] = 0;
            int verticesCount = graph.Nodes.Count;

            int iterations = 0;

            for (int i = 0; i < verticesCount - 1; i++)
            {
                foreach (var edge in graph.Edges)
                {
                    iterations++;

                    var u = edge.Source;
                    var v = edge.Target;
                    var weight = edge.Weight;

                    if (!double.IsPositiveInfinity(distances[u]) && distances[u] + weight < distances[v] - Epsilon)
                    {
                        previous[v] = u;
                        distances[v] = distances[u] + weight;
                    }
                }
            }

            foreach (var edge in graph.Edges)
            {
                var u = edge.Source;
                var v = edge.Target;
                var weight = edge.Weight;

                if (!double.IsPositiveInfinity(distances[u]) && distances[u] + weight < distances[v] - Epsilon)
                {
                    throw new InvalidOperationException("A negative weight cycle was detected in the graph. No shortest path exists.");
                }
            }

            if (double.IsPositiveInfinity(distances[target]))
            {
                return new PathResult("Path not found.", Name, iterations);
            }

            return BuildPathResult(previous, target, graph.Nodes.Count, distances[target], iterations, Name);
        }
    }
}