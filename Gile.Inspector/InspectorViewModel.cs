﻿using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.LayerManager;
using Autodesk.AutoCAD.Runtime;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using AcDb = Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Interaction logic for InspectorDialog.xaml
    /// </summary>
    public class InspectorViewModel : INotifyPropertyChanged
    {
        private IEnumerable<PropertyItem> properties;
        private IEnumerable<InspectableItem> inspectables;

        #region INotitfyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region Constructors
        public InspectorViewModel(Database db)
        {
            var item = new InspectableItem(db);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(db);
        }

        public InspectorViewModel(ObjectId id)
        {
            var item = new InspectableItem(id);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListObjectIdProperties(id);
        }

        public InspectorViewModel(ResultBuffer resbuf)
        {
            var item = new InspectableItem(resbuf);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(resbuf);
        }

        public InspectorViewModel(ObjectIdCollection ids)
        {
            var item = new InspectableItem(ids[0]);
            ItemTree = ids.Cast<ObjectId>().Select(id => new InspectableItem(id));
            item.IsSelected = true;
            Properties = ListObjectIdProperties(ids[0]);
        }

        public InspectorViewModel(AcDb.AttributeCollection attribs)
        {
            var item = new InspectableItem(attribs[0]);
            ItemTree = attribs.Cast<ObjectId>().Select(id => new InspectableItem(id));
            item.IsSelected = true;
            Properties = ListObjectIdProperties(attribs[0]);
            //SetSubTypeGroups();
        }

        public InspectorViewModel(DynamicBlockReferencePropertyCollection props)
        {
            var item = new InspectableItem(props[0]);
            ItemTree = props.Cast<DynamicBlockReferenceProperty>().Select(p => new InspectableItem(p));
            item.IsSelected = true;
            Properties = ListProperties(props[0]);
        }

        public InspectorViewModel(Matrix3d matrix)
        {
            var item = new InspectableItem(matrix);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(matrix);
        }

        public InspectorViewModel(Extents3d extents)
        {
            var item = new InspectableItem(extents);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(extents);
        }

        public InspectorViewModel(CoordinateSystem3d cs)
        {
            var item = new InspectableItem(cs);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(cs);
        }

        public InspectorViewModel(EntityColor co)
        {
            var item = new InspectableItem(co);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(co);
        }

        public InspectorViewModel(Color co)
        {
            var item = new InspectableItem(co);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(co);
        }

        public InspectorViewModel(PolylineVertices vertices)
        {
            var item = new InspectableItem(vertices.Vertices[0]);
            ItemTree = vertices.Vertices.Select(v => new InspectableItem(v));
            item.IsSelected = true;
            Properties = ListProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(Entity3d curve)
        {
            var item = new InspectableItem(curve);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListCurve3dProperties(curve);
        }

        public InspectorViewModel(Polyline3dVertices vertices)
        {
            var item = new InspectableItem(vertices.Vertices[0]);
            ItemTree = vertices.Vertices.Cast<ObjectId>().Select(id => new InspectableItem(id));
            item.IsSelected = true;
            Properties = ListObjectIdProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(Polyline2dVertices vertices)
        {
            var item = new InspectableItem(vertices.Vertices[0]);
            ItemTree = vertices.Vertices.Cast<ObjectId>().Select(id => new InspectableItem(id));
            item.IsSelected = true;
            Properties = ListObjectIdProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(FitData fitData)
        {
            var item = new InspectableItem(fitData);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListFitDataProperties(fitData);
        }

        public InspectorViewModel(NurbsData nurbsData)
        {
            var item = new InspectableItem(nurbsData);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListNurbsDataProperties(nurbsData);
        }

        public InspectorViewModel(Point3dCollection points)
        {
            var item = new InspectableItem(points);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(points);
        }

        public InspectorViewModel(DoubleCollection doubles)
        {
            var item = new InspectableItem(doubles);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(doubles);
        }

        public InspectorViewModel(Spline spline)
        {
            var item = new InspectableItem(spline);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListDBObjectProperties(spline);
        }

        public InspectorViewModel(LayerFilterTree filterTree)
        {
            var item = new InspectableItem(filterTree.Root);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(filterTree.Root);
        }

        public InspectorViewModel(LayerFilterCollection filters)
        {
            var item = new InspectableItem(filters[0]);
            ItemTree = filters.Cast<LayerFilter>().Select(f => new InspectableItem(f));
            item.IsSelected = true;
            Properties = ListLayerFilterProperties(filters[0]);
        }

        public InspectorViewModel(LayerFilter filter)
        {
            var item = new InspectableItem(filter);
            ItemTree = filter.NestedFilters.Cast<LayerFilter>().Select(f => new InspectableItem(f));
            item.IsSelected = true;
            Properties = ListProperties(filter);
        }

        public InspectorViewModel(LayerFilterDisplayImages images)
        {
            var item = new InspectableItem(images);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(images);
        }

        public InspectorViewModel(DatabaseSummaryInfo info)
        {
            var item = new InspectableItem(info);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(info);
        }

        public InspectorViewModel(Dictionary<string, string>.Enumerator dict)
        {
            var item = new InspectableItem(dict);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListDictEnumProperties(dict);
        }

        public InspectorViewModel(AnnotationScale scale)
        {
            var item = new InspectableItem(scale);
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(scale);
        }

        public InspectorViewModel(FontDescriptor font)
        {
            var item = new InspectableItem(font); ItemTree = new[] { item };
            ItemTree = new[] { item };
            item.IsSelected = true;
            Properties = ListProperties(font);
        }
        #endregion

        #region Properties
        public IEnumerable<InspectableItem> ItemTree
        {
            get { return inspectables; }
            set { inspectables = value; NotifyPropertyChanged(nameof(ItemTree)); }
        }

        public IEnumerable<PropertyItem> Properties
        {
            get { return properties; }
            set
            {
                properties = value;
                NotifyPropertyChanged(nameof(Properties));
            }
        }

        public PropertyItem SelectedProperty
        {
            get { return null; }
            set
            {
                if (value != null && value.IsInspectable)
                {
                    InspectorViewModel viewModel = null;
                    switch (value.Value)
                    {
                        case ObjectId id: viewModel = new InspectorViewModel(id); break;
                        case ResultBuffer resbuf: viewModel = new InspectorViewModel(resbuf); break;
                        case Matrix3d matrix: viewModel = new InspectorViewModel(matrix); break;
                        case CoordinateSystem3d coordSystem: viewModel = new InspectorViewModel(coordSystem); break;
                        case Extents3d extents: viewModel = new InspectorViewModel(extents); break;
                        case AcDb.AttributeCollection attribs: viewModel = new InspectorViewModel(attribs); break;
                        case DynamicBlockReferencePropertyCollection dynProps: viewModel = new InspectorViewModel(dynProps); break;
                        case EntityColor entityColor: viewModel = new InspectorViewModel(entityColor); break;
                        case Color color: viewModel = new InspectorViewModel(color); break;
                        case PolylineVertices vertices: viewModel = new InspectorViewModel(vertices); break;
                        case Entity3d entity3d: viewModel = new InspectorViewModel(entity3d); break;
                        case Polyline3dVertices vertices: viewModel = new InspectorViewModel(vertices); break;
                        case Polyline2dVertices vertices: viewModel = new InspectorViewModel(vertices); break;
                        case FitData fitData: viewModel = new InspectorViewModel(fitData); break;
                        case NurbsData nurbsData: viewModel = new InspectorViewModel(nurbsData); break;
                        case Point3dCollection points: viewModel = new InspectorViewModel(points); break;
                        case DoubleCollection doubles: viewModel = new InspectorViewModel(doubles); break;
                        case Spline spline: viewModel = new InspectorViewModel(spline); break;
                        case Database db: viewModel = new InspectorViewModel(db); break;
                        case LayerFilterTree filterTree: viewModel = new InspectorViewModel(filterTree); break;
                        case LayerFilterCollection filters: viewModel = new InspectorViewModel(filters); break;
                        case LayerFilter filter: viewModel = new InspectorViewModel(filter); break;
                        case LayerFilterDisplayImages images: viewModel = new InspectorViewModel(images); break;
                        case DatabaseSummaryInfo info: viewModel = new InspectorViewModel(info); break;
                        case Dictionary<string, string>.Enumerator dict: viewModel = new InspectorViewModel(dict); break;
                        case AnnotationScale scale: viewModel = new InspectorViewModel(scale); break;
                        case FontDescriptor font: viewModel = new InspectorViewModel(font); break;
                        case ObjectIdCollection ids: viewModel = new InspectorViewModel(ids); break;
                        default: break;
                    }
                    viewModel?.ShowDialog();
                }
            }
        }
        #endregion

        public void ShowDialog() => AcAp.ShowModalWindow(new InspectorDialog(this));

        #region SetProperties methods
        /// <summary>
        /// Handles the TreeView_SelectedItemChanged event.
        /// </summary>
        /// <param name="obj">e.NewValue</param>
        public void SetProperties(object obj)
        {
            var item = (InspectableItem)obj;
            if (!item.ObjectId.IsNull)
                Properties = ListObjectIdProperties(item.ObjectId);
            else if (item.DynamicProperty != null)
                Properties = ListProperties(item.DynamicProperty);
            else if (item.ResultBuffer != null)
                Properties = ListResultBufferProperties(item.ResultBuffer);
            else if (item.PolylineVertex != null)
                Properties = ListProperties(item.PolylineVertex);
            else if (item.Points != null)
                Properties = ListPoint3dCollectionProperties(item.Points);
            else if (item.Doubles != null)
                Properties = ListDoubleCollectionProperties(item.Doubles);
            else if (item.LayerFilter != null)
                Properties = ListLayerFilterProperties(item.LayerFilter);
        }
        #endregion

        #region ListProperties methods
        private IEnumerable<PropertyItem> ListObjectIdProperties(ObjectId id)
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
                if (dbObj is AcDb.Polyline pl)
                    yield return new PropertyItem("Vertices", new PolylineVertices(pl), typeof(AcDb.Polyline), true);
                else if (dbObj is Polyline3d pl3d)
                    yield return new PropertyItem("Vertices", new Polyline3dVertices(pl3d), typeof(Polyline3d), true);
                else if (dbObj is Polyline2d pl2d)
                    yield return new PropertyItem("Vertices", new Polyline2dVertices(pl2d), typeof(Polyline2d), true);
                else if (dbObj is BlockTableRecord)
                {
                    var btr = (BlockTableRecord)dbObj;
                    var ids = new ObjectIdCollection();
                    foreach (ObjectId oId in btr)
                    {
                        ids.Add(oId);
                    }
                    yield return new PropertyItem("Entities within block", ids, typeof(BlockTableRecord), 0 < ids.Count);
                    if (!btr.IsLayout)
                    {
                        ids = btr.GetBlockReferenceIds(true, true);
                        yield return new PropertyItem("Block reference Ids (directOnly = true)", ids, typeof(BlockTableRecord), 0 < ids.Count);
                        ids = btr.GetBlockReferenceIds(false, true);
                        yield return new PropertyItem("Block reference Ids (directOnly = false)", ids, typeof(BlockTableRecord), 0 < ids.Count);
                    }
                }
                tr.Commit();
            }
        }

        private IEnumerable<PropertyItem> ListDBObjectProperties(DBObject dbObj)
        {
            var types = new List<Type>();
            var type = dbObj.GetType();
            while (type != typeof(Drawable)
                && type != typeof(DisposableWrapper)
                && type != null)
            {
                types.Add(type);
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

        private IEnumerable<PropertyItem> ListCurve3dProperties(Entity3d curve)
        {
            var types = new List<Type>();
            var type = curve.GetType();
            while (type != typeof(DisposableWrapper) && type != null)
            {
                types.Add(type);
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

        private IEnumerable<PropertyItem> ListLayerFilterProperties(LayerFilter layerFilter)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var prop in typeof(LayerFilter).GetProperties(flags))
            {
                string name = prop.Name;
                object value;
                try { value = prop.GetValue(layerFilter, null) ?? "(Null)"; }
                catch (System.Exception e) { value = e.Message; }
                bool isInspectable = CheckIsInspectable(value);
                if (name == "Parent" && value is LayerFilter f)
                    yield return new PropertyItem(name, f.Parent, typeof(LayerFilter), isInspectable && f.Parent != null);
                else
                    yield return new PropertyItem(name, value, typeof(LayerFilter), isInspectable);
            }
        }

        private IEnumerable<PropertyItem> ListProperties<T>(T item)
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

        private IEnumerable<PropertyItem> ListFitDataProperties(FitData data)
        {
            foreach (var item in ListProperties(data))
            {
                yield return item;
            }
            var fitPoints = data.GetFitPoints();
            yield return new PropertyItem("FitPoints", fitPoints, typeof(FitData), 0 < fitPoints.Count);
        }

        private IEnumerable<PropertyItem> ListNurbsDataProperties(NurbsData data)
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

        private IEnumerable<PropertyItem> ListResultBufferProperties(ResultBuffer resbuf) =>
            resbuf.Cast<TypedValue>().Select(tv => new PropertyItem(tv.TypeCode.ToString(), tv.Value, typeof(ResultBuffer), false));

        private IEnumerable<PropertyItem> ListPoint3dCollectionProperties(Point3dCollection points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                yield return new PropertyItem($"[{i}]", points[i], typeof(Point3dCollection), false);
            }
        }

        private IEnumerable<PropertyItem> ListDoubleCollectionProperties(DoubleCollection doubles)
        {
            for (int i = 0; i < doubles.Count; i++)
            {
                yield return new PropertyItem($"[{i}]", doubles[i], typeof(DoubleCollection), false);
            }
        }

        private IEnumerable<PropertyItem> ListDictEnumProperties(Dictionary<string, string>.Enumerator dictEnum)
        {
            while (dictEnum.MoveNext())
            {
                yield return new PropertyItem(dictEnum.Current.Key, dictEnum.Current.Value, typeof(Dictionary<string, string>.Enumerator), false);
            }
        }

        private static bool CheckIsInspectable(object value) =>
            (value is ObjectId id && !id.IsNull) ||
            (value is ResultBuffer && value != null) ||
            value is Matrix3d ||
            (value is Extents3d && value != null) ||
            value is CoordinateSystem3d ||
            (value is AcDb.AttributeCollection attCol && 0 < attCol.Count) ||
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
            value is Dictionary<string, string>.Enumerator dictEnum && dictEnum.MoveNext() ||
            value is AnnotationScale ||
            value is FontDescriptor;
        #endregion
    }
}
