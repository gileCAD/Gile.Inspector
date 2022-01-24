using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Gile.AutoCAD.Inspector
{
    public class MlineVertices
    {
        public Point3dCollection Vertices { get; }

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
