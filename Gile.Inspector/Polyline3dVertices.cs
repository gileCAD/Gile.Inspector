using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.Inspector
{
    public class Polyline3dVertices
    {
        public ObjectIdCollection Vertices { get; }

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
