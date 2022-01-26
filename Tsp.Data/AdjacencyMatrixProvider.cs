using System;
using System.Data;

namespace Tsp.Data
{
    public interface IAdjacencyMatrixProvider
    {
        double[,] GetAdjacencyMatrix(Point2d[] points);
        double[,] GetAdjacencyMatrix(DataTable dataTable);
    }

    public class AdjacencyMatrixProvider : IAdjacencyMatrixProvider
    {
        public double[,] GetAdjacencyMatrix(Point2d[] points)
        {
            var matrix = new double[points.Length, points.Length];
            for (var i = 0; i < points.Length; i++)
            {
                for (var j = 0; j < points.Length; j++)
                {
                    if (i == j)
                        matrix[i, j] = 0;
                    else
                    {
                        var distance = Math.Sqrt((points[j].X - points[i].X) * (points[j].X - points[i].X) +
                                                 (points[j].Y - points[i].Y) * (points[j].Y - points[i].Y));
                        matrix[i, j] = distance;
                    }
                }
            }

            return matrix;
        }

        public double[,] GetAdjacencyMatrix(DataTable dataTable)
        {
            var matrix = new double[dataTable.Rows.Count, dataTable.Rows.Count];

            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                for (var j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (!double.TryParse((string)dataTable.Rows[i][j], out var cell))
                        throw new ArgumentException("Error! Value in DataGrid cell [" + i + ", " + j + "] isn't numeric.");

                    matrix[i, j] = cell;
                }
            }

            return matrix;
        }
    }
}
