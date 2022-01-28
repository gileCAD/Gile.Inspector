using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

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
                case Dictionary<string, string>.Enumerator _:
                    Label = "Custom properties";
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
        }
    }
}
