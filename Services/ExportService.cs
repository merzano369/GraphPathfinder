using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using GraphPathfinder.Models;

namespace GraphPathfinder.Services
{
    /// <summary>
    /// Service for exporting pathfinding results.
    /// Supports exporting the result as text and saving a rendered graph image.
    /// </summary>
    public class ExportService
    {
        /// <summary>
        /// Exports the pathfinding result to a text file at the specified path.
        /// The file contains the algorithm name, total distance, iteration count, and list of path vertices.
        /// </summary>
        /// <param name="result">Pathfinding result to export.</param>
        /// <param name="filePath">Path to the file where the result will be saved.</param>
        /// <exception cref="InvalidOperationException">Thrown on file write errors (file locked by another program or insufficient access rights).</exception>
        public void ExportPath(PathResult result, string filePath)
        {
            try
            {
                using var writer = new StreamWriter(filePath);
                writer.WriteLine($"Algorithm: {result.AlgorithmName}");
                writer.WriteLine($"Path Result - Total Distance: {result.TotalDistance}");
                writer.WriteLine($"Iterations: {result.Iterations}");
                writer.WriteLine("Nodes:");
                foreach (var node in result.Path)
                    writer.WriteLine(node.Id);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException($"File is locked by another program. Close the file and try again: {ex.Message}");
            }
            catch (UnauthorizedAccessException)
            {
                throw new InvalidOperationException("No access to the file. Check write permissions.");
            }
        }

        /// <summary>
        /// Saves the specified bitmap image to a file.
        /// </summary>
        /// <param name="image">Bitmap image to export.</param>
        /// <param name="filePath">Target file path (PNG).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="image"/> or <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when image saving fails.</exception>
        public void ExportImage(Bitmap image, string filePath)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));

            try
            {
                image.Save(filePath, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save image: {ex.Message}");
            }
        }
    }
}
