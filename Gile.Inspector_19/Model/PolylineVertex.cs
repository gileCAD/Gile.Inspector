using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a Polyline vertex.
    /// </summary>
    public class PolylineVertex
    {
        /// <summary>
        /// Gets the index of the vertex.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the SegmentType of the vertex.
        /// </summary>
        public SegmentType SegmentType { get; }

        /// <summary>
        /// Gets the Point2d of the vertex.
        /// </summary>
        public Point2d Point2d { get; }

        /// <summary>
        /// Gets the Point3d of the vertex.
        /// </summary>
        public Point3d Point3d { get; }

        /// <summary>
        /// Gets the Bulge of the vertex.
        /// </summary>
        public double Bulge { get; }

        /// <summary>
        /// Gets the StartWidth of the vertex.
        /// </summary>
        public double StartWidth { get; }

        /// <summary>
        /// Gets the EndWidth of the vertex.
        /// </summary>
        public double EndWidth { get; }

        /// <summary>
        /// Gets the Curve3d of the vertex.
        /// </summary>
        public Curve3d Curve {get;}

        /// <summary>
        /// Creates a new instance of PolylineVertex.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="segmentType">Segment type.</param>
        /// <param name="pt2d">Point 2d</param>
        /// <param name="pt3d">Point 3d.</param>
        /// <param name="bulge">Bulge.</param>
        /// <param name="startWidth">Start width.</param>
        /// <param name="endWidth">End width</param>
        /// <param name="curve">Curve 3d.</param>
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
