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
using Autodesk.AutoCAD.LayerManager;
using System.Collections;

namespace Gile.AutoCAD.Inspector
{
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
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetProperties(db);
        }

        public InspectorViewModel(ObjectId id)
        {
            var item = new InspectableItem(id);
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetObjectIdProperties(id);
        }

        public InspectorViewModel(ResultBuffer resbuf)
        {
            var item = new InspectableItem(resbuf);
            ItemTree = new [] { item };
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
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetProperties(matrix);
        }

        public InspectorViewModel(Extents3d extents)
        {
            var item = new InspectableItem(extents);
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetProperties(extents);
        }

        public InspectorViewModel(CoordinateSystem3d cs)
        {
            var item = new InspectableItem(cs);
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetProperties(cs);
        }

        public InspectorViewModel(EntityColor co)
        {
            var item = new InspectableItem(co);
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetProperties(co);
        }

        public InspectorViewModel(Color co)
        {
            var item = new InspectableItem(co);
            ItemTree = new [] { item };
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
            ItemTree = new [] { item };
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
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetFitDataProperties(fitData);
        }

        public InspectorViewModel(NurbsData nurbsData)
        {
            var item = new InspectableItem(nurbsData);
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetNurbsDataProperties(nurbsData);
        }

        public InspectorViewModel(Point3dCollection points)
        {
            var item = new InspectableItem(points);
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetProperties(points);
        }

        public InspectorViewModel(DoubleCollection doubles)
        {
            var item = new InspectableItem(doubles);
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetProperties(doubles);
        }

        public InspectorViewModel(Spline spline)
        {
            var item = new InspectableItem(spline);
            ItemTree = new [] { item };
            item.IsSelected = true;
            SetSplineProperties(spline);
        }

        public InspectorViewModel(LayerFilterTree filterTree)
        {
            var item = new InspectableItem(filterTree.Root);
            ItemTree = new[] { item };
            item.IsSelected = true;
            SetProperties(filterTree.Root);
        }

        public InspectorViewModel(LayerFilterCollection filters)
        {
            var item = new InspectableItem(filters[0]);
            ItemTree = filters.Cast<LayerFilter>().Select(f => new InspectableItem(f));
            item.IsSelected = true;
            SetLayerFilterProperties(filters[0]);
        }

        public InspectorViewModel(LayerFilter filter)
        {
            var item = new InspectableItem(filter);
            ItemTree = filter.NestedFilters.Cast<LayerFilter>().Select(f => new InspectableItem(f));
            item.IsSelected = true;
            SetProperties(filter);
        }

        public InspectorViewModel(LayerFilterDisplayImages images)
        {
            var item = new InspectableItem(images);
            ItemTree = new[] { item };
            item.IsSelected = true;
            SetProperties(images);
        }

        public InspectorViewModel(DatabaseSummaryInfo info)
        {
            var item = new InspectableItem(info);
            ItemTree = new[] { item };
            item.IsSelected = true;
            SetProperties(info);
        }

        public InspectorViewModel(Dictionary<string, string>.Enumerator dict)
        {
            var item = new InspectableItem(dict);
            ItemTree = new[] { item };
            item.IsSelected = true;
            SetDictEnumProperties(dict);
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
            set { properties = value; NotifyPropertyChanged(nameof(Properties)); }
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
                SetObjectIdProperties(item.ObjectId);
            else if (item.DynamicProperty != null)
                SetProperties(item.DynamicProperty);
            else if (item.ResultBuffer != null)
                SetResultBufferProperties(item.ResultBuffer);
            else if (item.PolylineVertex != null)
                SetProperties(item.PolylineVertex);
            else if (item.Points != null)
                SetPoint3dCollectionProperties(item.Points);
            else if (item.Doubles != null)
                SetDoubleCollectionProperties(item.Doubles);
            else if (item.LayerFilter != null)
                SetLayerFilterProperties(item.LayerFilter);
        }

        private void SetObjectIdProperties(ObjectId id)
        {
            Properties = PropertyItem.ListObjectIdProperties(id);
            SetSubTypeGroups();
        }

        private void SetCurve3dProperties(Entity3d curve)
        {
            Properties = PropertyItem.ListCurve3dProperties(curve);
            SetSubTypeGroups();
        }

        private void SetFitDataProperties(FitData data)
        {
            Properties = PropertyItem.ListFitDataProperties(data);
            SetSubTypeGroups();
        }

        private void SetNurbsDataProperties(NurbsData data)
        {
            Properties = PropertyItem.ListNurbsDataProperties(data);
            SetSubTypeGroups();
        }

        private void SetSplineProperties(Spline spline)
        {
            Properties = PropertyItem.ListDBObjectProperties(spline);
            SetSubTypeGroups();
        }

        private void SetSubTypeGroups()
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Properties);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SubType.Name");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void SetResultBufferProperties(ResultBuffer resbuf) => Properties = PropertyItem.ListResultBufferProperties(resbuf);

        private void SetPoint3dCollectionProperties(Point3dCollection points) => Properties = PropertyItem.ListPoint3dCollectionProperties(points);

        private void SetDoubleCollectionProperties(DoubleCollection doubles) => Properties = PropertyItem.ListDoubleCollectionProperties(doubles);

        public void SetDictEnumProperties(Dictionary<string, string>.Enumerator dict) => Properties = PropertyItem.ListDictEnumProperties(dict);

        public void SetLayerFilterProperties(LayerFilter filter) => Properties = PropertyItem.ListLayerFilterProperties(filter);

        private void SetProperties<T>(T item) => Properties = PropertyItem.ListProperties(item);
        #endregion
    }
}
