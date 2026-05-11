namespace GraphPathfinder.Models
{
    /// <summary>
    /// Represents a vertex (node) of a graph with a unique identifier and positioning coordinates.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Unique numeric identifier of the vertex.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// X-coordinate of the vertex on the plane (used for visualization).
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y-coordinate of the vertex on the plane (used for visualization).
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Initializes a new vertex with the specified identifier and coordinates.
        /// </summary>
        /// <param name="id">Unique identifier of the vertex.</param>
        /// <param name="x">X-coordinate of the vertex.</param>
        /// <param name="y">Y-coordinate of the vertex.</param>
        public Node(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }
}