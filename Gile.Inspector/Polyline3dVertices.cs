using Autodesk.AutoCAD.DatabaseServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
