using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a Datatable cell collection (row or column);
    /// </summary>
    public class DataCellCollection
    {
        /// <summary>
        /// Gets the cells.
        /// </summary>
        public DataCell[] Cells { get; }

        /// <summary>
        /// Creates a new instance of DataCellCollection.
        /// </summary>
        /// <param name="column">DataColum instance</param>
        public DataCellCollection(DataColumn column)
        {
            Cells = new DataCell[column.NumCells];
            for (int i = 0; i < column.NumCells; i++)
            {
                Cells[i] = column.GetCellAt(i);
            }
        }

        /// <summary>
        /// Creates a new instance of DataCellCollection.
        /// </summary>
        /// <param name="cells">Array of DataCell.</param>
        public DataCellCollection(DataCell[] cells)
        {
            Cells = cells;
        }
    }
}
