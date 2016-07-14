using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace Traveling_Salesman_Problem__TSP__WPF
{
    public partial class MainWindow : Window // first tab - GUI version
    {
        private List<Point> points = new List<Point>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) // mouse click on canvas
        {
            var p = e.GetPosition(this.Canvas);
            points.Add(p);
            CanvasDrawing.DrawEllipse(p,this.Canvas);
           int count = CanvasDrawing.CountUiElements("Ellipse", this.Canvas);
           CanvasDrawing.DrawNumber(p, count.ToString(),this.Canvas);
            DataGridGUI.Items.Add(new Point3D {Id = count,X=p.X, Y = p.Y});
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasDrawing.DrawCanvasLayout(this.Canvas);
            DataGridGUI.Items.Clear();
            DataGridGUI.Items.Refresh();
            points.Clear();
        }
        private void ButtonRunAlgorithmGui_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                double temp, delta, cooling, absolute;

                if (!(double.TryParse(TextBoxTemperatureGUI.Text, out temp)) || !(double.TryParse(TextBoxDeltaDGUI.Text, out delta))
                    || !(double.TryParse(TextBoxCoolingDGUI.Text, out cooling)) || !(double.TryParse(TextBoxAbsoluteTDGUI.Text, out absolute)))
                { throw new ArgumentException("One of the algorithm settings can't be parsed."); }

                double[,] matrix = Adjacency_matrix(points);
                if (matrix.LongLength==0) { throw new ArgumentNullException("Adjacency matrix = null");}

                var sw = new Stopwatch();
                sw.Start();

                var SA = new SimulatedAnnealing(matrix);
                int iterations = SA.Anneal(temp, delta, cooling, absolute);
                
                TextBoxCalculationTimeGui.Text = sw.ElapsedMilliseconds.ToString();

                var path = SA.CitiesOrder.Aggregate("", (current, t) => current + (t+1 + " -> "));

                TextBoxOptimalPathGui.Text = path;

                TextBoxPathLengthGui.Text = SA.ShortestDistance.ToString();
                TextBoxIterationsAmountGui.Text = iterations.ToString();

                //draw arrows
                for (int i = 0; i < SA.CitiesOrder.Count-1; i++)
                {
                    var p1 = points[SA.CitiesOrder[i]];
                    var p2 = points[SA.CitiesOrder[i + 1]];
                    CanvasDrawing.DrawLinkArrow(p1,p2,this.Canvas);
                   }
                //from last to first
               CanvasDrawing.DrawLinkArrow(points[SA.CitiesOrder.Count-1],points[SA.CitiesOrder[0]],this.Canvas );

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private double[,] Adjacency_matrix(List<Point> Cities)
        {
            double[,] matrix = new double[,] {};
            try
            {
                if (Cities.Count < 4) { throw new ArgumentException("Number of cities shouldn't be less than 4");}

                int size = Cities.Count;
                matrix = new double[size, size];
                for (int i = 0; i <= Cities.Count - 1; i++)
                {
                    for (int j = 0; j <= Cities.Count - 1; j++)
                    {
                        if (i == j)
                            matrix[i, j] = 0;
                        else
                        {
                            double distance = Math.Sqrt((Cities[j].X - Cities[i].X) * (Cities[j].X - Cities[i].X) +
                                              (Cities[j].Y - Cities[i].Y) * (Cities[j].Y - Cities[i].Y));
                            matrix[i, j] = distance;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          return matrix;
        }
        private void ButtonClearCanvasAndDgGui_OnClick(object sender, RoutedEventArgs e)
        {
           CanvasDrawing.DrawCanvasLayout(this.Canvas);
            DataGridGUI.Items.Clear();
            DataGridGUI.Items.Refresh();
            points.Clear();
        }
        private void ButtonClearResultsGui_OnClick(object sender, RoutedEventArgs e)
        {
            TextBoxCalculationTimeGui.Clear();
            TextBoxIterationsAmountGui.Clear();
            TextBoxOptimalPathGui.Clear();
            TextBoxPathLengthGui.Clear();
        }
    }

    public partial class MainWindow : Window // second tab - text version
    {
        private void BtnStartAlgorithm_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                double temp, delta, cooling, absolute;

                if (!(double.TryParse(TextBoxTemperature.Text, out temp)) || !(double.TryParse(TextBoxDeltaD.Text, out delta))
                    || !(double.TryParse(TextBoxCoolingR.Text, out cooling)) || !(double.TryParse(TextBoxAbsoluteT.Text, out absolute)))
                { throw new ArgumentException("One of the algorithm settings can't be parsed."); }

                double[,] matrix = ParseDataGridValues();

                if (matrix.LongLength==0) { throw new ArgumentNullException("Matrix has unparsable cells!");}

                var sw = new Stopwatch();
                sw.Start();

                var SA = new SimulatedAnnealing(matrix);
                int iterations = SA.Anneal(temp, delta, cooling, absolute);

                TextBoxCalculationTime.Text = sw.ElapsedMilliseconds.ToString();

                var path = SA.CitiesOrder.Aggregate("", (current, t) => current + (t + 1 + " -> "));

                TextBoxPath.Text = path;

                TextBoxPathLength.Text = SA.ShortestDistance.ToString();
                TextBoxIterationsAmount.Text = iterations.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        private void DataGridTextVersion_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            string rowHeader = (e.Row.GetIndex() + 1) + " city";
            e.Row.Header = rowHeader;
        }

        public static DataTable DataGridtoDataTable(DataGrid dg)
        {
            dg.SelectAllCells();
            dg.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dg);
            dg.UnselectAllCells();
            String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            string[] Lines = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            string[] Fields;
            Fields = Lines[0].Split(new char[] { ',' });
            int Cols = Fields.GetLength(0);

            DataTable dt = new DataTable();
            for (int i = 0; i < Cols; i++)
                dt.Columns.Add(Fields[i].ToUpper(), typeof(string));
            DataRow Row;
            for (int i = 1; i < Lines.GetLength(0) - 1; i++)
            {
                Fields = Lines[i].Split(new char[] { ',' });
                Row = dt.NewRow();
                for (int f = 0; f < Cols; f++)
                {
                    Row[f] = Fields[f];
                }
                dt.Rows.Add(Row);
            }
            return dt;

        }

        private double[,] ParseDataGridValues()
        {
            double[,] data = new double[,] { };
            try
            {
                var dt = DataGridtoDataTable(DataGridTextVersion);
                data = new double[dt.Rows.Count, dt.Rows.Count];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        double cell;
                        if (!double.TryParse((string)dt.Rows[i][j], out cell))
                        {
                            throw new ArgumentException("Error! Value in DataGrid cell [" + i + ", " + j + "] isn't numeric.");
                        }
                        data[i, j] = cell;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return data;
        }

        private void BtnCreateDgv_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int size;
                if (!int.TryParse(txtBoxDgvRow.Text, out size)) { throw new ArgumentException("Size argument should be numeric"); }

                for (int i = 1; i <= size; i++)
                {
                    int z = i - 1;
                    var textColumn = new DataGridTextColumn();
                    textColumn.Header = i + " city";
                    textColumn.Binding = new Binding("[" + z + "]");
                    DataGridTextVersion.Columns.Add(textColumn);

                }
                var rows = new List<string[]>();
                var value = new string[size];

                for (int i = 0; i < size; i++)
                    value[i] = "";

                for (int i = 0; i < size; i++)
                    rows.Add(value);


                DataGridTextVersion.ItemsSource = rows;

                for (int i = size; i < DataGridTextVersion.Columns.Count; i++) 
                    DataGridTextVersion.Columns[i].Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


       
    }
}
