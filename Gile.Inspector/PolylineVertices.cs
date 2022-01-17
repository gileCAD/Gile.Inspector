using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gile.AutoCAD.Inspector
{
    public class PolylineVertices
    {
        public List<PolylineVertex> Vertices { get; }

        public PolylineVertices(Polyline pline)
        {
            Vertices = new List<PolylineVertex>();
            for (int i = 0; i < pline.NumberOfVertices; i++)
            {
                Curve3d curve = null;
                var segmentType = pline.GetSegmentType(i);
                switch (segmentType)
                {
                    case SegmentType.Line:
                        curve = pline.GetLineSegmentAt(i);
                        break;
                    case SegmentType.Arc:
                        curve = pline.GetArcSegmentAt(i);
                        break;
                    default:
                        break;
                }
                Vertices.Add(new PolylineVertex(
                    i,
                    segmentType,
                    pline.GetPoint2dAt(i),
                    pline.GetPoint3dAt(i),
                    pline.GetBulgeAt(i),
                    pline.GetStartWidthAt(i),
                    pline.GetEndWidthAt(i),
                    curve));
            }
        }
    }
}
