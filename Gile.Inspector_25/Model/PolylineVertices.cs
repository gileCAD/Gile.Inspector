using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using System.Collections.Generic;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Describes a collection of PolylineVertex
    /// </summary>
    public class PolylineVertices
    {
        /// <summary>
        /// Gets the vertices collection.
        /// </summary>
        public List<PolylineVertex> Vertices { get; }

        /// <summary>
        /// Creates a new instance of PolylineVertices.
        /// </summary>
        /// <param name="pline">Polyline instance.</param>
        public PolylineVertices(Polyline pline)
        {
            Vertices = [];
            for (int i = 0; i < pline.NumberOfVertices; i++)
            {
                Curve3d? curve = null;
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
