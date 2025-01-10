namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a row of DataTable.
    /// </summary>
    public class DataRow
    {
        /// <summary>
        /// Gets the cells of the row.
        /// </summary>
        public DataCellCollection Cells { get; }

        /// <summary>
        /// Create a new instance of DataRow.
        /// </summary>
        /// <param name="cells">Instance of DataCellCollection.</param>
        public DataRow(DataCellCollection cells)
        {
            Cells = cells;
        }
    }
}
