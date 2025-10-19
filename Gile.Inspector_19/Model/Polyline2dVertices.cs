using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a collection of Polyline2d vertices
    /// </summary>
    public class Polyline2dVertices : ComplexEntityVertices<Polyline2d, Vertex2d>
    {
        /// <summary>
        /// Creates a new instance of Polyline2dVertices.
        /// </summary>
        /// <param name="pline">Polyline2d instance.</param>
        public Polyline2dVertices(Polyline2d pline) : base(pline) { }
    }
}
