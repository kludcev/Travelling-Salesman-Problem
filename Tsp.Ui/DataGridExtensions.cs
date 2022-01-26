using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Tsp.Ui
{
    public static class DataGridExtensions
    {
        public static DataTable ToDataTable(this DataGrid dataGrid)
        {
            dataGrid.SelectAllCells();
            dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dataGrid);
            dataGrid.UnselectAllCells();

            var result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            var lines = result.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var fields = lines[0].Split(',');
            var columns = fields.GetLength(0);

            var dataTable = new DataTable();

            for (var i = 0; i < columns; i++)
                dataTable.Columns.Add(fields[i].ToUpper(), typeof(string));

            for (var i = 1; i < lines.GetLength(0) - 1; i++)
            {
                fields = lines[i].Split(',');
                var row = dataTable.NewRow();
                for (var f = 0; f < columns; f++)
                {
                    row[f] = fields[f];
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
