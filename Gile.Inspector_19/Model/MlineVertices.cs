using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a collection of Mline vertices
    /// </summary>
    public class MlineVertices
    {
        /// <summary>
        /// Gets the vertices collection.
        /// </summary>
        public Point3dCollection Vertices { get; }

        /// <summary>
        /// Creates a new instance of MlineVertices.
        /// </summary>
        /// <param name="mline">Mline instance.</param>
        public MlineVertices(Mline mline)
        {
            Vertices = new Point3dCollection();
            for (int i = 0; i < mline.NumberOfVertices; i++)
            {
                Vertices.Add(mline.VertexAt(i));
            }
        }
    }
}
