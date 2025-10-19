using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes a collection of PolygonMesh vertices
    /// </summary>
    /// <param name="mesh">PolygonMesh instance.</param>
    public class PolygonMeshVertices(PolygonMesh mesh) : ComplexEntityVertices<PolygonMesh, PolygonMeshVertex>(mesh)
    {
    }
}
