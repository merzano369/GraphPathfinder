using System;
using System.Collections.Generic;
using System.Linq;
using GraphPathfinder.Models;

namespace GraphPathfinder.Algorithms
{
    /// <summary>
    /// Implementation of the A* algorithm for finding the shortest path in a graph.
    /// Uses a heuristic estimate to speed up the search.
    /// Does not support negative edge weights.
    /// </summary>
    public class AStar : BasePathAlgorithm
    {
        /// <summary>
        /// Returns the algorithm name "A*".
        /// </summary>
        public override string Name => "A*";

        /// <summary>
        /// Finds the shortest path between the start and target vertices using the A* algorithm.
        /// The algorithm uses a heuristic function to estimate the distance to the goal.
        /// </summary>
        /// <param name="graph">Graph to search in.</param>
        /// <param name="start">Start vertex of the path.</param>
        /// <param name="target">Target vertex of the path.</param>
        /// <returns>Search result with the reconstructed path and statistics.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the graph contains edges with negative weights.</exception>
        public override PathResult FindPath(Graph graph, Node start, Node target)
        {
            if (graph.Edges.Any(e => e.Weight < 0))
            {
                throw new InvalidOperationException("A* algorithm does not work with negative edge weights.");
            }

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
            heuristicScale *= 0.99;

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

                iterations++;
                visited.Add(current);

                if (current == target) break;

                foreach (var edge in graph.GetOutgoingEdges(current))
                {
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