using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes a collection of PolyFaceMesh vertices
    /// </summary>
    /// <param name="mesh">PolyFaceMesh instance.</param>
    public class PolyFaceMeshVertices(PolyFaceMesh mesh) : ComplexEntityVertices<PolyFaceMesh, PolyFaceMeshVertex>(mesh)
    {
    }
}
