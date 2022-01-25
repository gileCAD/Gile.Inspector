using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.LayerManager;

using System;
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
                case null:
                    Label = "(Null)"; 
                    break;
                case ObjectId id:
                    if (id.IsNull)
                    {
                        Label = "(Null)";
                    }
                    else
                    {
                        using (var tr = id.Database.TransactionManager.StartTransaction())
                        {
                            var dbObject = tr.GetObject(id, OpenMode.ForRead);
                            Label = $"< {dbObject.GetType().Name}\t{dbObject.Handle} >";
                        }
                    }
                    break;
                case double d:
                    Label = d.ToString(Commands.NumberFormat);
                    break;
                case Enum _:
                case Handle _:
                    Label = value.ToString();
                    break;
                case Point2d _:
                case Point3d _:
                case Vector2d _:
                case Vector3d _:
                    Label = ((IFormattable)value).ToString(Commands.NumberFormat, CultureInfo.CurrentCulture);
                    break;
                case DBObject dBObject:
                    Label = $"< {value.GetType().Name}\t{dBObject.Handle} >";
                    break;
                default:
                    var type = value.GetType();
                    string nspace = type.Namespace;
                    if (nspace.StartsWith("Autodesk.AutoCAD"))
                    {
                        Label = $"< {type.Name} >";
                    }
                    else if (nspace.StartsWith("Gile.AutoCAD.Inspector"))
                    {
                        Label = $"[ {type.Name} ]";
                    }
                    else
                    {
                        Label = value.ToString();
                    }
                    break;
            }

            //if (value == null)
            //{
            //    Label = "(Null)";
            //}
            //else if (value is ObjectId id)
            //{
            //    if (id.IsNull)
            //    {
            //        Label = "(Null)";
            //    }
            //    else
            //    {
            //        using (var tr = id.Database.TransactionManager.StartTransaction())
            //        {
            //            var dbObject = tr.GetObject(id, OpenMode.ForRead);
            //            Label = $"< {dbObject.GetType().Name}\t{dbObject.Handle} >";
            //        }
            //    }
            //}
            //else if (value is double d)
            //{
            //    Label = d.ToString(Commands.NumberFormat);
            //}
            //else if (value is Enum)
            //{
            //    Label = value.ToString();
            //}
            //else if (value is Handle)
            //{
            //    Label = value.ToString();
            //}
            //else if (value is Point2d || value is Point3d || value is Vector2d || value is Vector3d)
            //{
            //    Label = ((IFormattable)value).ToString(Commands.NumberFormat, CultureInfo.CurrentCulture);
            //}
            //else if (value is DBObject dBObject)
            //{
            //    Label = $"< {value.GetType().Name}\t{dBObject.Handle} >";
            //}
            //else
            //{
            //    var type = value.GetType();
            //    string nspace = type.Namespace;
            //    if (nspace.StartsWith("Autodesk.AutoCAD"))
            //    {
            //        Label = $"< {type.Name} >";
            //    }
            //    else if (nspace.StartsWith("Gile.AutoCAD.Inspector"))
            //    {
            //        Label = $"[ {type.Name} ]";
            //    }
            //    else
            //    {
            //        Label = value.ToString();
            //    }
            //}

            //switch (value)
            //{
            //    case null: Label = "(Null)"; break;
            //    // ObjectId
            //    case ObjectId id:
            //        if (id.IsNull)
            //        {
            //            Label = "(Null)";
            //        }
            //        else
            //        {
            //            using (var tr = id.Database.TransactionManager.StartTransaction())
            //            {
            //                Label = $"< {tr.GetObject(id, OpenMode.ForRead).GetType().Name} >";
            //            }
            //        }
            //        break;
            //    // Numeric values
            //    case double d: Label = d.ToString(Commands.NumberFormat); break;
            //    case Point2d p: Label = p.ToString(Commands.NumberFormat, CultureInfo.CurrentCulture); break;
            //    case Point3d p: Label = p.ToString(Commands.NumberFormat, CultureInfo.CurrentCulture); break;
            //    case Vector2d v: Label = v.ToString(Commands.NumberFormat, CultureInfo.CurrentCulture); break;
            //    case Vector3d v: Label = v.ToString(Commands.NumberFormat, CultureInfo.CurrentCulture); break;
            //    // AutoCAD types
            //    case Autodesk.AutoCAD.Runtime.RXObject _:
            //    case Matrix3d _:
            //    case Extents3d _:
            //    //case Database _:
            //    case ResultBuffer _:
            //    case CoordinateSystem3d _:
            //    case AttributeCollection _:
            //    case DynamicBlockReferencePropertyCollection _:
            //    case DynamicBlockReferenceProperty _:
            //    case EntityColor _:
            //    case Entity3d _:
            //    case FitData _:
            //    case NurbsData _:
            //    case Point3dCollection _:
            //    case DoubleCollection _:
            //    //case DBObject _:
            //    case LayerFilterTree _:
            //    case LayerFilterCollection _:
            //    //case LayerFilter _:
            //    case LayerFilterDisplayImages _:
            //    case DatabaseSummaryInfo _:
            //    //case AnnotationScale _:
            //    case Dictionary<string, string>.Enumerator _:
            //    case FontDescriptor _:
            //    case ObjectIdCollection _:
            //    //case Profile3d _:
            //    case LoftProfile[] _:
            //    case Entity[] _:
            //    case LoftOptions _:
            //    case SweepOptions _:
            //    case RevolveOptions _:
            //    case Solid3dMassProperties _:
            //    case MlineStyleElementCollection _:
            //    case MlineStyleElement _:
            //    case CellRange _:
            //    case CellBorders _:
            //    case CellBorder _:
            //    case DataTypeParameter _:
            //    case RowsCollection _:
            //    case ColumnsCollection _:
            //    case HyperLinkCollection _:
            //    case HyperLink _:
            //    //case GeomRef _:
            //    case EdgeRef[] _:
            //    case SubentityId _:
            //    //case CompoundObjectId _:
            //    case HatchLoop _:
            //    case Curve2dCollection _:
            //    case BulgeVertexCollection _:
            //    case Entity2d _:
            //    case BulgeVertex _:
            //    case KnotCollection _:
            //    case NurbCurve2dData _:
            //    case NurbCurve2dFitData _:
            //    case NurbCurve3dData _:
            //    case NurbCurve3dFitData _:
            //    case Tolerance _:
            //    case Point2dCollection _:
            //        Label = $"< {value.GetType().Name} >";
            //        break;
            //    // Inspector types
            //    case PolylineVertices _:
            //    case PolylineVertex _:
            //    case Polyline3dVertices _:
            //    case Polyline2dVertices _:
            //    case ReferencesTo _:
            //    case ReferencedBy _:
            //    case MlineVertices _:
            //    case HatchLoopCollection _:
            //        Label = $"< Inspector.{value.GetType().Name} >";
            //        break;
            //    default: Label = value.ToString(); break;
            //}
        }
    }
}
