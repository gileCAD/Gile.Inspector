using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gile.AutoCAD.Inspector
{
    public class PolylineVertex
    {
        public int Index { get; }

        public SegmentType SegmentType { get; }

        public Point2d Point2d { get; }

        public Point3d Point3d { get; }

        public double Bulge { get; }

        public double StartWidth { get; }

        public double EndWidth { get; }

        public Curve3d Curve {get;}

        public PolylineVertex(int index, SegmentType segmentType, Point2d pt2d, Point3d pt3d, double bulge, double startWidth, double endWidth, Curve3d curve)
        {
            Index = index;
            SegmentType = segmentType;
            Point2d = pt2d;
            Point3d = pt3d;
            Bulge = bulge;
            StartWidth = startWidth;
            EndWidth = endWidth;
            Curve = curve;
        }

    }
}
