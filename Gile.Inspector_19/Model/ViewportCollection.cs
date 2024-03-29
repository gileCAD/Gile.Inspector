using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a collection of viewports.
    /// </summary>
    internal class ViewportCollection
    {
        /// <summary>
        /// Gets the viewports for the current Layout.
        /// </summary>
        public ObjectIdCollection Viewports { get; }

        /// <summary>
        /// Creates a new instance of ViewportCollection.
        /// </summary>
        /// <param name="layout">Instance of Layout.</param>
        public ViewportCollection(Layout layout)
        {
            Viewports = layout.GetViewports();
        }
    }
}
