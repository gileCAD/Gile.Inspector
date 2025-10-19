using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a collection of PolygonMesh vertices
    /// </summary
    public class PolygonMeshVertices : ComplexEntityVertices<PolygonMesh, PolygonMeshVertex>
    {
        /// <summary>
        /// Creates a new instance of PolygonMeshVertices.
        /// </summary>
        /// <param name="mesh">PolygonMesh instance.</param>
        public PolygonMeshVertices(PolygonMesh mesh) : base(mesh) { }
    }
}
