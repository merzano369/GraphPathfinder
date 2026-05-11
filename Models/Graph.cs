using System.Collections.Generic;
using System.Linq;

namespace GraphPathfinder.Models
{
    /// <summary>
    /// Represents a graph with a set of vertices and directed edges.
    /// Implements the <see cref="IReadOnlyGraph"/> interface to provide read-only access.
    /// </summary>
    public class Graph : IReadOnlyGraph
    {
        /// <summary>
        /// Internal collection of graph vertices.
        /// </summary>
        private readonly List<Node> _nodes = new List<Node>();

        /// <summary>
        /// Collection of graph vertices, available as read-only.
        /// </summary>
        public IReadOnlyList<Node> Nodes => _nodes.AsReadOnly();

        /// <summary>
        /// Internal collection of graph edges.
        /// </summary>
        private readonly List<Edge> _edges = new List<Edge>();

        /// <summary>
        /// Collection of graph edges, available as read-only.
        /// </summary>
        public IReadOnlyList<Edge> Edges => _edges.AsReadOnly();

        /// <summary>
        /// Adds a vertex to the graph.
        /// </summary>
        /// <param name="node">Vertex to add.</param>
        public void AddNode(Node node)
        {
            _nodes.Add(node);
        }

        /// <summary>
        /// Maximum allowed edge weight. Used for validation.
        /// </summary>
        private const double MaxWeight = 100000;

        /// <summary>
        /// Adds a directed edge between two vertices with the specified weight.
        /// If an edge between the specified vertices already exists, the weight is updated.
        /// </summary>
        /// <param name="source">Source vertex of the edge.</param>
        /// <param name="target">Target vertex of the edge.</param>
        /// <param name="weight">Edge weight (must be in range from -MaxWeight to MaxWeight).</param>
        /// <exception cref="ArgumentException">Thrown when attempting to create a loop (edge to itself).</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when weight is outside the allowed bounds.</exception>
        public void AddEdge(Node source, Node target, double weight)
        {
            if (source.Id == target.Id)
            {
                throw new ArgumentException("Cannot create a loop (edge to itself).");
            }

            if (weight > MaxWeight || weight < -MaxWeight)
            {
                throw new ArgumentOutOfRangeException(nameof(weight), $"Weight must be in range from -{MaxWeight} to {MaxWeight}.");
            }

            var existingEdge = _edges.FirstOrDefault(e => e.Source == source && e.Target == target);
            if (existingEdge != null)
            {
                existingEdge.SetWeight(weight);
                return;
            }

            _edges.Add(new Edge(source, target, weight));
        }

        /// <summary>
        /// Returns all edges outgoing from the specified vertex.
        /// </summary>
        /// <param name="node">Vertex to get outgoing edges for.</param>
        /// <returns>Collection of outgoing edges.</returns>
        public IEnumerable<Edge> GetOutgoingEdges(Node node)
        {
            return _edges.Where(e => e.Source == node);
        }
    }
}