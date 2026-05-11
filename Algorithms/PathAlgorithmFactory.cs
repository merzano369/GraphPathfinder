using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphPathfinder.Algorithms
{
    /// <summary>
    /// Factory for creating pathfinding algorithm instances.
    /// Provides a centralized location for obtaining algorithms by name and getting the list of available algorithms.
    /// </summary>
    public static class PathAlgorithmFactory
    {
        /// <summary>
        /// Constant name of Dijkstra's algorithm.
        /// </summary>
        public const string DijkstraName = "Dijkstra";

        /// <summary>
        /// Constant name of the Bellman-Ford algorithm.
        /// </summary>
        public const string BellmanFordName = "Bellman-Ford";

        /// <summary>
        /// Constant name of the A* algorithm.
        /// </summary>
        public const string AStarName = "A*";

        /// <summary>
        /// Creates an instance of a pathfinding algorithm by its name.
        /// </summary>
        /// <param name="algorithmName">Algorithm name (one of: "Dijkstra", "Bellman-Ford", "A*").</param>
        /// <returns>Algorithm instance implementing <see cref="IPathAlgorithm"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified algorithm name is unknown.</exception>
        public static IPathAlgorithm Create(string algorithmName)
        {
            return algorithmName switch
            {
                DijkstraName => new Dijkstra(),
                BellmanFordName => new BellmanFord(),
                AStarName => new AStar(),
                _ => throw new ArgumentException($"Unknown algorithm: {algorithmName}")
            };
        }

        /// <summary>
        /// Returns a collection of all available pathfinding algorithms.
        /// </summary>
        /// <returns>Collection of algorithm instances.</returns>
        public static IEnumerable<IPathAlgorithm> GetAll()
        {
            return new IPathAlgorithm[] 
            { 
                new Dijkstra(), 
                new BellmanFord(), 
                new AStar() 
            };
        }

        /// <summary>
        /// Returns an array of names of all available algorithms.
        /// </summary>
        /// <returns>Array of algorithm name strings.</returns>
        public static string[] GetAvailableNames()
        {
            return new[] 
            { 
                DijkstraName, 
                BellmanFordName, 
                AStarName 
            };
        }
    }
}