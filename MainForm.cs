using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GraphPathfinder.Controllers;
using GraphPathfinder.Services;
using GraphPathfinder.Models;
using GraphPathfinder.Presenters;
using GraphPathfinder.Algorithms;

namespace GraphPathfinder
{
    /// <summary>
    /// Main application form. Implements the <see cref="IMainView"/> interface and provides user interaction with the application.
    /// Contains controls for algorithm selection, adjacency matrix input, and graph visualization.
    /// </summary>
    public partial class MainForm : Form, IMainView
    {
        /// <summary>
        /// Left panel containing the controls.
        /// </summary>
        private Panel leftPanel = default!;

        /// <summary>
        /// Flow layout panel for vertical control arrangement.
        /// </summary>
        private FlowLayoutPanel controlPanel = default!;

        /// <summary>
        /// DataGridView table for adjacency matrix input.
        /// </summary>
        private DataGridView edgesGrid = default!;

        /// <summary>
        /// Numeric field for selecting the number of graph vertices.
        /// </summary>
        private NumericUpDown nodeCountSpinner = default!;

        /// <summary>
        /// Dropdown list for pathfinding algorithm selection.
        /// </summary>
        private ComboBox algoCombo = default!, startCombo = default!, targetCombo = default!;

        /// <summary>
        /// Button to start the path search.
        /// </summary>
        private Button findPathBtn = default!, clearBtn = default!;

        /// <summary>
        /// PictureBox element for graph drawing.
        /// </summary>
        private PictureBox canvasBox = default!;

        /// <summary>
        /// Label for displaying the status of operations.
        /// </summary>
        private Label statusLabel = default!;

        /// <summary>
        /// Text field for displaying benchmark results.
        /// </summary>
        private TextBox benchmarkTextBox = default!;

        /// <summary>
        /// Button to export results to a file.
        /// </summary>
        private Button exportBtn = default!;

        /// <summary>
        /// Graph management controller.
        /// </summary>
        private readonly GraphController _controller;

        /// <summary>
        /// Graph visualization service.
        /// </summary>
        private readonly GraphVisualizer _visualizer;

        /// <summary>
        /// Result export service.
        /// </summary>
        private readonly ExportService _exportService;

        /// <summary>
        /// Graph vertex layout service.
        /// </summary>
        private readonly GraphLayoutService _layoutService;

        /// <summary>
        /// Presenter that coordinates interaction between View and Controller.
        /// </summary>
        private GraphPresenter _presenter = default!;

        /// <summary>
        /// Initializes a new main form and its components.
        /// </summary>
        public MainForm()
        {
            _controller = new GraphController();
            _visualizer = new GraphVisualizer();
            _exportService = new ExportService();
            _layoutService = new GraphLayoutService();

            Text = "Graph Pathfinder";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;

            _presenter = new GraphPresenter(this, _controller, _exportService, _layoutService);

            InitializeUI();
            _presenter.BuildGraphFromMatrix();
        }

        /// <summary>
        /// Initializes and configures all user interface elements.
        /// </summary>
        private void InitializeUI()
        {
            leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };

            controlPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            controlPanel.Controls.Add(new Label { Text = "Algorithm:", AutoSize = true, Margin = new Padding(0, 5, 0, 0) });
            algoCombo = new ComboBox { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            algoCombo.Items.AddRange(PathAlgorithmFactory.GetAvailableNames());
            algoCombo.SelectedIndex = 0;
            controlPanel.Controls.Add(algoCombo);

            controlPanel.Controls.Add(new Label { Text = "Number of vertices:", AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            nodeCountSpinner = new NumericUpDown { Minimum = 2, Maximum = 20, Value = 5, Width = 250 };
            nodeCountSpinner.ValueChanged += (s, e) => { SetupMatrixStructure(); GraphChanged?.Invoke(this, EventArgs.Empty); };
            controlPanel.Controls.Add(nodeCountSpinner);

            controlPanel.Controls.Add(new Label { Text = "Adjacency matrix (weights):", AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            controlPanel.Controls.Add(new Label { Text = "Row = FROM, Column = TO", AutoSize = true, Font = new Font("Segoe UI", 7), ForeColor = Color.Gray });
            edgesGrid = new DataGridView
            {
                Width = 250,
                Height = 220,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ScrollBars = ScrollBars.Both,
                RowHeadersWidth = 50,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false
            };
            edgesGrid.CellEndEdit += (s, e) => GraphChanged?.Invoke(this, EventArgs.Empty);
            edgesGrid.DataError += (s, e) => { e.Cancel = true; };
            controlPanel.Controls.Add(edgesGrid);

            SetupMatrixStructure();

            controlPanel.Controls.Add(new Label { Text = "Start | Finish:", AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            startCombo = new ComboBox { Width = 115, DropDownStyle = ComboBoxStyle.DropDownList };
            targetCombo = new ComboBox { Width = 115, DropDownStyle = ComboBoxStyle.DropDownList };
            var pointsPanel = new FlowLayoutPanel { Width = 250, Height = 40, Margin = new Padding(0) };
            pointsPanel.Controls.Add(startCombo);
            pointsPanel.Controls.Add(targetCombo);
            controlPanel.Controls.Add(pointsPanel);

            findPathBtn = new Button { Text = "Find Path", Width = 250, Height = 35, Margin = new Padding(0, 10, 0, 0) };
            findPathBtn.Click += (s, e) => FindPathRequested?.Invoke(this, EventArgs.Empty);
            controlPanel.Controls.Add(findPathBtn);

            clearBtn = new Button { Text = "Clear", Width = 250, Height = 35, Margin = new Padding(0, 5, 0, 0) };
            clearBtn.Click += (s, e) => ClearRequested?.Invoke(this, EventArgs.Empty);
            controlPanel.Controls.Add(clearBtn);

            exportBtn = new Button { Text = "Export Results", Width = 250, Height = 35, Margin = new Padding(0, 5, 0, 0) };
            exportBtn.Click += (s, e) => ExportRequested?.Invoke(this, EventArgs.Empty);
            controlPanel.Controls.Add(exportBtn);

            statusLabel = new Label { Text = "Status: Ready", AutoSize = true, Margin = new Padding(0, 20, 0, 0) };
            controlPanel.Controls.Add(statusLabel);

            controlPanel.Controls.Add(new Label { Text = "Benchmark results:", AutoSize = true, Margin = new Padding(0, 15, 0, 0) });
            benchmarkTextBox = new TextBox
            {
                Width = 250,
                Height = 80,
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.WhiteSmoke
            };
            controlPanel.Controls.Add(benchmarkTextBox);

            leftPanel.Controls.Add(controlPanel);

            canvasBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            canvasBox.Paint += CanvasBox_Paint;

            Controls.Add(canvasBox);
            Controls.Add(leftPanel);
            canvasBox.BringToFront();
        }

        /// <summary>
        /// Configures the adjacency matrix structure (columns and diagonal).
        /// Called when the number of vertices changes.
        /// </summary>
        private void SetupMatrixStructure()
        {
            int count = (int)nodeCountSpinner.Value;

            edgesGrid.Columns.Clear();
            edgesGrid.Rows.Clear();

            for (int i = 0; i < count; i++)
            {
                edgesGrid.Columns.Add($"Node{i}", $"{i}");
                edgesGrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                edgesGrid.Columns[i].Width = 40;
            }

            edgesGrid.Rows.Add(count);
            for (int i = 0; i < count; i++)
            {
                edgesGrid.Rows[i].HeaderCell.Value = $"{i}";
                var diagonalCell = edgesGrid.Rows[i].Cells[i];
                diagonalCell.ReadOnly = true;
                diagonalCell.Style.BackColor = Color.LightGray;
                diagonalCell.Style.ForeColor = Color.DarkGray;
                diagonalCell.Value = "-";
            }
        }

        /// <summary>
        /// Paint event handler for the canvas. Invokes graph visualization.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Paint event arguments.</param>
        private void CanvasBox_Paint(object? sender, PaintEventArgs e)
        {
            try
            {
                _visualizer.Visualize(_controller.CurrentGraph, e.Graphics, _presenter.LastPathResult);
            }
            catch (InvalidOperationException ex)
            {
                DrawVisualizationError(e.Graphics, ex.Message);
            }
            catch (Exception)
            {
                DrawVisualizationError(e.Graphics, "Graph visualization error");
            }
        }

        /// <summary>
        /// Draws a visualization error message on the canvas.
        /// </summary>
        /// <param name="g">Graphics context for drawing.</param>
        /// <param name="message">Error message text.</param>
        private void DrawVisualizationError(Graphics g, string message)
        {
            g.Clear(Color.White);

            using Font font = new Font("Arial", 12, FontStyle.Bold);
            using StringFormat format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            RectangleF bounds = new RectangleF(0, 0, canvasBox.Width, canvasBox.Height);
            g.DrawRectangle(Pens.Red, 50, 50, canvasBox.Width - 100, canvasBox.Height - 100);

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, Color.Red)))
            {
                g.FillRectangle(brush, 50, 50, canvasBox.Width - 100, canvasBox.Height - 100);
            }

            g.DrawString(message, font, Brushes.DarkRed, bounds, format);
        }

        /// <summary>
        /// Number of graph vertices.
        /// </summary>
        public int NodeCount
        {
            get => (int)nodeCountSpinner.Value;
            set => nodeCountSpinner.Value = value;
        }

        /// <summary>
        /// Index of the selected algorithm in the list.
        /// </summary>
        public int SelectedAlgorithmIndex => algoCombo.SelectedIndex;

        /// <summary>
        /// Name of the selected algorithm.
        /// </summary>
        public string SelectedAlgorithmName => algoCombo.SelectedItem?.ToString() ?? "";

        /// <summary>
        /// Index of the start vertex.
        /// </summary>
        public int StartNodeIndex
        {
            get
            {
                if (startCombo.SelectedItem == null) return -1;
                return int.TryParse(startCombo.SelectedItem.ToString(), out int id) ? id : -1;
            }
        }

        /// <summary>
        /// Index of the target vertex.
        /// </summary>
        public int TargetNodeIndex
        {
            get
            {
                if (targetCombo.SelectedItem == null) return -1;
                return int.TryParse(targetCombo.SelectedItem.ToString(), out int id) ? id : -1;
            }
        }

        /// <summary>
        /// Width of the canvas for graph drawing.
        /// </summary>
        public int CanvasWidth => canvasBox.Width;

        /// <summary>
        /// Height of the canvas for graph drawing.
        /// </summary>
        public int CanvasHeight => canvasBox.Height;

        /// <summary>
        /// Gets the adjacency matrix cell value by row and column indices.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <returns>Cell value or <c>null</c> if the cell is unavailable.</returns>
        public string? GetMatrixCellValue(int row, int col)
        {
            if (row < 0 || row >= edgesGrid.RowCount || col < 0 || col >= edgesGrid.ColumnCount)
                return null;
            return edgesGrid.Rows[row].Cells[col].Value?.ToString();
        }

        /// <summary>
        /// Sets the adjacency matrix cell value.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <param name="value">Value to set.</param>
        public void SetMatrixCellValue(int row, int col, string value)
        {
            if (row < 0 || row >= edgesGrid.RowCount || col < 0 || col >= edgesGrid.ColumnCount)
                return;
            
            edgesGrid.Rows[row].Cells[col].Value = value;
        }

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="message">Message text.</param>
        public void ShowError(string message)
        {
            UpdateStatus(message);
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Updates the status text in the UI.
        /// </summary>
        /// <param name="message">New status text.</param>
        public void UpdateStatus(string message)
        {
            statusLabel.Text = $"Status: {message}";
            controlPanel.ScrollControlIntoView(statusLabel);
        }

        /// <summary>
        /// Sets the benchmark results text.
        /// </summary>
        /// <param name="text">Results text.</param>
        public void SetBenchmarkResults(string text)
        {
            benchmarkTextBox.Text = text;
        }

        /// <summary>
        /// Requests a graph redraw.
        /// </summary>
        public void RequestGraphRedraw()
        {
            canvasBox.Invalidate();
        }

        /// <summary>
        /// Updates the vertex dropdown lists.
        /// </summary>
        /// <param name="nodeIds">Array of vertex identifiers.</param>
        public void UpdateNodeComboboxes(string[] nodeIds)
        {
            string? prevStart = startCombo.SelectedItem?.ToString();
            string? prevTarget = targetCombo.SelectedItem?.ToString();

            startCombo.Items.Clear();
            targetCombo.Items.Clear();
            startCombo.Items.AddRange(nodeIds);
            targetCombo.Items.AddRange(nodeIds);

            if (nodeIds.Length > 0)
            {
                startCombo.SelectedItem = prevStart != null && nodeIds.Contains(prevStart) ? prevStart : nodeIds[0];
                targetCombo.SelectedItem = prevTarget != null && nodeIds.Contains(prevTarget) ? prevTarget : nodeIds[^1];
            }
        }

        /// <summary>
        /// Path finding request event.
        /// </summary>
        public event EventHandler? FindPathRequested;

        /// <summary>
        /// Graph change event.
        /// </summary>
        public event EventHandler? GraphChanged;

        /// <summary>
        /// Export request event.
        /// </summary>
        public event EventHandler? ExportRequested;

        /// <summary>
        /// Graph clear request event.
        /// </summary>
        public event EventHandler? ClearRequested;
    }
}