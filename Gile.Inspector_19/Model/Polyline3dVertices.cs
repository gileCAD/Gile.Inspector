using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a collection of Polyline3d vertices
    /// </summary>
    public class Polyline3dVertices
    {
        /// <summary>
        /// Gets the vertices collection.
        /// </summary>
        public DBObjectCollection Vertices { get; }

        /// <summary>
        /// Creates a new instance of Polyline3dVertices.
        /// </summary>
        /// <param name="pline">Polyline3d instance.</param>
        public Polyline3dVertices(Polyline3d pline)
        {
            Vertices = new DBObjectCollection();
            foreach (var obj in pline)
            {
                if (obj is ObjectId id)
                    Vertices.Add((PolylineVertex3d)id.GetObject(OpenMode.ForRead));
                else
                    Vertices.Add((PolylineVertex3d)obj);
            }
        }
    }
}
