using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GraphPathfinder.Controllers;
using GraphPathfinder.Models;
using GraphPathfinder.Algorithms;
using GraphPathfinder.Services;

namespace GraphPathfinder.Presenters
{
    /// <summary>
    /// Presenter for the main form. Coordinates interaction between the View and Controller.
    /// Handles UI events, manages graph construction, algorithm execution, and result export.
    /// </summary>
    public class GraphPresenter
    {
        /// <summary>
        /// Reference to the view (UI).
        /// </summary>
        private readonly IMainView _view;

        /// <summary>
        /// Graph management controller.
        /// </summary>
        private readonly GraphController _controller;

        /// <summary>
        /// Result export service.
        /// </summary>
        private readonly ExportService _exportService;

        /// <summary>
        /// Graph vertex layout service.
        /// </summary>
        private readonly GraphLayoutService _layoutService;

        /// <summary>
        /// Last pathfinding result.
        /// </summary>
        private PathResult? _lastPathResult;

        /// <summary>
        /// Flag that prevents re-processing of changes during UI updates.
        /// </summary>
        private bool _isUpdating = false;

        /// <summary>
        /// Initializes a new presenter with the specified dependencies.
        /// </summary>
        /// <param name="view">View (UI).</param>
        /// <param name="controller">Graph management controller.</param>
        /// <param name="exportService">Result export service.</param>
        /// <param name="layoutService">Vertex layout service.</param>
        public GraphPresenter(IMainView view, GraphController controller, ExportService exportService, GraphLayoutService layoutService)
        {
            _view = view;
            _controller = controller;
            _exportService = exportService;
            _layoutService = layoutService;

            _view.FindPathRequested += OnFindPathRequested;
            _view.GraphChanged += OnGraphChanged;
            _view.ExportRequested += OnExportRequested;
            _view.ClearRequested += OnClearRequested;
        }

        /// <summary>
        /// Gets the last pathfinding result.
        /// </summary>
        public PathResult? LastPathResult => _lastPathResult;

        /// <summary>
        /// Gets the current graph in read-only mode.
        /// </summary>
        public IReadOnlyGraph CurrentGraph => _controller.CurrentGraph;

        /// <summary>
        /// Builds a graph based on adjacency matrix data from the UI.
        /// Invokes matrix parsing, graph construction, and visualization update.
        /// </summary>
        public void BuildGraphFromMatrix()
        {
            _lastPathResult = null;

            if (!TryParseGridParameters(out var validEdges, out var usedNodeIds))
            {
                _controller.ClearGraph();
                _view.UpdateStatus("Fix errors in the matrix");
                _view.RequestGraphRedraw();
                return;
            }

            try
            {
                _controller.BuildGraph(usedNodeIds, validEdges);
            }
            catch (Exception ex)
            {
                _controller.ClearGraph();
                _view.ShowError($"Graph construction error: {ex.Message}");
                _view.RequestGraphRedraw();
                return;
            }

            _layoutService.ArrangeInCircle(_controller.CurrentGraph, _view.CanvasWidth, _view.CanvasHeight);

            _view.UpdateStatus("Ready");
            SyncViewWithGraph();
        }

        /// <summary>
        /// Attempts to parse graph parameters from the adjacency matrix.
        /// </summary>
        /// <param name="validEdges">Output parameter: list of valid edges with weights.</param>
        /// <param name="usedNodeIds">Output parameter: set of used vertex identifiers.</param>
        /// <returns><c>true</c> if parsing was successful, otherwise — <c>false</c>.</returns>
        private bool TryParseGridParameters(
            out List<(int sourceId, int targetId, double weight)> validEdges,
            out HashSet<int> usedNodeIds)
        {
            int nodeCount = _view.NodeCount;

            validEdges = new List<(int sourceId, int targetId, double weight)>();
            usedNodeIds = new HashSet<int>();

            for (int i = 0; i < nodeCount; i++)
                usedNodeIds.Add(i);

            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = 0; j < nodeCount; j++)
                {
                    if (i == j) continue;

                    string cellStr = (_view.GetMatrixCellValue(i, j) ?? "").Trim();
                    if (string.IsNullOrEmpty(cellStr) || cellStr == "-")
                        continue;

                    string normalizedStr = cellStr.Replace(',', '.');
                    if (!double.TryParse(normalizedStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double weight))
                    {
                        _view.ShowError($"Error: '{cellStr}' is not a number (row {i}, column {j}).");
                        return false;
                    }

                    validEdges.Add((i, j, weight));
                }
            }

            return true;
        }

        /// <summary>
        /// Synchronizes the UI state with the current graph.
        /// Updates the adjacency matrix and vertex dropdown lists.
        /// </summary>
        private void SyncViewWithGraph()
        {
            _isUpdating = true;
            try
            {
                int nodeCount = _view.NodeCount;

                for (int i = 0; i < nodeCount; i++)
                {
                    for (int j = 0; j < nodeCount; j++)
                    {
                        if (i == j) continue;
                        _view.SetMatrixCellValue(i, j, "-");
                    }
                }

                foreach (var edge in _controller.CurrentGraph.Edges)
                {
                    _view.SetMatrixCellValue(edge.Source.Id, edge.Target.Id,
                        edge.Weight.ToString(CultureInfo.InvariantCulture));
                }

                var nodeIds = _controller.CurrentGraph.Nodes.Select(n => n.Id.ToString()).ToArray();
                _view.UpdateNodeComboboxes(nodeIds);
                _view.RequestGraphRedraw();
            }
            finally
            {
                _isUpdating = false;
            }
        }

        /// <summary>
        /// Event handler for the path finding request.
        /// Performs validation, launches the algorithm, and updates the UI.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnFindPathRequested(object? sender, EventArgs e)
        {
            int startId = _view.StartNodeIndex;
            int targetId = _view.TargetNodeIndex;

            if (startId < 0 || targetId < 0)
            {
                _view.ShowError("Error: Select start and end vertices.");
                return;
            }

            var startNode = _controller.CurrentGraph.Nodes.FirstOrDefault(n => n.Id == startId);
            var targetNode = _controller.CurrentGraph.Nodes.FirstOrDefault(n => n.Id == targetId);

            if (startNode == null || targetNode == null) return;

            if (startNode == targetNode)
            {
                _view.UpdateStatus("Start and finish coincide. Path cost: 0.");
                _lastPathResult = new PathResult(new List<Node> { startNode }, 0, 0, "N/A");
                _view.RequestGraphRedraw();
                return;
            }

            if (_controller.CurrentGraph.Edges.Count == 0)
            {
                _view.ShowError("Graph has no edges. Path is impossible.");
                return;
            }

            IPathAlgorithm algorithm = PathAlgorithmFactory.Create(_view.SelectedAlgorithmName);

            try
            {
                _lastPathResult = _controller.Solve(algorithm, startNode, targetNode);

                if (_lastPathResult != null && _lastPathResult.IsSuccess)
                {
                    _view.UpdateStatus($"Path found! Cost: {_lastPathResult.TotalDistance}");
                }
                else if (_lastPathResult != null)
                {
                    _view.UpdateStatus(_lastPathResult.ErrorMessage ?? "Path not found.");
                }

                _view.RequestGraphRedraw();

                var benchmarkResults = _controller.RunBenchmark(startNode, targetNode);
                string text = "Number of iterations:\r\n";
                text += string.Join("\r\n", benchmarkResults.Select(r =>
                    r.IsSuccess ? $"{r.AlgorithmName}: {r.Iterations} iter." : $"{r.AlgorithmName}: Failed"));
                _view.SetBenchmarkResults(text);
            }
            catch (InvalidOperationException ex)
            {
                _view.ShowError($"Algorithm error: {ex.Message}");
                _view.SetBenchmarkResults("Benchmark unavailable due to algorithm error");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Unexpected error: {ex.Message}");
            }
        }

        /// <summary>
        /// Event handler for the graph change event.
        /// Checks the update flag and initiates graph reconstruction.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnGraphChanged(object? sender, EventArgs e)
        {
            if (_isUpdating) return;
            BuildGraphFromMatrix();
        }

        /// <summary>
        /// Event handler for the export request event.
        /// Saves the last successful pathfinding result as text and exports the current graph visualization as an image.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExportRequested(object? sender, EventArgs e)
        {
            if (_lastPathResult == null)
            {
                _view.ShowError("Error: No results to export.");
                return;
            }

            if (!_lastPathResult.IsSuccess)
            {
                _view.ShowError("No successful path to export.");
                return;
            }

            try
            {
                string baseFileName = "GraphResult";
                string textFile = $"{baseFileName}.txt";
                string imageFile = $"{baseFileName}.png";

                _exportService.ExportPath(_lastPathResult, textFile);

                using (var graphImage = _view.GetGraphImage())
                {
                    _exportService.ExportImage(graphImage, imageFile);
                }

                _view.UpdateStatus("Successfully saved text and image");
            }
            catch (Exception ex)
            {
                _view.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Event handler for the clear request event.
        /// Clears the graph and resets the UI to the initial state.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnClearRequested(object? sender, EventArgs e)
        {
            _lastPathResult = null;
            _controller.ClearGraph();

            SyncViewWithGraph();

            _view.UpdateStatus("Cleared");
        }
    }
}
