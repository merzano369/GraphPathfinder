using System;
using System.Collections.Generic;
using System.Linq;
using GraphPathfinder.Models;
using GraphPathfinder.Algorithms;

namespace GraphPathfinder.Controllers
{
    /// <summary>
    /// Graph management controller. Responsible for building graphs, executing pathfinding algorithms, and running benchmarks.
    /// </summary>
    public class GraphController
    {
        /// <summary>
        /// Internal mutable graph managed by the controller.
        /// </summary>
        private Graph _graph;

        /// <summary>
        /// Immutable view of the graph exposed to external clients.
        /// </summary>
        private IReadOnlyGraph _graphView;

        /// <summary>
        /// Initializes a new controller with an empty graph.
        /// </summary>
        public GraphController()
        {
            _graph = new Graph();
            _graphView = new ReadOnlyGraphView(_graph);
        }

        /// <summary>
        /// Executes a pathfinding algorithm between two vertices of the current graph.
        /// </summary>
        /// <param name="algorithm">Pathfinding algorithm.</param>
        /// <param name="start">Start vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <returns>Path search result or <c>null</c> if no path was found.</returns>
        public PathResult? Solve(IPathAlgorithm algorithm, Node start, Node target)
        {
            return algorithm.FindPath(_graph, start, target);
        }

        /// <summary>
        /// Runs a benchmark of all available algorithms on the current graph between the specified vertices.
        /// Returns comparison results of each algorithm's performance.
        /// </summary>
        /// <param name="start">Start vertex for the benchmark.</param>
        /// <param name="target">Target vertex for the benchmark.</param>
        /// <returns>List of benchmark results for each algorithm.</returns>
        public List<BenchmarkResult> RunBenchmark(Node start, Node target)
        {
            var results = new List<BenchmarkResult>();
            var algorithms = PathAlgorithmFactory.GetAll();

            foreach (var alg in algorithms)
            {
                try
                {
                    var result = alg.FindPath(_graph, start, target);
                    results.Add(new BenchmarkResult(alg.Name, result.Iterations, result.IsSuccess));
                }
                catch (InvalidOperationException)
                {
                    results.Add(new BenchmarkResult(alg.Name, 0, false));
                }
            }

            return results;
        }

        /// <summary>
        /// Clears the current graph and creates a new empty graph.
        /// </summary>
        public void ClearGraph()
        {
            _graph = new Graph();
            _graphView = new ReadOnlyGraphView(_graph);
        }

        /// <summary>
        /// Builds a new graph based on the specified vertex IDs and edge list.
        /// </summary>
        /// <param name="usedNodeIds">Vertex IDs to create.</param>
        /// <param name="validEdges">Tuples containing source ID, target ID, and weight for each edge.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="usedNodeIds"/> or <paramref name="validEdges"/> are <c>null</c>.</exception>
        public void BuildGraph(IEnumerable<int> usedNodeIds,
            IEnumerable<(int sourceId, int targetId, double weight)> validEdges)
        {
            if (usedNodeIds == null) throw new ArgumentNullException(nameof(usedNodeIds));
            if (validEdges == null) throw new ArgumentNullException(nameof(validEdges));

            var newGraph = new Graph();

            var nodeById = new Dictionary<int, Node>();
            foreach (int id in usedNodeIds.OrderBy(i => i))
            {
                var node = new Node(id, 0, 0);
                newGraph.AddNode(node);
                nodeById[id] = node;
            }

            foreach (var (sourceId, targetId, weight) in validEdges)
            {
                var source = nodeById[sourceId];
                var target = nodeById[targetId];
                newGraph.AddEdge(source, target, weight);
            }

            _graph = newGraph;
            _graphView = new ReadOnlyGraphView(_graph);
        }

        /// <summary>
        /// Gets the current graph in read-only mode.
        /// </summary>
        public IReadOnlyGraph CurrentGraph => _graphView;
    }
}