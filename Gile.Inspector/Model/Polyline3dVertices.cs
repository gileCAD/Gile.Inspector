using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Describes a collection of Polyline3d vertices
    /// </summary>
    public class Polyline3dVertices
    {
        /// <summary>
        /// Gets the vertices collection.
        /// </summary>
        public ObjectIdCollection Vertices { get; }

        /// <summary>
        /// Creates a new instance of Polyline3dVertices.
        /// </summary>
        /// <param name="pline">Polyline3d instance.</param>
        public Polyline3dVertices(Polyline3d pline)
        {
            Vertices = new ObjectIdCollection();
            foreach (ObjectId id in pline)
            {
                Vertices.Add(id);
            }
        }
    }
}
