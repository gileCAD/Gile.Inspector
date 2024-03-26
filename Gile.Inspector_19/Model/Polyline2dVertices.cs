using Autodesk.AutoCAD.DatabaseServices;

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
        public DBObjectCollection Vertices { get; }

        /// <summary>
        /// Creates a new instance of Polyline2dVertices.
        /// </summary>
        /// <param name="pline">Polyline2d instance.</param>
        public Polyline2dVertices(Polyline2d pline)
        {
            Vertices = new DBObjectCollection();
            foreach (var obj in pline)
            {
                if (obj is ObjectId id)
                    Vertices.Add((Vertex2d)id.GetObject(OpenMode.ForRead));
                else
                    Vertices.Add((Vertex2d)obj);
            }
        }
    }
}
