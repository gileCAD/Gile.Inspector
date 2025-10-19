using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes a collection of Polyline3d vertices
    /// </summary>
    /// <param name="pline">Polyline3d instance.</param>
    public class Polyline3dVertices(Polyline3d pline) : ComplexEntityVertices<Polyline3d, PolylineVertex3d>(pline)
    {
    }
}
