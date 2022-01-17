using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gile.AutoCAD.Inspector
{
    public class PropertyItem : LabelItem
    {
        #region Properties
        public bool IsInspectable { get; }
        
        public string Name { get; }

        public Type SubType { get; }
        #endregion

        #region Constuctors
        public PropertyItem(string name, object value, Type subType, bool isInspectable) : base(value)
        {
            Name = name;
            SubType = subType;
            IsInspectable = isInspectable;
        }

        public PropertyItem(TypedValue typedValue) : base(typedValue.Value)
        {
            Name = typedValue.TypeCode.ToString();
        }
        #endregion

        #region ListProperties methods
        public static IEnumerable<PropertyItem> ListObjectIdProperties(ObjectId id)
        {
            if (id.IsNull)
                throw new ArgumentNullException("id");
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            using (var tr = new OpenCloseTransaction())
            {
                var dbObj = tr.GetObject(id, OpenMode.ForRead);
                var types = new List<Type>();
                var type = dbObj.GetType();
                while (true)
                {
                    types.Add(type);
                    if (type == typeof(DBObject))
                        break;
                    type = type.BaseType;
                }
                types.Reverse();
                yield return new PropertyItem("Name", id.ObjectClass.Name, typeof(RXClass), false);
                yield return new PropertyItem("DxfName", id.ObjectClass.DxfName, typeof(RXClass), false);
                foreach (Type t in types)
                {
                    var subType = t;
                    foreach (var prop in t.GetProperties(flags))
                    {
                        string name = prop.Name;
                        object value;
                        try { value = prop.GetValue(dbObj, null) ?? "(Null)"; }
                        catch (System.Exception e) { value = e.Message; }
                        bool isInspectable = CheckIsInspectable(value);
                        if ((value is ObjectId x) && x == id)
                            isInspectable = false;
                        yield return new PropertyItem(name, value, subType, isInspectable);
                    }
                }
                tr.Commit();
            }
        }

        public static IEnumerable<PropertyItem> ListResultBufferProperties(ResultBuffer resbuf) =>
            resbuf.Cast<TypedValue>().Select(tv => new PropertyItem(tv));

        public static IEnumerable<PropertyItem> ListProperties<T>(T item)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var prop in typeof(T).GetProperties(flags))
            {
                string name = prop.Name;
                object value;
                try { value = prop.GetValue(item, null) ?? "(Null)"; }
                catch (System.Exception e) { value = e.Message; }
                bool isInspectable = CheckIsInspectable(value);
                yield return new PropertyItem(name, value, typeof(T), isInspectable);
            }
        }

        private static bool CheckIsInspectable(object value) =>
            (value is ObjectId id && !id.IsNull) ||
            (value is ResultBuffer && value != null) ||
            value is Matrix3d ||
            (value is Extents3d && value != null) ||
            value is CoordinateSystem3d ||
            (value is AttributeCollection attCol && 0 < attCol.Count) ||
            (value is DynamicBlockReferencePropertyCollection props && 0 < props.Count) ||
            value is EntityColor ||
            value is Color;
        #endregion
    }
}
