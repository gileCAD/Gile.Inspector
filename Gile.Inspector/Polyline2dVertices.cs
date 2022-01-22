using Autodesk.AutoCAD.DatabaseServices;

using System.Linq;

namespace Gile.AutoCAD.Inspector
{
    public class Polyline2dVertices
    {
        public ObjectIdCollection Vertices { get; }

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
