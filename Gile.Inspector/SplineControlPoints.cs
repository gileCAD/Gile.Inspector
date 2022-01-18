using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Gile.AutoCAD.Inspector
{
    internal class SplineControlPoints
    {
        public Point3dCollection ControlPoints;

        public SplineControlPoints(Spline spline)
        {
            ControlPoints = new Point3dCollection();
            for (int i = 0; i < spline.NumControlPoints; i++)
            {
                ControlPoints.Add(spline.GetControlPointAt(i));
            }
        }
    }
}