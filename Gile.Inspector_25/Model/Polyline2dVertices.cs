using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes a collection of Polyline2d vertices
    /// </summary>
    /// <param name="pline">Polyline2d instance.</param>
    public class Polyline2dVertices(Polyline2d pline) : ComplexEntityVertices<Polyline2d, Vertex2d>(pline)
    {
    }
}
