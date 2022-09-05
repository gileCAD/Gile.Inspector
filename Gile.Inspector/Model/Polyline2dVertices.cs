using Autodesk.AutoCAD.DatabaseServices;

using System.Linq;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Describes a collection of Polyline2d vertices
    /// </summary>
    public class Polyline2dVertices
    {
        /// <summary>
        /// Gets the vertices collection.
        /// </summary>
        public ObjectIdCollection Vertices { get; }

        /// <summary>
        /// Creates a new instance of Polyline2dVertices.
        /// </summary>
        /// <param name="pline">Polyline2d instance.</param>
        public Polyline2dVertices(Polyline2d pline)
        {
            Vertices = new ObjectIdCollection();
            foreach (ObjectId id in pline)
            {
                Vertices.Add(id);
            }
        }
    }
}
