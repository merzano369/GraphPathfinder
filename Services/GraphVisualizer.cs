using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using GraphPathfinder.Models;

namespace GraphPathfinder.Services
{
    /// <summary>
    /// Service for graph and pathfinding result visualization.
    /// Draws vertices, edges, and highlights the found path on a Graphics canvas.
    /// </summary>
    public class GraphVisualizer
    {
        /// <summary>
        /// Minimum allowed coordinate for vertices and edges.
        /// Vertices with coordinates less than this value are ignored during visualization.
        /// </summary>
        private const float MinCoordinate = -1000f;

        /// <summary>
        /// Maximum allowed coordinate for vertices and edges.
        /// Vertices with coordinates greater than this value are ignored during visualization.
        /// </summary>
        private const float MaxCoordinate = 3000f;

        /// <summary>
        /// Visualizes the graph on the specified Graphics canvas.
        /// Draws all vertices, edges, and optionally highlights the found path.
        /// </summary>
        /// <param name="graph">Graph to visualize.</param>
        /// <param name="g">Graphics context for drawing.</param>
        /// <param name="pathResult">Optional pathfinding result to highlight.</param>
        /// <exception cref="InvalidOperationException">Thrown when exceeding maximum vertex count, insufficient memory, or visualization error.</exception>
        public void Visualize(IReadOnlyGraph graph, Graphics g, PathResult? pathResult = null)
        {
            try
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                if (graph == null) return;

                if (graph.Nodes.Count > 500)
                {
                    throw new InvalidOperationException("Too many nodes for visualization (maximum 500).");
                }

                int nodeRadius = 15;
                int nodeDiameter = nodeRadius * 2;

                using Font weightFont = new Font("Arial", 9, FontStyle.Bold);
                using StringFormat format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                using (Pen edgePen = new Pen(Color.DarkGray, 2))
                using (AdjustableArrowCap arrowCap = new AdjustableArrowCap(4, 4, true))
                {
                    edgePen.CustomEndCap = arrowCap;

                    if (graph.Edges != null)
                    {
                        foreach (var edge in graph.Edges)
                        {
                            if (edge.Source == edge.Target) continue;

                            float x1 = (float)edge.Source.X;
                            float y1 = (float)edge.Source.Y;
                            float x2 = (float)edge.Target.X;
                            float y2 = (float)edge.Target.Y;

                            if (x1 < MinCoordinate || y1 < MinCoordinate || x2 > MaxCoordinate || y2 > MaxCoordinate) continue;

                            DrawEdgeWithArrow(g, edgePen, edge.Source, edge.Target, nodeRadius);

                            float offsetRatio = 0.3f; 
                            float textX = x1 + (x2 - x1) * offsetRatio;
                            float textY = y1 + (y2 - y1) * offsetRatio;

                            string weightText = edge.Weight.ToString("F1");
                            var textSize = g.MeasureString(weightText, weightFont);

                            g.FillRectangle(Brushes.White, textX - textSize.Width / 2, textY - textSize.Height / 2, textSize.Width, textSize.Height);
                            g.DrawString(weightText, weightFont, Brushes.DarkGreen, textX, textY, format);
                        }
                    }
                }

                if (pathResult != null && pathResult.Path != null && pathResult.Path.Count > 1)
                {
                    using (Pen highlightPen = new Pen(Color.Red, 4))
                    using (AdjustableArrowCap bigArrowCap = new AdjustableArrowCap(5, 5, true))
                    {
                        highlightPen.CustomEndCap = bigArrowCap;

                        for (int i = 0; i < pathResult.Path.Count - 1; i++)
                        {
                            var n1 = pathResult.Path[i];
                            var n2 = pathResult.Path[i + 1];
                            
                            DrawEdgeWithArrow(g, highlightPen, n1, n2, nodeRadius);
                        }
                    }
                }

                using (Font font = new Font("Arial", 10, FontStyle.Bold))
                using (Pen nodeBorder = new Pen(Color.Navy, 2))
                {
                    if (graph.Nodes != null)
                    {
                        foreach (var node in graph.Nodes)
                        {
                            if (node.X < MinCoordinate || node.Y < MinCoordinate || node.X > MaxCoordinate || node.Y > MaxCoordinate) continue;

                            RectangleF rect = new RectangleF((float)node.X - nodeRadius, (float)node.Y - nodeRadius, nodeDiameter, nodeDiameter);

                            g.FillEllipse(Brushes.SkyBlue, rect);
                            g.DrawEllipse(nodeBorder, rect);

                            PointF center = new PointF((float)node.X, (float)node.Y);
                            g.DrawString(node.Id.ToString(), font, Brushes.Black, center, format);
                        }
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                throw new InvalidOperationException("Not enough memory to visualize the graph.");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
            {
                throw new InvalidOperationException($"Visualization error: {ex.Message}");
            }
        }

        /// <summary>
        /// Draws an edge with an arrow between two vertices.
        /// </summary>
        /// <param name="g">Graphics context for drawing.</param>
        /// <param name="pen">Pen for drawing the edge line.</param>
        /// <param name="source">Source vertex of the edge.</param>
        /// <param name="target">Target vertex of the edge.</param>
        /// <param name="radius">Vertex radius for calculating start and end of the line.</param>
        private void DrawEdgeWithArrow(Graphics g, Pen pen, Node source, Node target, int radius)
        {
            float dx = (float)(target.X - source.X);
            float dy = (float)(target.Y - source.Y);
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance <= radius * 2) return; 

            float dirX = dx / distance;
            float dirY = dy / distance;

            float startX = (float)source.X + dirX * radius;
            float startY = (float)source.Y + dirY * radius;
            float endX = (float)target.X - dirX * radius;
            float endY = (float)target.Y - dirY * radius;

            g.DrawLine(pen, startX, startY, endX, endY);
        }
    }
}