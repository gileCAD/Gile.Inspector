using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.LayerManager;

using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Gile.AutoCAD.Inspector
{
    public abstract class LabelItem
    {
        public object Value { get; }

        public string Label { get; private set; }

        public LabelItem(object value)
        {
            Value = value;
            string format = GetNumberFormat();
            switch (value)
            {
                case null: Label = "(Null)"; break;
                // ObjectId
                case ObjectId id:
                    if (id.IsNull)
                        Label = "(Null)";
                    else
                        using (var tr = new OpenCloseTransaction())
                            Label = $"< {tr.GetObject(id, OpenMode.ForRead).GetType().Name} >";
                    break;
                // Numeric values
                case double d: Label = d.ToString(format); break;
                case Point2d p: Label = p.ToString(format, CultureInfo.CurrentCulture); break;
                case Point3d p: Label = p.ToString(format, CultureInfo.CurrentCulture); break;
                case Vector2d v: Label = v.ToString(format, CultureInfo.CurrentCulture); break;
                case Vector3d v: Label = v.ToString(format, CultureInfo.CurrentCulture); break;
                // AutoCAD types
                case Matrix3d x: SetAcadTypeLabel(x); break;
                case Extents3d x: SetAcadTypeLabel(x); break;
                case Database x: SetAcadTypeLabel(x); break;
                case ResultBuffer x: SetAcadTypeLabel(x); break;
                case CoordinateSystem3d x: SetAcadTypeLabel(x); break;
                case AttributeCollection x: SetAcadTypeLabel(x); break;
                case DynamicBlockReferencePropertyCollection x: SetAcadTypeLabel(x); break;
                case DynamicBlockReferenceProperty x: SetAcadTypeLabel(x); break;
                case EntityColor x: SetAcadTypeLabel(x); break;
                case Entity3d x: SetAcadTypeLabel(x); break;
                case FitData x: SetAcadTypeLabel(x); break;
                case NurbsData x: SetAcadTypeLabel(x); break;
                case Point3dCollection x: SetAcadTypeLabel(x); break;
                case DoubleCollection x: SetAcadTypeLabel(x); break;
                case Spline x: SetAcadTypeLabel(x); break;
                case LayerFilterTree x: SetAcadTypeLabel(x); break;
                case LayerFilterCollection x: SetAcadTypeLabel(x); break;
                case LayerFilter x: SetAcadTypeLabel(x); break;
                case LayerFilterDisplayImages x: SetAcadTypeLabel(x); break;
                case DatabaseSummaryInfo x: SetAcadTypeLabel(x); break;
                // Inspector types
                case PolylineVertices x: SetInspectorTypeLabel(x); break;
                case PolylineVertex x: SetInspectorTypeLabel(x); break;
                case Polyline3dVertices x: SetInspectorTypeLabel(x); break;
                case Polyline2dVertices x: SetInspectorTypeLabel(x); break;

                case Dictionary<string, string>.Enumerator x: SetAcadTypeLabel(x); break;

                default: Label = value.ToString(); break;
            }
        }

        private static string GetNumberFormat()
        {
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

            return format;
        }
        private void SetAcadTypeLabel(object x) => Label = $"< {x.GetType().Name} >";
        private void SetInspectorTypeLabel(object x) => Label = $"< Inspector.{x.GetType().Name} >";
    }
}
