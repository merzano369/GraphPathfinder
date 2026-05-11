using GraphPathfinder.Models;
using System.Linq;

namespace GraphPathfinder.Services
{
    /// <summary>
    /// Service for calculating graph vertex layout on a plane.
    /// Provides methods for automatic vertex placement in various geometric configurations.
    /// </summary>
    public class GraphLayoutService
    {
        /// <summary>
        /// Arranges graph vertices in a circle centered in the specified area.
        /// Vertices are evenly distributed along the circle according to their count.
        /// </summary>
        /// <param name="graph">Graph whose vertices need to be arranged.</param>
        /// <param name="width">Width of the layout area.</param>
        /// <param name="height">Height of the layout area.</param>
        public void ArrangeInCircle(IReadOnlyGraph graph, double width, double height)
        {
            var nodes = graph.Nodes.ToList();
            if (nodes.Count == 0) return;

            double centerX = width / 2.0;
            double centerY = height / 2.0;
            double radius = Math.Min(centerX, centerY) * 0.7;

            for (int i = 0; i < nodes.Count; i++)
            {
                double angle = 2 * Math.PI * i / nodes.Count;
                nodes[i].X = centerX + radius * Math.Cos(angle - Math.PI / 2);
                nodes[i].Y = centerY + radius * Math.Sin(angle - Math.PI / 2);
            }
        }
    }
}