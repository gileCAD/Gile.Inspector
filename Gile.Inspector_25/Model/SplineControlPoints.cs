using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Describes a collection of Spline control points
    /// </summary>
    internal class SplineControlPoints
    {
        /// <summary>
        /// Gets the control point collection.
        /// </summary>
        public Point3dCollection ControlPoints;

        /// <summary>
        /// Creates a new instance of SplineControlPoints.
        /// </summary>
        /// <param name="spline">Instance of Spline.</param>
        public SplineControlPoints(Spline spline)
        {
            ControlPoints = [];
            for (int i = 0; i < spline.NumControlPoints; i++)
            {
                ControlPoints.Add(spline.GetControlPointAt(i));
            }
        }
    }
}