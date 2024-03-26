using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Describes the columns of a DataTable.
    /// </summary>
    public class DataRowCollection
    {
        /// <summary>
        /// Gets the columns of the DataTable
        /// </summary>
        public DataRow[] Rows { get; }

        /// <summary>
        /// Creates a new intance of DataColumnCollection.
        /// </summary>
        /// <param name="table">DataTable instance.</param>
        public DataRowCollection(DataTable table)
        {
            Rows = new DataRow[table.NumRows];
            for (int i = 0; i < table.NumRows; i++)
            {
                var cells = new DataCell[table.NumColumns];
                for (int j = 0; j < table.NumColumns; j++)
                {
                    cells[j] = table.GetCellAt(i, j);
                }
                Rows[i] = new DataRow(new DataCellCollection(cells));
            }
        }
    }
}