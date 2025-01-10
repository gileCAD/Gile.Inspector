using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes a collection of viewports.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of ViewportCollection.
    /// </remarks>
    /// <param name="layout">Instance of Layout.</param>
    internal class ViewportCollection(Layout layout)
    {
        /// <summary>
        /// Gets the viewports for the current Layout.
        /// </summary>
        public ObjectIdCollection Viewports { get; } = layout.GetViewports();
    }
}
