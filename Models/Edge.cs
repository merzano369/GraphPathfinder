namespace GraphPathfinder.Models
{
    /// <summary>
    /// Represents a directed edge of a graph with source and target vertices and a weight.
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Source vertex of the edge (where the edge starts).
        /// </summary>
        public Node Source { get; }

        /// <summary>
        /// Target vertex of the edge (where the edge ends).
        /// </summary>
        public Node Target { get; }

        /// <summary>
        /// Weight of the edge (cost of traversing).
        /// </summary>
        public double Weight { get; private set; }

        /// <summary>
        /// Initializes a new edge with the specified vertices and weight.
        /// </summary>
        /// <param name="source">Source vertex of the edge.</param>
        /// <param name="target">Target vertex of the edge.</param>
        /// <param name="weight">Weight of the edge.</param>
        public Edge(Node source, Node target, double weight)
        {
            Source = source;
            Target = target;
            Weight = weight;
        }

        /// <summary>
        /// Sets a new weight for the edge (used for updating existing edges).
        /// </summary>
        /// <param name="weight">New weight of the edge.</param>
        internal void SetWeight(double weight)
        {
            Weight = weight;
        }
    }
}