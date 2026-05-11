using System.Collections.Generic;
using GraphPathfinder.Models;

namespace GraphPathfinder.Algorithms
{
    /// <summary>
    /// Implementation of Dijkstra's algorithm for finding the shortest path in a graph with non-negative edge weights.
    /// Uses a priority queue for efficient selection of the next vertex.
    /// </summary>
    public class Dijkstra : BasePathAlgorithm
    {
        /// <summary>
        /// Returns the algorithm name "Dijkstra".
        /// </summary>
        public override string Name => "Dijkstra";

        /// <summary>
        /// Finds the shortest path between the start and target vertices using Dijkstra's algorithm.
        /// The algorithm does not support negative edge weights.
        /// </summary>
        /// <param name="graph">Graph to search in.</param>
        /// <param name="start">Start vertex of the path.</param>
        /// <param name="target">Target vertex of the path.</param>
        /// <returns>Search result with the reconstructed path and statistics.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the graph contains edges with negative weights.</exception>
        public override PathResult FindPath(Graph graph, Node start, Node target)
        {
            foreach (var edge in graph.Edges)
            {
                if (edge.Weight < 0)
                {
                    throw new InvalidOperationException("Dijkstra's algorithm does not work with negative edge weights.");
                }
            }

            var distances = new Dictionary<Node, double>();
            var previous = new Dictionary<Node, Node>();
            var priorityQueue = new PriorityQueue<Node, double>();
            var visited = new HashSet<Node>();

            foreach (var node in graph.Nodes)
            {
                distances[node] = double.PositiveInfinity;
            }

            distances[start] = 0;
            priorityQueue.Enqueue(start, 0);

            int iterations = 0;

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();

                if (visited.Contains(current))
                {
                    continue;
                }

                iterations++;
                visited.Add(current);

                if (current == target)
                {
                    break;
                }

                foreach (var edge in graph.GetOutgoingEdges(current))
                {
                    var neighbor = edge.Target;
                    var newDistance = distances[current] + edge.Weight;

                    if (newDistance < distances[neighbor])
                    {
                        distances[neighbor] = newDistance;
                        previous[neighbor] = current;
                        priorityQueue.Enqueue(neighbor, newDistance);
                    }
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