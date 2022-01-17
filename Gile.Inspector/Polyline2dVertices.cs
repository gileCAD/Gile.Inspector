using Autodesk.AutoCAD.DatabaseServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gile.AutoCAD.Inspector
{
    public class Polyline2dVertices
    {
        public ObjectIdCollection Vertices { get; }

        public Polyline2dVertices(Polyline2d pline)
        {
            Vertices = new ObjectIdCollection(pline.Cast<ObjectId>().ToArray());
        }
    }
}
