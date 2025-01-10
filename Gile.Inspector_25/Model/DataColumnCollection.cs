using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes the columns of a DataTable.
    /// </summary>
    public class DataColumnCollection
    {
        /// <summary>
        /// Gets the columns of the DataTable
        /// </summary>
        public DataColumn[] Columns { get; }

        /// <summary>
        /// Creates a new intance of DataColumnCollection.
        /// </summary>
        /// <param name="table">DataTable instance.</param>
        public DataColumnCollection(DataTable table)
        {
            Columns = new DataColumn[table.NumColumns];
            for (int i = 0; i < table.NumColumns; i++)
            {
                Columns[i] = table.GetColumnAt(i);
            }
        }
    }
}