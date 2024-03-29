using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes a collection of Spline fit points
    /// </summary>
    public class SplineFitPoints
    {
        /// <summary>
        /// Gets the fit point collection.
        /// </summary>
        public Point3dCollection FitPoints;

        /// <summary>
        /// Creates a new instance of SplineFitPoints.
        /// </summary>
        /// <param name="spline">Instance of Spline.</param>
        public SplineFitPoints(Spline spline)
        {
            FitPoints = [];
            for (int i = 0; i < spline.NumFitPoints; i++)
            {
                FitPoints.Add(spline.GetFitPointAt(i)); 
            }
        }
    }
}