using System;

namespace GraphPathfinder.Presenters
{
    /// <summary>
    /// Interface for the main View for presenter interaction with the UI.
    /// Defines the contract for UI data access and user event subscription.
    /// </summary>
    public interface IMainView
    {
        /// <summary>
        /// Number of graph vertices.
        /// </summary>
        int NodeCount { get; set; }

        /// <summary>
        /// Index of the selected algorithm in the list.
        /// </summary>
        int SelectedAlgorithmIndex { get; }

        /// <summary>
        /// Name of the selected algorithm.
        /// </summary>
        string SelectedAlgorithmName { get; }

        /// <summary>
        /// Index of the start vertex.
        /// </summary>
        int StartNodeIndex { get; }

        /// <summary>
        /// Index of the target vertex.
        /// </summary>
        int TargetNodeIndex { get; }
        
        /// <summary>
        /// Width of the canvas for graph drawing.
        /// </summary>
        int CanvasWidth { get; }

        /// <summary>
        /// Height of the canvas for graph drawing.
        /// </summary>
        int CanvasHeight { get; }

        /// <summary>
        /// Gets the adjacency matrix cell value by row and column indices.
        /// </summary>
        /// <param name="row">Row index (from).</param>
        /// <param name="col">Column index (to).</param>
        /// <returns>String with edge weight value or <c>null</c> if the cell is unavailable.</returns>
        string? GetMatrixCellValue(int row, int col);

        /// <summary>
        /// Sets the adjacency matrix cell value.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <param name="value">Value to set.</param>
        void SetMatrixCellValue(int row, int col, string value);

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="message">Error message text.</param>
        void ShowError(string message);

        /// <summary>
        /// Updates the status text in the UI.
        /// </summary>
        /// <param name="status">New status text.</param>
        void UpdateStatus(string status);

        /// <summary>
        /// Sets the benchmark results text.
        /// </summary>
        /// <param name="text">Benchmark results text.</param>
        void SetBenchmarkResults(string text);

        /// <summary>
        /// Requests a graph redraw.
        /// </summary>
        void RequestGraphRedraw();

        /// <summary>
        /// Updates vertex dropdown lists with the specified identifiers.
        /// </summary>
        /// <param name="nodeIds">Array of vertex identifier strings.</param>
        void UpdateNodeComboboxes(string[] nodeIds);

        /// <summary>
        /// Event that occurs when the user requests to find a path.
        /// </summary>
        event EventHandler? FindPathRequested;

        /// <summary>
        /// Event that occurs when the graph is changed by the user.
        /// </summary>
        event EventHandler? GraphChanged;

        /// <summary>
        /// Event that occurs when export results are requested.
        /// </summary>
        event EventHandler? ExportRequested;

        /// <summary>
        /// Event that occurs when graph clearing is requested.
        /// </summary>
        event EventHandler? ClearRequested;
    }
}