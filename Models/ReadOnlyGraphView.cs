using System.Collections.Generic;

namespace GraphPathfinder.Models
{
    /// <summary>
    /// Represents a wrapped (immutable) view of a graph providing read-only access.
    /// Created based on an existing <see cref="Graph"/> object and prevents external modification.
    /// </summary>
    public sealed class ReadOnlyGraphView : IReadOnlyGraph
    {
        /// <summary>
        /// Reference to the original graph for which the read-only view is created.
        /// </summary>
        private readonly Graph _graph;

        /// <summary>
        /// Initializes a new read-only graph view based on the specified graph.
        /// </summary>
        /// <param name="graph">Graph to create read-only view for.</param>
        public ReadOnlyGraphView(Graph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Gets a read-only collection of graph vertices.
        /// </summary>
        public IReadOnlyList<Node> Nodes => _graph.Nodes;

        /// <summary>
        /// Gets a read-only collection of graph edges.
        /// </summary>
        public IReadOnlyList<Edge> Edges => _graph.Edges;

        /// <summary>
        /// Returns all edges outgoing from the specified vertex.
        /// </summary>
        /// <param name="node">Vertex to get outgoing edges for.</param>
        /// <returns>Collection of outgoing edges.</returns>
        public IEnumerable<Edge> GetOutgoingEdges(Node node)
        {
            return _graph.GetOutgoingEdges(node);
        }
    }
}