namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Describes a row of DataTable.
    /// </summary>
    /// <remarks>
    /// Create a new instance of DataRow.
    /// </remarks>
    /// <param name="cells">Instance of DataCellCollection.</param>
    public class DataRow(DataCellCollection cells)
    {
        /// <summary>
        /// Gets the cells of the row.
        /// </summary>
        public DataCellCollection Cells { get; } = cells;
    }
}