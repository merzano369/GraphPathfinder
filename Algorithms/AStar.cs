using System;
using System.Collections.Generic;
using System.Linq;
using GraphPathfinder.Models;

namespace GraphPathfinder.Algorithms
{
    /// <summary>
    /// Implementation of the A* algorithm for finding the shortest path in a graph.
    /// Uses a heuristic estimate to speed up the search.
    /// Automatically switches between the standard fast variant and a variant that supports negative edge weights.
    /// Detects negative weight cycles when working with negative edge weights.
    /// </summary>
    public class AStar : BasePathAlgorithm
    {
        /// <summary>
        /// Returns the algorithm name "A*".
        /// </summary>
        public override string Name => "A*";

        /// <summary>
        /// Finds the shortest path between the start and target vertices.
        /// Uses the standard A* algorithm for graphs without negative edge weights.
        /// If the graph contains negative edge weights, uses a variant that allows node re-opening.
        /// </summary>
        /// <param name="graph">Graph to search in.</param>
        /// <param name="start">Start vertex of the path.</param>
        /// <param name="target">Target vertex of the path.</param>
        /// <returns>Search result with the reconstructed path and statistics.</returns>
        /// <exception cref="InvalidOperationException">Thrown if a negative weight cycle is detected in the graph.</exception>
        public override PathResult FindPath(Graph graph, Node start, Node target)
        {
            bool hasNegativeWeights = graph.Edges.Any(e => e.Weight < 0);
            double heuristicScale = CalculateHeuristicScale(graph);

            if (hasNegativeWeights)
            {
                return RunAStarWithNegativeWeights(graph, start, target, heuristicScale);
            }

            return RunStandardAStar(graph, start, target, heuristicScale);
        }

        /// <summary>
        /// Runs the standard A* algorithm for graphs without negative edge weights.
        /// </summary>
        /// <param name="graph">Graph to search in.</param>
        /// <param name="start">Start vertex of the path.</param>
        /// <param name="target">Target vertex of the path.</param>
        /// <param name="heuristicScale">Heuristic multiplier used to scale the heuristic estimate.</param>
        /// <returns>Search result with the reconstructed path and statistics.</returns>
        private PathResult RunStandardAStar(Graph graph, Node start, Node target, double heuristicScale)
        {
            var gScores = graph.Nodes.ToDictionary(n => n, n => double.PositiveInfinity);
            var fScores = graph.Nodes.ToDictionary(n => n, n => double.PositiveInfinity);
            var previous = new Dictionary<Node, Node>();
            var visited = new HashSet<Node>();

            gScores[start] = 0;
            fScores[start] = Heuristic(start, target) * heuristicScale;

            var priorityQueue = new PriorityQueue<Node, double>();
            priorityQueue.Enqueue(start, fScores[start]);

            int iterations = 0;

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();

                if (visited.Contains(current))
                {
                    continue;
                }

                visited.Add(current);

                if (current == target)
                {
                    break;
                }

                foreach (var edge in graph.GetOutgoingEdges(current))
                {
                    iterations++;

                    var neighbor = edge.Target;
                    var tentativeGScore = gScores[current] + edge.Weight;

                    if (tentativeGScore < gScores[neighbor])
                    {
                        previous[neighbor] = current;
                        gScores[neighbor] = tentativeGScore;

                        double h = Heuristic(neighbor, target) * heuristicScale;
                        fScores[neighbor] = tentativeGScore + h;

                        priorityQueue.Enqueue(neighbor, fScores[neighbor]);
                    }
                }
            }

            if (double.IsPositiveInfinity(gScores[target]))
            {
                return new PathResult("Path not found.", Name, iterations);
            }

            return BuildPathResult(previous, target, graph.Nodes.Count, gScores[target], iterations, Name);
        }

        /// <summary>
        /// Runs a variant of the A* algorithm that supports negative edge weights by allowing node re-opening.
        /// Also detects negative weight cycles.
        /// </summary>
        /// <param name="graph">Graph to search in.</param>
        /// <param name="start">Start vertex of the path.</param>
        /// <param name="target">Target vertex of the path.</param>
        /// <param name="heuristicScale">Heuristic multiplier used to scale the heuristic estimate.</param>
        /// <returns>Search result with the reconstructed path and statistics.</returns>
        /// <exception cref="InvalidOperationException">Thrown if a negative weight cycle is detected in the graph.</exception>
        private PathResult RunAStarWithNegativeWeights(Graph graph, Node start, Node target, double heuristicScale)
        {
            var gScores = graph.Nodes.ToDictionary(n => n, n => double.PositiveInfinity);
            var fScores = graph.Nodes.ToDictionary(n => n, n => double.PositiveInfinity);
            var previous = new Dictionary<Node, Node>();
            var enqueueCount = graph.Nodes.ToDictionary(n => n, n => 0);

            gScores[start] = 0;
            fScores[start] = Heuristic(start, target) * heuristicScale;

            var priorityQueue = new PriorityQueue<Node, double>();
            priorityQueue.Enqueue(start, fScores[start]);
            enqueueCount[start]++;

            int iterations = 0;
            int verticesCount = graph.Nodes.Count;

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();

                foreach (var edge in graph.GetOutgoingEdges(current))
                {
                    iterations++;

                    var neighbor = edge.Target;
                    var tentativeGScore = gScores[current] + edge.Weight;

                    if (tentativeGScore < gScores[neighbor])
                    {
                        previous[neighbor] = current;
                        gScores[neighbor] = tentativeGScore;

                        double h = Heuristic(neighbor, target) * heuristicScale;
                        fScores[neighbor] = tentativeGScore + h;

                        priorityQueue.Enqueue(neighbor, fScores[neighbor]);
                        enqueueCount[neighbor]++;

                        if (enqueueCount[neighbor] >= verticesCount)
                        {
                            throw new InvalidOperationException(
                                "A negative weight cycle was detected in the graph. No shortest path exists.");
                        }
                    }
                }
            }

            if (double.IsPositiveInfinity(gScores[target]))
            {
                return new PathResult("Path not found.", Name, iterations);
            }

            return BuildPathResult(previous, target, graph.Nodes.Count, gScores[target], iterations, Name);
        }

        /// <summary>
        /// Calculates the heuristic scaling factor based on the graph's edges.
        /// </summary>
        /// <param name="graph">Graph to analyze.</param>
        /// <returns>Heuristic scaling factor.</returns>
        private static double CalculateHeuristicScale(Graph graph)
        {
            double heuristicScale = 1.0;
            foreach (var edge in graph.Edges)
            {
                double directDist = Heuristic(edge.Source, edge.Target);
                if (directDist > 0.001)
                {
                    double scale = edge.Weight / directDist;
                    if (scale < heuristicScale)
                    {
                        heuristicScale = scale;
                    }
                }
            }

            return heuristicScale * 0.99;
        }

        /// <summary>
        /// Heuristic function that estimates the distance between two vertices using Euclidean distance.
        /// </summary>
        /// <param name="a">First vertex.</param>
        /// <param name="b">Second vertex.</param>
        /// <returns>Estimated distance between the vertices.</returns>
        private static double Heuristic(Node a, Node b)
        {
            double dX = a.X - b.X;
            double dY = a.Y - b.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }
    }
}
