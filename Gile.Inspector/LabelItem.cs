using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using System.Globalization;

namespace Gile.AutoCAD.Inspector
{
    public abstract class LabelItem
    {
        public object Value { get; }

        public string Label { get; }

        public LabelItem(object value)
        {
            Value = value;
            int luprec = HostApplicationServices.WorkingDatabase.Luprec;
            string format = "0";
            if (0 < luprec)
            {
                format += ".";
                for (int i = 0; i < luprec; i++)
                {
                    format += "0";
                }
            }
            switch (value)
            {
                case ObjectId id:
                    if (id.IsNull)
                        Label = "(Null)";
                    else
                        using (var tr = new OpenCloseTransaction())
                            Label = $"< {tr.GetObject(id, OpenMode.ForRead).GetType().Name} >";
                    break;
                case Database _: Label = "< Database>"; break;
                case ResultBuffer _: Label = "< ResultBuffer >"; break;
                case double d: Label = d.ToString(format); break;
                case Point2d p: Label = p.ToString(format, CultureInfo.CurrentCulture); break;
                case Point3d p: Label = p.ToString(format, CultureInfo.CurrentCulture); break;
                case Vector2d v: Label = v.ToString(format, CultureInfo.CurrentCulture); break;
                case Vector3d v: Label = v.ToString(format, CultureInfo.CurrentCulture); break;
                case Matrix3d _: Label = "< Matrix3d >"; break;
                case Extents3d _: Label = "< Extents3d >"; break;
                case CoordinateSystem3d _: Label = "< CoordinateSystem3d >"; break;
                case AttributeCollection _: Label = "< AttributeCollection >"; break;
                case DynamicBlockReferencePropertyCollection _: Label = "< DynamicBlockReferencePropertyCollection >"; break;
                case DynamicBlockReferenceProperty _: Label = "< DynamicBlockReferenceProperty >"; break;
                case EntityColor _: Label = "< EntityColor >"; break;
                case PolylineVertices _: Label = "Polyline Vertices"; break;
                case PolylineVertex _: Label = "Polyline Vertex"; break;
                case Entity3d c: Label = $"< {c.GetType().Name} >"; break;
                case Polyline3dVertices _: Label = "Polyline3d Vertices"; break;
                case Polyline2dVertices _: Label = "Polyline2d Vertices"; break;
                case FitData _: Label = "< FitData >"; break;
                case NurbsData _: Label = "< NurbsData >"; break;
                case Point3dCollection _: Label = "< Point3dCollection >"; break;
                case DoubleCollection _: Label = "< DoubleCollection >"; break;
                case Spline _: Label = "< Spline >"; break;
                default: Label = value.ToString(); break;
            }
        }
    }
}
