using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes a Polyline vertex.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of PolylineVertex.
    /// </remarks>
    /// <param name="index">Index.</param>
    /// <param name="segmentType">Segment type.</param>
    /// <param name="pt2d">Point 2d</param>
    /// <param name="pt3d">Point 3d.</param>
    /// <param name="bulge">Bulge.</param>
    /// <param name="startWidth">Start width.</param>
    /// <param name="endWidth">End width</param>
    /// <param name="curve">Curve 3d.</param>
    public class PolylineVertex(int index, SegmentType segmentType, Point2d pt2d, Point3d pt3d, double bulge, double startWidth, double endWidth, Curve3d? curve)
    {
        /// <summary>
        /// Gets the index of the vertex.
        /// </summary>
        public int Index { get; } = index;

        /// <summary>
        /// Gets the SegmentType of the vertex.
        /// </summary>
        public SegmentType SegmentType { get; } = segmentType;

        /// <summary>
        /// Gets the Point2d of the vertex.
        /// </summary>
        public Point2d Point2d { get; } = pt2d;

        /// <summary>
        /// Gets the Point3d of the vertex.
        /// </summary>
        public Point3d Point3d { get; } = pt3d;

        /// <summary>
        /// Gets the Bulge of the vertex.
        /// </summary>
        public double Bulge { get; } = bulge;

        /// <summary>
        /// Gets the StartWidth of the vertex.
        /// </summary>
        public double StartWidth { get; } = startWidth;

        /// <summary>
        /// Gets the EndWidth of the vertex.
        /// </summary>
        public double EndWidth { get; } = endWidth;

        /// <summary>
        /// Gets the Curve3d of the vertex.
        /// </summary>
        public Curve3d? Curve { get; } = curve;
    }
}
