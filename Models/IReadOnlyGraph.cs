using System.Collections.Generic;

namespace GraphPathfinder.Models
{
    /// <summary>
    /// Interface for read-only graph representation.
    /// Provides access to vertices and edges without modification capability.
    /// </summary>
    public interface IReadOnlyGraph
    {
        /// <summary>
        /// Gets a read-only collection of graph vertices.
        /// </summary>
        IReadOnlyList<Node> Nodes { get; }

        /// <summary>
        /// Gets a read-only collection of graph edges.
        /// </summary>
        IReadOnlyList<Edge> Edges { get; }

        /// <summary>
        /// Returns all edges outgoing from the specified vertex.
        /// </summary>
        /// <param name="node">Vertex to get outgoing edges for.</param>
        /// <returns>Collection of outgoing edges.</returns>
        IEnumerable<Edge> GetOutgoingEdges(Node node);
    }
}