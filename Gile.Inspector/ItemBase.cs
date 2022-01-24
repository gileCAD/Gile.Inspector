using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.LayerManager;

using System.Collections.Generic;
using System.Globalization;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Base class for TreeView and ListView items.
    /// </summary>
    public abstract class ItemBase
    {
        public object Value { get; }

        public string Label { get; private set; }

        public ItemBase(object value)
        {
            Value = value;
            switch (value)
            {
                case null: Label = "(Null)"; break;
                // ObjectId
                case ObjectId id:
                    if (id.IsNull)
                    {
                        Label = "(Null)";
                    }
                    else
                    {
                        using (var tr = id.Database.TransactionManager.StartTransaction())
                        {
                            Label = $"< {tr.GetObject(id, OpenMode.ForRead).GetType().Name} >";
                        }
                    }
                    break;
                // Numeric values
                case double d: Label = d.ToString(GetNumberFormat()); break;
                case Point2d p: Label = p.ToString(GetNumberFormat(), CultureInfo.CurrentCulture); break;
                case Point3d p: Label = p.ToString(GetNumberFormat(), CultureInfo.CurrentCulture); break;
                case Vector2d v: Label = v.ToString(GetNumberFormat(), CultureInfo.CurrentCulture); break;
                case Vector3d v: Label = v.ToString(GetNumberFormat(), CultureInfo.CurrentCulture); break;
                // AutoCAD types
                case Matrix3d _:
                case Extents3d _:
                case Database _:
                case ResultBuffer _:
                case CoordinateSystem3d _:
                case AttributeCollection _:
                case DynamicBlockReferencePropertyCollection _:
                case DynamicBlockReferenceProperty _:
                case EntityColor _:
                case Entity3d _:
                case FitData _:
                case NurbsData _:
                case Point3dCollection _:
                case DoubleCollection _:
                case DBObject _:
                case LayerFilterTree _:
                case LayerFilterCollection _:
                case LayerFilter _:
                case LayerFilterDisplayImages _:
                case DatabaseSummaryInfo _:
                case AnnotationScale _:
                case Dictionary<string, string>.Enumerator _:
                case FontDescriptor _:
                case ObjectIdCollection _:
                case Profile3d _:
                case LoftProfile[] _:
                case Entity[] _:
                case LoftOptions _:
                case SweepOptions _:
                case RevolveOptions _:
                case Solid3dMassProperties _:
                case MlineStyleElementCollection _:
                case MlineStyleElement _:
                case CellRange _:
                case CellBorders _:
                case CellBorder _:
                case DataTypeParameter _:
                case RowsCollection _:
                case ColumnsCollection _:
                    Label = $"< {value.GetType().Name} >";
                    break;
                // Inspector types
                case PolylineVertices _:
                case PolylineVertex _:
                case Polyline3dVertices _:
                case Polyline2dVertices _:
                case ReferencesTo _:
                case ReferencedBy _:
                case MlineVertices _:
                    Label = $"< Inspector.{value.GetType().Name} >";
                    break;
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
                for (int i = 0; i < luprec; i++) format += "0";
            }
            return format;
        }
    }
}
