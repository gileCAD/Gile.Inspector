using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

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
                    else if (value.Value is ResultBuffer rb)
                        ShowDialog(rb);
                    else if (value.Value is Matrix3d mx)
                        ShowDialog(mx);
                    else if (value.Value is Extents3d ex)
                        ShowDialog(ex);
                    else if (value.Value is CoordinateSystem3d cs)
                        ShowDialog(cs);
                    else if (value.Value is AcDb.AttributeCollection at)
                        ShowDialog(at);
                    else if (value.Value is DynamicBlockReferencePropertyCollection dp)
                        ShowDialog(dp);
                    else if (value.Value is EntityColor ec)
                        ShowDialog(ec);
                    else if (value.Value is Color co)
                        ShowDialog(co);
                }
            }
        }
        #endregion

        #region ShowDialog methods
        public static void ShowDialog(Database db) => Show(new InspectorViewModel(db));

        public static void ShowDialog(ObjectId id) => Show(new InspectorViewModel(id));

        public static void ShowDialog(ResultBuffer resbuf) => Show(new InspectorViewModel(resbuf));

        public static void ShowDialog(ObjectIdCollection ids) => Show(new InspectorViewModel(ids));

        public static void ShowDialog(AcDb.AttributeCollection at) => Show(new InspectorViewModel(at));

        public static void ShowDialog(DynamicBlockReferencePropertyCollection props) => Show(new InspectorViewModel(props));

        public static void ShowDialog(Matrix3d matrix) => Show(new InspectorViewModel(matrix));

        public static void ShowDialog(Extents3d extents) => Show(new InspectorViewModel(extents));

        public static void ShowDialog(CoordinateSystem3d cs) => Show(new InspectorViewModel(cs));

        public static void ShowDialog(EntityColor co) => Show(new InspectorViewModel(co));

        public static void ShowDialog(Color co) => Show(new InspectorViewModel(co));

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

        public void SetResultBufferProperties(ResultBuffer resbuf) => Properties = PropertyItem.ListResultBufferProperties(resbuf);

        public void SetProperties<T>(T item) => Properties = PropertyItem.ListProperties(item);
        #endregion
    }
}
