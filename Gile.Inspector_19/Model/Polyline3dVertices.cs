using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a collection of Polyline3d vertices
    /// </summary>
    public class Polyline3dVertices : ComplexEntityVertices<Polyline3d, PolylineVertex3d>
    {
        /// <summary>
        /// Creates a new instance of Polyline3dVertices.
        /// </summary>
        /// <param name="pline">Polyline3d instance.</param>
        public Polyline3dVertices(Polyline3d pline) : base(pline) { }
    }
}
