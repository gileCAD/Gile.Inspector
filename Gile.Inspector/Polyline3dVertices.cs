using Autodesk.AutoCAD.DatabaseServices;

using System.Linq;

namespace Gile.AutoCAD.Inspector
{
    public class Polyline3dVertices
    {
        public ObjectIdCollection Vertices { get; }

        public Polyline3dVertices(Polyline3d pline)
        {
            Vertices = new ObjectIdCollection(pline.Cast<ObjectId>().ToArray());
        }
    }
}
