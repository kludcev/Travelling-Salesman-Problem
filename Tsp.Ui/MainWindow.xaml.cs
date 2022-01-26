using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using TravelingSalesmanProblem.Models;
using Tsp.Data;


namespace Tsp.Ui
{
    // TODO implement proper MVC binding and validation
    // TODO make MapPointsDataGrid editable
    public partial class MainWindow
    {
        private const double DefaultTemperature = 10;
        private const double DefaultCoolingRate = 0.9999;
        private const double DefaultAbsoluteTemperature = 0.00001;

        private readonly List<Point> _points = new List<Point>();
        private readonly IUiElementsBuilder _uiElementsBuilder = new UiElementsBuilder();
        private readonly IAdjacencyMatrixProvider _adjacencyMatrixProvider = new AdjacencyMatrixProvider();

        private InputDataType _inputDataType = InputDataType.Map;

        public MainWindow()
        {
            InitializeComponent();

            TemperatureTextBox.Text = DefaultTemperature.ToString(CultureInfo.InvariantCulture);
            CoolingRateTextBox.Text = DefaultCoolingRate.ToString("F04");
            AbsoluteTemperatureTextBox.Text = DefaultAbsoluteTemperature.ToString("F05");
        }

        public void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var positionPoint = e.GetPosition(Canvas);
            var ellipseCount = Canvas.CountElements(UiElementsBuilder.EllipseUid);
            _points.Add(positionPoint);

            var ellipse = _uiElementsBuilder.GetEllipse();
            Canvas.SetElement(ellipse, positionPoint);

            var pointNumber = _uiElementsBuilder.GetTextBlock(ellipseCount.ToString());
            var pointNumberLocation = new Point(positionPoint.X + 6, positionPoint.Y + 5);
            Canvas.SetElement(pointNumber, pointNumberLocation);

            MapPointsDataGrid.Items.Add(new { Number = ellipseCount, positionPoint.X, positionPoint.Y });
        }

        public void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawCanvasLayout();

            MapPointsDataGrid.Items.Clear();
            MapPointsDataGrid.Items.Refresh();

            _points.Clear();
        }

        public void RunAlgorithmButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(TemperatureTextBox.Text, out var temp) ||
                    !double.TryParse(CoolingRateTextBox.Text, out var cooling) ||
                    !double.TryParse(AbsoluteTemperatureTextBox.Text, out var absolute))
                {
                    throw new ApplicationException("One of the algorithm settings can't be parsed.");
                }

                var matrix = _inputDataType == InputDataType.Map
                    ? _adjacencyMatrixProvider.GetAdjacencyMatrix(_points.Select(p => new Point2d(p.X, p.Y)).ToArray())
                    : _inputDataType == InputDataType.AdjacencyMatrix
                        ? _adjacencyMatrixProvider.GetAdjacencyMatrix(AdjacencyMatrixDataGrid.ToDataTable())
                        : throw new InvalidOperationException(nameof(_inputDataType));

                if (matrix.LongLength == 0)
                    throw new ApplicationException("Adjacency matrix is not defined.");

                var sw = new Stopwatch();
                sw.Start();

                var simulatedAnnealing = new SimulatedAnnealing(matrix);
                var annealingResults = simulatedAnnealing.Anneal(temp, cooling, absolute);

                ResultsTextBlock.Text = "";
                ResultsTextBlock.Inlines.Add(new Bold(new Run("Optimal Path: ")));
                ResultsTextBlock.Inlines.Add(annealingResults.CitiesOrder.Aggregate("", (current, t) => $"{current}{t} -> ") + annealingResults.CitiesOrder.First() + "\n");
                ResultsTextBlock.Inlines.Add(new Bold(new Run("Calculation Time: ")));
                ResultsTextBlock.Inlines.Add(sw.ElapsedMilliseconds + "\n");
                ResultsTextBlock.Inlines.Add(new Bold(new Run("Path Length: ")));
                ResultsTextBlock.Inlines.Add(annealingResults.ShortestDistance.ToString(CultureInfo.InvariantCulture) + "\n");
                ResultsTextBlock.Inlines.Add(new Bold(new Run("Iteration Count: ")));
                ResultsTextBlock.Inlines.Add(annealingResults.IterationCount + "\n");

                if (_inputDataType == InputDataType.Map)
                    DrawOptimalPathOnCanvas(annealingResults.CitiesOrder);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ClearInputButton_OnClick(object sender, RoutedEventArgs e)
        {
            DrawCanvasLayout();

            MapPointsDataGrid.Items.Clear();
            MapPointsDataGrid.Items.Refresh();

            _points.Clear();

            AdjacencyMatrixDataGrid.Items.Clear();
            AdjacencyMatrixDataGrid.Items.Refresh();
        }

        private void InputDataTypeMapRadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            _inputDataType = InputDataType.Map;

            MapAndPointsGrid.Visibility = Visibility.Visible;
            AdjacencyMatrixPanel.Visibility = Visibility.Collapsed;
            AdjacencyMatrixDataGrid.Visibility = Visibility.Hidden;
        }

        private void InputDataTypeAdjacencyMatrixRadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            _inputDataType = InputDataType.AdjacencyMatrix;

            MapAndPointsGrid.Visibility = Visibility.Hidden;
            AdjacencyMatrixPanel.Visibility = Visibility.Visible;
            AdjacencyMatrixDataGrid.Visibility = Visibility.Visible;
        }

        public void AdjacencyMatrixDataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1 + " city";
        }

        public void CreateAdjacencyMatrixButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(AdjacencyMatrixSizeTextBox.Text, out var size))
                throw new ArgumentException("Size can't be parsed.");

            for (var i = 1; i <= size; i++)
            {
                AdjacencyMatrixDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"{i} city",
                    Binding = new Binding($"[{i - 1}]"),
                });
            }

            var rows = new List<string[]>();
            var values = new string[size];

            for (var i = 0; i < size; i++)
                values[i] = "";

            for (var i = 0; i < size; i++)
                rows.Add(values);

            AdjacencyMatrixDataGrid.ItemsSource = rows;

            for (var i = size; i < AdjacencyMatrixDataGrid.Columns.Count; i++)
                AdjacencyMatrixDataGrid.Columns[i].Visibility = Visibility.Collapsed;
        }

        private void DrawCanvasLayout()
        {
            foreach (var userElementUid in UiElementsBuilder.UserElementUids)
                Canvas.RemoveElementsById(userElementUid);

            var height = (int)Math.Truncate(Canvas.ActualHeight / 100);
            var width = (int)Math.Truncate(Canvas.ActualWidth / 100);

            var gridElements = new List<UIElement>();

            for (var i = 0; i <= height; i++)
                gridElements.Add(_uiElementsBuilder.GetLine(new Point(0, i * 100), new Point(Canvas.ActualWidth, i * 100)));

            for (var i = 0; i <= width; i++)
                gridElements.Add(_uiElementsBuilder.GetLine(new Point(i * 100, Canvas.ActualHeight), new Point(i * 100, 0)));

            foreach (var gridElement in gridElements)
            {
                Canvas.Children.Add(gridElement);
            }

            var canvasScaleTextBlock = _uiElementsBuilder.GetTextBlock("(100;100)");
            Canvas.SetElement(canvasScaleTextBlock, new Point(100, 100));
        }

        private void DrawOptimalPathOnCanvas(int[] pointsOrder)
        {
            // draw arrows
            for (var i = 0; i < pointsOrder.Length - 1; i++)
            {
                var arrow = _uiElementsBuilder.GetLine(_points[pointsOrder[i]], _points[pointsOrder[i + 1]]);
                Canvas.Children.Add(arrow);
            }

            // from last to first
            Canvas.Children.Add(_uiElementsBuilder.GetLine(_points[pointsOrder.Last()], _points[pointsOrder.First()]));
        }
    }
}
