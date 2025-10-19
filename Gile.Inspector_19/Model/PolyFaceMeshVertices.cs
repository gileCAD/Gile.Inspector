using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a collection of PolyFaceMesh vertices
    /// </summary
    public class PolyFaceMeshVertices : ComplexEntityVertices<PolyFaceMesh, PolyFaceMeshVertex>
    {
        /// <summary>
        /// Creates a new instance of PolyFaceMeshVertices.
        /// </summary>
        /// <param name="mesh">PolyFaceMesh instance.</param>
        public PolyFaceMeshVertices(PolyFaceMesh mesh) : base(mesh) { }
    }
}
