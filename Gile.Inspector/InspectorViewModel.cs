using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using System;

namespace Gile.AutoCAD.Inspector
{
    public class InspectorViewModel : INotifyPropertyChanged
    {
        private IEnumerable<PropertyItem> properties;
        private IEnumerable<InspectableItem> inspectables;
        private PropertyItem selectedProperty;

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
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetProperties(db);
        }

        public InspectorViewModel(ObjectId id)
        {
            var item = new InspectableItem(id);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetObjectIdProperties(id);
        }

        public InspectorViewModel(ResultBuffer resbuf)
        {
            var item = new InspectableItem(resbuf);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetProperties(resbuf);
        }

        public InspectorViewModel(ObjectIdCollection ids)
        {
            var item = new InspectableItem(ids[0]);
            ItemTree = ids.Cast<ObjectId>().Select(id => new InspectableItem(id));
            item.IsSelected = true;
            SetObjectIdProperties(ids[0]);
        }

        public InspectorViewModel(AcDb.AttributeCollection attribs)
        {
            var item = new InspectableItem(attribs[0]);
            ItemTree = attribs.Cast<ObjectId>().Select(id => new InspectableItem(id));
            item.IsSelected = true;
            SetObjectIdProperties(attribs[0]);
        }

        public InspectorViewModel(DynamicBlockReferencePropertyCollection props)
        {
            var item = new InspectableItem(props[0]);
            ItemTree = props.Cast<DynamicBlockReferenceProperty>().Select(p => new InspectableItem(p));
            item.IsSelected = true;
            SetProperties(props[0]);
        }

        public InspectorViewModel(Matrix3d matrix)
        {
            var item = new InspectableItem(matrix);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetProperties(matrix);
        }

        public InspectorViewModel(Extents3d extents)
        {
            var item = new InspectableItem(extents);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetProperties(extents);
        }

        public InspectorViewModel(CoordinateSystem3d cs)
        {
            var item = new InspectableItem(cs);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetProperties(cs);
        }

        public InspectorViewModel(EntityColor co)
        {
            var item = new InspectableItem(co);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetProperties(co);
        }

        public InspectorViewModel(Color co)
        {
            var item = new InspectableItem(co);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetProperties(co);
        }

        public InspectorViewModel(PolylineVertices vertices)
        {
            var item = new InspectableItem(vertices.Vertices[0]);
            ItemTree = vertices.Vertices.Select(v => new InspectableItem(v));
            item.IsSelected = true;
            SetProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(Entity3d curve)
        {
            var item = new InspectableItem(curve);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetCurve3dProperties(curve);
        }

        public InspectorViewModel(Polyline3dVertices vertices)
        {
            var item = new InspectableItem(vertices.Vertices[0]);
            ItemTree = vertices.Vertices.Cast<ObjectId>().Select(id => new InspectableItem(id));
            item.IsSelected = true;
            SetObjectIdProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(Polyline2dVertices vertices)
        {
            var item = new InspectableItem(vertices.Vertices[0]);
            ItemTree = vertices.Vertices.Cast<ObjectId>().Select(id => new InspectableItem(id));
            item.IsSelected = true;
            SetObjectIdProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(FitData fitData)
        {
            var item = new InspectableItem(fitData);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetFitDataProperties(fitData);
        }

        public InspectorViewModel(NurbsData nurbsData)
        {
            var item = new InspectableItem(nurbsData);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetNurbsDataProperties(nurbsData);
        }

        public InspectorViewModel(Point3dCollection points)
        {
            var item = new InspectableItem(points);
            ItemTree = new List<InspectableItem>{ item };
            item.IsSelected = true;
            SetProperties(points);
        }

        public InspectorViewModel(DoubleCollection doubles)
        {
            var item = new InspectableItem(doubles);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetProperties(doubles);
        }
        public InspectorViewModel(Spline spline)
        {
            var item = new InspectableItem(spline);
            ItemTree = new List<InspectableItem> { item };
            item.IsSelected = true;
            SetSplineProperties(spline);
        }
        #endregion

        #region Properties
        public IEnumerable<InspectableItem> ItemTree
        {
            get { return inspectables; }
            set { 
                inspectables = value; 
                NotifyPropertyChanged(nameof(ItemTree)); }
        }

        public IEnumerable<PropertyItem> Properties
        {
            get { return properties; }
            set { 
                properties = value; 
                NotifyPropertyChanged(nameof(Properties)); }
        }

        public PropertyItem SelectedProperty
        {
            get { return selectedProperty; }
            set
            {
                if (value != null)
                {
                    selectedProperty = null;
                    if (value.Value is ObjectId id)
                        ShowDialog(id);
                    else if (value.Value is ResultBuffer resbuf)
                        ShowDialog(resbuf);
                    else if (value.Value is Matrix3d matrix)
                        ShowDialog(matrix);
                    else if (value.Value is Extents3d extents)
                        ShowDialog(extents);
                    else if (value.Value is CoordinateSystem3d coordSystem)
                        ShowDialog(coordSystem);
                    else if (value.Value is AcDb.AttributeCollection attrib)
                        ShowDialog(attrib);
                    else if (value.Value is DynamicBlockReferencePropertyCollection dynProp)
                        ShowDialog(dynProp);
                    else if (value.Value is EntityColor entityColor)
                        ShowDialog(entityColor);
                    else if (value.Value is Color color)
                        ShowDialog(color);
                    else if (value.Value is PolylineVertices vertices)
                        ShowDialog(vertices);
                    else if (value.Value is Entity3d curve)
                        ShowDialog(curve);
                    else if (value.Value is Polyline3dVertices vertices3d)
                        ShowDialog(vertices3d);
                    else if (value.Value is Polyline2dVertices vertices2d)
                        ShowDialog(vertices2d);
                    else if (value.Value is FitData fitData)
                        ShowDialog(fitData);
                    else if (value.Value is NurbsData nurbsData)
                        ShowDialog(nurbsData);
                    else if (value.Value is Point3dCollection points)
                        ShowDialog(points);
                    else if (value.Value is DoubleCollection doubles)
                        ShowDialog(doubles);
                    else if (value.Value is Spline spline)
                        ShowDialog(spline);
                }
            }
        }
        #endregion

        #region ShowDialog methods
        public static void ShowDialog(Database db) => Show(new InspectorViewModel(db));

        public static void ShowDialog(ObjectId id) => Show(new InspectorViewModel(id));

        public static void ShowDialog(ResultBuffer resbuf) => Show(new InspectorViewModel(resbuf));

        public static void ShowDialog(ObjectIdCollection ids) => Show(new InspectorViewModel(ids));

        public static void ShowDialog(AcDb.AttributeCollection attrib) => Show(new InspectorViewModel(attrib));

        public static void ShowDialog(DynamicBlockReferencePropertyCollection props) => Show(new InspectorViewModel(props));

        public static void ShowDialog(Matrix3d matrix) => Show(new InspectorViewModel(matrix));

        public static void ShowDialog(Extents3d extents) => Show(new InspectorViewModel(extents));

        public static void ShowDialog(CoordinateSystem3d coordSystems) => Show(new InspectorViewModel(coordSystems));

        public static void ShowDialog(EntityColor entityColor) => Show(new InspectorViewModel(entityColor));

        public static void ShowDialog(Color color) => Show(new InspectorViewModel(color));

        public static void ShowDialog(PolylineVertices vertices) => Show(new InspectorViewModel(vertices));

        public static void ShowDialog(Entity3d entitiy3d) => Show(new InspectorViewModel(entitiy3d));

        public static void ShowDialog(Polyline3dVertices vertices) => Show(new InspectorViewModel(vertices));

        public static void ShowDialog(Polyline2dVertices vertices) => Show(new InspectorViewModel(vertices));

        public static void ShowDialog(FitData fitData) => Show(new InspectorViewModel(fitData));

        public static void ShowDialog(NurbsData nurbsData) => Show(new InspectorViewModel(nurbsData));

        public static void ShowDialog(Point3dCollection points) => Show(new InspectorViewModel(points));

        public static void ShowDialog(DoubleCollection doubles) => Show(new InspectorViewModel(doubles));

        public static void ShowDialog(Spline spline) => Show(new InspectorViewModel(spline));

        private static void Show(InspectorViewModel viewModel) => AcAp.ShowModalWindow(new InspectorDialog(viewModel));
        #endregion

        #region SetProperties methods
        public void SetObjectIdProperties(ObjectId id)
        {
            Properties = PropertyItem.ListObjectIdProperties(id);
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Properties);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SubType.Name");
            view.GroupDescriptions.Add(groupDescription);
        }

        public void SetCurve3dProperties(Entity3d curve)
        {
            Properties = PropertyItem.ListCurve3dProperties(curve);
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Properties);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SubType.Name");
            view.GroupDescriptions.Add(groupDescription);
        }

        public void SetFitDataProperties(FitData data)
        {
            Properties = PropertyItem.ListFitDataProperties(data);
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Properties);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SubType.Name");
            view.GroupDescriptions.Add(groupDescription);
        }

        public void SetNurbsDataProperties(NurbsData data)
        {
            Properties = PropertyItem.ListNurbsDataProperties(data);
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Properties);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SubType.Name");
            view.GroupDescriptions.Add(groupDescription);
        }

        public  void SetSplineProperties(Spline spline)
        {
            Properties = PropertyItem.ListDBObjectProperties(spline);
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Properties);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SubType.Name");
            view.GroupDescriptions.Add(groupDescription);
        }

        public void SetResultBufferProperties(ResultBuffer resbuf) => Properties = PropertyItem.ListResultBufferProperties(resbuf);

        public void SetPoint3dCollectionProperties(Point3dCollection points) => Properties = PropertyItem.ListPoint3dCollectionProperties(points);

        public void SetDoubleCollectionProperties(DoubleCollection doubles) => Properties = PropertyItem.ListDoubleCollectionProperties(doubles);

        public void SetProperties<T>(T item) => Properties = PropertyItem.ListProperties(item);
        #endregion
    }
}
