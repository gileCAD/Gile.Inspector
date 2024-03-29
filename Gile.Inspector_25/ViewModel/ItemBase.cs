using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Base class for TreeView and ListView items.
    /// </summary>
    public abstract class ItemBase
    {
        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Gets the string representing the value in the ListView.
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Creates a new intance of ItemBase.
        /// </summary>
        /// <param name="value">Value of the property.</param>
        protected ItemBase(object? value)
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
                        using var tr = id.Database.TransactionManager.StartTransaction();
                        var dbObject = tr.GetObject(id, OpenMode.ForRead);
                        Label = $"< {dbObject.GetType().Name} \t{dbObject.Handle} >";
                    }
                    break;
                case double d:
                    Label = d.ToString(NumberFormat());
                    break;
                case Enum _:
                case Handle _:
                    Label = value?.ToString() ?? "(Null)";
                    break;
                case Point2d _:
                case Point3d _:
                case Vector2d _:
                case Vector3d _:
                    Label = ((IFormattable)value).ToString(NumberFormat(), CultureInfo.CurrentCulture);
                    break;
                case DBObject dBObject:
                    Label = $"< {value.GetType().Name} \t{dBObject.Handle} >";
                    break;
                case Dictionary<string, string>.Enumerator _:
                    Label = "Custom properties";
                    break;
                default:
                    var type = value.GetType();
                    string? nspace = type.Namespace;
                    if (nspace != null && nspace.StartsWith("Autodesk.AutoCAD"))
                    {
                        Label = $"< {type.Name} >";
                    }
                    else if (nspace != null && nspace.StartsWith("Gile.AutoCAD.R25.Inspector"))
                    {
                        Label = $"< Inspector.{type.Name} >";
                    }
                    else
                    {
                        Label = value?.ToString() ?? "(Null)";
                    }
                    break;
            }
        }

        private static string NumberFormat()
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
