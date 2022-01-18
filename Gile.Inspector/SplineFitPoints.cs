using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Gile.AutoCAD.Inspector
{
    public class SplineFitPoints
    {
        public Point3dCollection FitPoints;

        public SplineFitPoints(Spline spline)
        {
            FitPoints = new Point3dCollection();
            for (int i = 0; i < spline.NumFitPoints; i++)
            {
                FitPoints.Add(spline.GetFitPointAt(i)); 
            }
        }
    }
}