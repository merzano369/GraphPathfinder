using GraphPathfinder.Models;

namespace GraphPathfinder.Algorithms
{
    /// <summary>
    /// Interface for a pathfinding algorithm in a graph.
    /// Defines the contract for all routing algorithm implementations.
    /// </summary>
    public interface IPathAlgorithm
    {
        /// <summary>
        /// Gets the name of the pathfinding algorithm.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Finds the shortest path between two graph vertices.
        /// </summary>
        /// <param name="graph">Graph to search in.</param>
        /// <param name="start">Start vertex of the path.</param>
        /// <param name="target">Target vertex of the path.</param>
        /// <returns>Search result containing the path and execution statistics.</returns>
        PathResult FindPath(Graph graph, Node start, Node target);
    }
}