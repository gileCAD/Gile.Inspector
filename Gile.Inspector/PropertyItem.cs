using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.LayerManager;
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
            yield return new PropertyItem("Name", id.ObjectClass.Name, typeof(RXClass), false);
            yield return new PropertyItem("DxfName", id.ObjectClass.DxfName, typeof(RXClass), false);
            using (var tr = new OpenCloseTransaction())
            {
                var dbObj = tr.GetObject(id, OpenMode.ForRead);
                foreach (var item in ListDBObjectProperties(dbObj))
                {
                    yield return item;
                }
                if (dbObj is Polyline pl)
                    yield return new PropertyItem("Vertices", new PolylineVertices(pl), typeof(Polyline), true);
                else if (dbObj is Polyline3d pl3d)
                    yield return new PropertyItem("Vertices", new Polyline3dVertices(pl3d), typeof(Polyline3d), true);
                else if (dbObj is Polyline2d pl2d)
                    yield return new PropertyItem("Vertices", new Polyline2dVertices(pl2d), typeof(Polyline2d), true);
                tr.Commit();
            }
        }

        public static IEnumerable<PropertyItem> ListDBObjectProperties(DBObject dbObj)
        {
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
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (Type t in types)
            {
                var subType = t;
                foreach (var prop in t.GetProperties(flags))
                {
                    string name = prop.Name;
                    object value;
                    try { value = prop.GetValue(dbObj, null) ?? "(Null)"; }
                    catch (System.Exception e) { value = e.Message; }
                    bool isInspectable = 
                        CheckIsInspectable(value) &&
                        !((value is ObjectId id) && id == dbObj.ObjectId) &&
                        !((value is DBObject obj) && obj.GetType() == dbObj.GetType() && obj.Handle == dbObj.Handle);
                    yield return new PropertyItem(name, value, subType, isInspectable);
                }
            }
        }

        public static IEnumerable<PropertyItem> ListCurve3dProperties(Entity3d curve)
        {
            var types = new List<Type>();
            var type = curve.GetType();
            while (true)
            {
                types.Add(type);
                if (type == typeof(Entity3d))
                    break;
                type = type.BaseType;
            }
            types.Reverse();
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (Type t in types)
            {
                var subType = t;
                foreach (var prop in t.GetProperties(flags))
                {
                    string name = prop.Name;
                    object value;
                    try { value = prop.GetValue(curve, null) ?? "(Null)"; }
                    catch (System.Exception e) { value = e.Message; }
                    bool isInspectable = CheckIsInspectable(value);
                    yield return new PropertyItem(name, value, subType, isInspectable);
                }
            }
        }

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

        public static IEnumerable<PropertyItem> ListFitDataProperties(FitData data)
        {
            foreach (var item in ListProperties(data))
            {
                yield return item;
            }
            var fitPoints = data.GetFitPoints();
            yield return new PropertyItem("FitPoints", fitPoints, typeof(FitData), 0 < fitPoints.Count);
        }

        public static IEnumerable<PropertyItem> ListNurbsDataProperties(NurbsData data)
        {
            foreach (var item in ListProperties(data))
            {
                yield return item;
            }
            var controlPoints = data.GetControlPoints();
            var knots = data.GetKnots();
            var weights = data.GetWeights();
            yield return new PropertyItem("FitPoints", controlPoints, typeof(NurbsData), 0 < controlPoints.Count);
            yield return new PropertyItem("Knots", knots, typeof(NurbsData), 0 < knots.Count);
            yield return new PropertyItem("Weights", weights, typeof(NurbsData), 0 < weights.Count);
        }

        public static IEnumerable<PropertyItem> ListResultBufferProperties(ResultBuffer resbuf) =>
            resbuf.Cast<TypedValue>().Select(tv => new PropertyItem(tv));

        public static IEnumerable<PropertyItem> ListPoint3dCollectionProperties(Point3dCollection points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                yield return new PropertyItem($"[{i}]", points[i], typeof(Point3dCollection), false);
            }
        }

        public static IEnumerable<PropertyItem> ListDoubleCollectionProperties(DoubleCollection doubles)
        {
            for (int i = 0; i < doubles.Count; i++)
            {
                yield return new PropertyItem($"[{i}]", doubles[i], typeof(DoubleCollection), false);
            }
        }

        public static IEnumerable<PropertyItem> ListDictEnumProperties(Dictionary<string, string>.Enumerator dictEnum)
        {
            while (dictEnum.MoveNext())
            {
                yield return new PropertyItem(dictEnum.Current.Key, dictEnum.Current.Value, typeof(DatabaseSummaryInfo), false);
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
            value is Color ||
            value is Entity3d ||
            value is FitData ||
            value is NurbsData ||
            value is Spline ||
            value is Database ||
            value is LayerFilterTree ||
            (value is LayerFilterCollection filters && 0 < filters.Count) ||
            value is LayerFilter ||
            value is LayerFilterDisplayImages ||
            value is DatabaseSummaryInfo ||
            value is Dictionary<string, string>.Enumerator dictEnum && dictEnum.MoveNext();
        #endregion
    }
}
