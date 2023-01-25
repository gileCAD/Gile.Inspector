using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.LayerManager;
using Autodesk.AutoCAD.Runtime;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private IEnumerable<InspectableItem> items;
        private readonly Entity brepSurf;
        private readonly Entity brepSolid;
        private readonly List<DBObject> toDispose = new List<DBObject>();

        #region INotitfyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new intance of InspectorViewModel.
        /// Set the the values of ItemTree.
        /// </summary>
        /// <param name="value">Object to be inspected.</param>
        public InspectorViewModel(object value)
        {
            var type = value.GetType();
            if (type.Namespace.StartsWith("Autodesk.AutoCAD"))
            {
                switch (value)
                {
                    case ObjectIdCollection ids:
                        items = ids.Cast<ObjectId>().Select(id => new InspectableItem(id, name: "<_>"));
                        break;
                    case LayerFilter filter:
                        items = filter.NestedFilters.Cast<LayerFilter>().Select(f => new InspectableItem(f));
                        break;
                    case CellBorders borders:
                        items = new[]
                        {
                            new InspectableItem(borders.Vertical, true, name: "Vertical"),
                            new InspectableItem(borders.Horizontal, name: "Horizontal"),
                            new InspectableItem(borders.Bottom, name: "Bottom"),
                            new InspectableItem(borders.Right, name: "Right"),
                            new InspectableItem(borders.Top, name: "Top"),
                            new InspectableItem(borders.Left, name: "Left")
                        };
                        break;
                    case EdgeRef[] edges:
                        items = edges.Select(e => new InspectableItem(e));
                        break;
                    case HatchLoopCollection loops:
                        items = loops.Loops.Select(l => new InspectableItem(l));
                        break;
                    case Curve2dCollection curves:
                        items = curves.Cast<Entity2d>().Select(e => new InspectableItem(e));
                        break;
                    case LayerFilterCollection collection: InitializeCollection<LayerFilter>(collection); break;
                    case AcDb.AttributeCollection collection: InitializeCollection<ObjectId>(collection); break;
                    case MlineStyleElementCollection collection: InitializeCollection<MlineStyleElement>(collection); break;
                    case DynamicBlockReferencePropertyCollection collection: InitializeCollection<DynamicBlockReferenceProperty>(collection); break;
                    case HyperLinkCollection collection: InitializeCollection<HyperLink>(collection); break;
                    case BulgeVertexCollection collection: InitializeCollection<BulgeVertex>(collection); break;
                    case IEnumerable<Entity> collection: InitializeIEnumerable(collection); break;
                    case IEnumerable<Profile3d> collection: InitializeIEnumerable(collection); break;
                    case RowsCollection collection: InitializeIEnumerable(collection); break;
                    case ColumnsCollection collection: InitializeIEnumerable(collection); break;
                    case BrepVertexCollection collection: InitializeIEnumerable(collection); break;
                    case BrepEdgeCollection collection: InitializeIEnumerable(collection); break;
                    case BrepFaceCollection collection: InitializeIEnumerable(collection); break;
                    case BrepComplexCollection collection: InitializeIEnumerable(collection); break;
                    case BrepShellCollection collection: InitializeIEnumerable(collection); break;
                    case FaceLoopCollection collection: InitializeIEnumerable(collection); break;
                    case EdgeLoopCollection collection: InitializeIEnumerable(collection); break;
                    case ComplexShellCollection collection: InitializeIEnumerable(collection); break;
                    case ShellFaceCollection collection: InitializeIEnumerable(collection); break;
                    case LoopVertexCollection collection: InitializeIEnumerable(collection); break;
                    case LoopEdgeCollection collection: InitializeIEnumerable(collection); break;
                    case VertexEdgeCollection collection: InitializeIEnumerable(collection); break;
                    case VertexLoopCollection collection: InitializeIEnumerable(collection); break;
                    case ObjectId id: InitializeSingle(id); break;
                    case DBObject dbObj: InitializeSingle(dbObj); break;
                    case Dictionary<string, string>.Enumerator dict: InitializeSingle(dict); break;
                    case Point2dCollection pts: InitializeSingle(pts); break;
                    case MlineVertices vertices: InitializeSingle(vertices.Vertices); break;
                    case LayerFilterTree filterTree: InitializeSingle(filterTree.Root); break;
                    case Brep brep:
                        brepSolid = brep.Solid;
                        brepSurf = brep.Surf;
                        var item = new InspectableItem(
                            brep,
                            true,
                            true,
                            brep.Complexes
                            .Select(c => new InspectableItem(c, children: c.Shells
                                .Select(s => new InspectableItem(s, children: s.Faces
                                    .Select(f => new InspectableItem(f, children: f.Loops
                                        .Select(l => new InspectableItem(l, children: l.Edges
                                            .Select(e => new InspectableItem(e)))))))))));
                        items = new[] { item };
                        break;
                    default: InitializeSingle(value); break;
                }

            }
            else if (type.Namespace == "Gile.AutoCAD.Inspector")
            {
                switch (value)
                {
                    case PolylineVertices vertices:
                        items = vertices.Vertices.Select(v => new InspectableItem(v));
                        break;
                    case Polyline3dVertices vertices:
                        items = vertices.Vertices.Cast<DBObject>().Select(obj => new InspectableItem(obj));
                        break;
                    case Polyline2dVertices vertices:
                        InitializeSingle(vertices.Vertices);
                        break;
                    case HatchLoopCollection loops:
                        items = loops.Loops.Select(l => new InspectableItem(l));
                        break;
                    case ViewportCollection viewports:
                        items = viewports.Viewports.Cast<ObjectId>().Select(id => new InspectableItem(id));
                        break;
                    case SplineControlPoints points:
                        InitializeSingle(points.ControlPoints);
                        break;
                    case SplineFitPoints points:
                        InitializeSingle(points.FitPoints);
                        break;
                    case IReferences references:
                        IEnumerable<InspectableItem> getChildren(ObjectIdCollection ids) =>
                            ids
                            .Cast<ObjectId>()
                            .Select(id => new InspectableItem(id, name: "<_>"));
                        items =
                            new[]
                            {
                                new InspectableItem(references.HardPointerIds, true, true, getChildren(references.HardPointerIds), name: "Hard pointer"),
                                new InspectableItem(references.SoftPointerIds, false, true, getChildren(references.SoftPointerIds), name: "Soft pointer"),
                                new InspectableItem(references.HardOwnershipIds, false, true, getChildren(references.HardOwnershipIds), name: "Hard ownership"),
                                new InspectableItem(references.SoftOwnershipIds, false, true, getChildren(references.SoftOwnershipIds), name: "Soft ownership"),
                            };
                        break;
                    case MlineVertices vertices: InitializeSingle(vertices.Vertices); break;
                    default:
                        break;
                }
            }
            else
            {
                if (value is Dictionary<string, string>.Enumerator dict)
                    InitializeSingle(dict);
            }
            if (items.Any())
            {
                ItemTree = new ObservableCollection<InspectableItem>(items);
                ItemTree.First().IsSelected = true;
            }
        }

        private void InitializeCollection<T>(ICollection collection)
        {
            items = collection.Cast<T>().Select(x => new InspectableItem(x));
        }

        private void InitializeIEnumerable<T>(IEnumerable<T> collection)
        {
            items = collection.Select(e => new InspectableItem(e));
        }

        private void InitializeSingle<T>(T value)
        {
            var item = new InspectableItem(value, true, true);
            items = new[] { item };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the items to be displayed in the TreeView.
        /// </summary>
        public ObservableCollection<InspectableItem> ItemTree
        {
            get { return new ObservableCollection<InspectableItem>(inspectables); }
            set { inspectables = value; NotifyPropertyChanged(nameof(ItemTree)); }
        }

        /// <summary>
        /// Gets or sets the items to be displayed in the ListView.
        /// </summary>
        public IEnumerable<PropertyItem> Properties
        {
            get { return properties; }
            set
            {
                properties = value;
                NotifyPropertyChanged(nameof(Properties));
            }
        }

        /// <summary>
        /// Handles the SelectedItem event of the ListView.
        /// </summary>
        public PropertyItem SelectedProperty
        {
            get { return null; }
            set
            {
                if (value != null && value.IsInspectable)
                {
                    new InspectorViewModel(value.Value)?.ShowDialog();
                }
            }
        }
        #endregion

        /// <summary>
        /// Shows a new InpectorDialog window bounded to the current instance.
        /// </summary>
        public void ShowDialog() => AcAp.ShowModalWindow(new InspectorDialog(this));

        /// <summary>
        /// Handles the TreeView_SelectedItemChanged event.
        /// </summary>
        /// <param name="obj">e.NewValue</param>
        public void SetProperties(object obj)
        {
            var item = (InspectableItem)obj;
            switch (item.Value)
            {
                case ObjectId id when !id.IsNull: Properties = ListObjectIdProperties(id); break;
                case DBObject dBObject: Properties = ListDBObjectProperties(dBObject); break;
                case ResultBuffer resbuf: Properties = ListResultBufferProperties(resbuf); break;
                case DoubleCollection doubles: Properties = ListCollection(doubles); break;
                case Point3dCollection points: Properties = ListCollection(points); break;
                case Point2dCollection points: Properties = ListCollection(points); break;
                case LayerFilter filter: Properties = ListLayerFilterProperties(filter); break;
                case Dictionary<string, string>.Enumerator dict: Properties = ListDictEnumProperties(dict); break;
                default: Properties = ListProperties(item.Value); break;
            }
        }

        /// <summary>
        /// Handles the WindowClosing event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            brepSolid?.Dispose();
            brepSurf?.Dispose();
            foreach (var item in toDispose)
            {
                if (!item.IsDisposed)
                    item.Dispose();
            }
        }

        #region ListProperties methods
        private IEnumerable<PropertyItem> ListObjectIdProperties(ObjectId id)
        {
            if (id.IsNull)
                throw new ArgumentNullException("id");
            yield return new PropertyItem("Name", id.ObjectClass.Name, typeof(RXClass), false);
            yield return new PropertyItem("DxfName", id.ObjectClass.DxfName, typeof(RXClass), false);
            using (var tr = id.Database.TransactionManager.StartTransaction())
            {
                var dbObj = tr.GetObject(id, OpenMode.ForRead);
                foreach (var item in ListDBObjectProperties(dbObj))
                {
                    yield return item;
                }
                yield return new PropertyItem("References to", new ReferencesTo(id), typeof(DBObject), true);
                yield return new PropertyItem("Referenced by", new ReferencedBy(id), typeof(DBObject), true);
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
                    if (name == "Item") continue;
                    if (name == "OwnerId" && dbObj.Handle == default(Handle))
                    {
                        yield return new PropertyItem(name, "(Null)", subType, false);
                        continue;
                    }
                    object value;
                    bool isInspectable = true;
                    // From Jeff_M http://www.theswamp.org/index.php?topic=57317.msg608371#msg608371
                    var obsAtt = prop.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "ObsoleteAttribute");
                    if (obsAtt != null)
                    {
                        value = obsAtt.ConstructorArguments.FirstOrDefault().Value;
                    }
                    else
                    {
                        try { value = prop.GetValue(dbObj, null) ?? "(Null)"; }
                        catch (System.Exception e) { value = e.Message; isInspectable = false; }
                    }
                    if (value is DBObject dbo && dbo.Handle == default)
                        toDispose.Add(dbo);
                    isInspectable =
                        isInspectable &&
                        CheckIsInspectable(value) &&
                        !((value is ObjectId id) && id == dbObj.ObjectId) &&
                        !((value is DBObject obj) && obj.GetType() == dbObj.GetType() && obj.Handle == dbObj.Handle);
                    yield return new PropertyItem(name, value, subType, isInspectable);
                }
            }
            if (dbObj is Spline spline)
            {
                yield return new PropertyItem("Control points", new SplineControlPoints(spline), typeof(Spline), 0 < spline.NumControlPoints);
                yield return new PropertyItem("Fit points", new SplineFitPoints(spline), typeof(Spline), 0 < spline.NumFitPoints);
            }
            else if (dbObj is AcDb.Polyline pl)
                yield return new PropertyItem("Vertices", new PolylineVertices(pl), typeof(AcDb.Polyline), true);
            else if (dbObj is Polyline3d pl3d)
                yield return new PropertyItem("Vertices", new Polyline3dVertices(pl3d), typeof(Polyline3d), true);
            else if (dbObj is Polyline2d pl2d)
                yield return new PropertyItem("Vertices", new Polyline2dVertices(pl2d), typeof(Polyline2d), true);
            else if (dbObj is Mline mline)
                yield return new PropertyItem("Vertices", new MlineVertices(mline), typeof(Mline), true);
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
            if (dbObj is Hatch hatch)
            {
                yield return new PropertyItem("Hatch Loops", new HatchLoopCollection(hatch), typeof(Hatch), true);
            }
            if (dbObj is Layout layout)
            {
                var vpCol = new ViewportCollection(layout);
                yield return new PropertyItem("Viewports", vpCol, typeof(Layout), 0 < vpCol.Viewports.Count);
            }
            if (dbObj is Region || dbObj is Solid3d || dbObj is AcDb.Surface)
            {
                yield return new PropertyItem("Boundary representation", new Brep((Entity)dbObj), dbObj.GetType(), true);
            }
        }

        private IEnumerable<PropertyItem> ListLayerFilterProperties(LayerFilter layerFilter)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var prop in typeof(LayerFilter).GetProperties(flags))
            {
                string name = prop.Name;
                bool isInspectable = true;
                object value;
                try { value = prop.GetValue(layerFilter, null) ?? "(Null)"; }
                catch (System.Exception e) { value = e.Message; isInspectable = false; }
                isInspectable = isInspectable && CheckIsInspectable(value);
                if (name == "Parent" && value is LayerFilter f)
                    yield return new PropertyItem(name, f.Parent, typeof(LayerFilter), isInspectable && f.Parent != null);
                else
                    yield return new PropertyItem(name, value, typeof(LayerFilter), isInspectable);
            }
        }

        private IEnumerable<PropertyItem> ListProperties(object item)
        {
            var types = new List<Type>();
            var type = item.GetType();
            while (type != null && type != typeof(DisposableWrapper))
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
                    if (prop.Name == "Item") continue;
                    string name = prop.Name;
                    if (name == "Item") continue;
                    if (item is Brep && (name == "Surf" || name == "Solid")) continue;
                    if (item is DynamicBlockReferenceProperty && name == "Value") continue;
                    object value;
                    bool isInspectable = true;
                    try { value = prop.GetValue(item, null) ?? "(Null)"; }
                    catch (System.Exception e) { value = e.Message; isInspectable = false; }
                    isInspectable = isInspectable && !item.Equals(value) && CheckIsInspectable(value);
                    yield return new PropertyItem(name, value, subType, isInspectable);
                }
            }
            if (item is FitData fitData)
            {
                var fitPoints = fitData.GetFitPoints();
                yield return new PropertyItem("FitPoints", fitPoints, typeof(FitData), 0 < fitPoints.Count);
            }
            else if (item is NurbsData nurbsData)
            {
                var controlPoints = nurbsData.GetControlPoints();
                var knots = nurbsData.GetKnots();
                var weights = nurbsData.GetWeights();
                yield return new PropertyItem("FitPoints", controlPoints, typeof(NurbsData), 0 < controlPoints.Count);
                yield return new PropertyItem("Knots", knots, typeof(NurbsData), 0 < knots.Count);
                yield return new PropertyItem("Weights", weights, typeof(NurbsData), 0 < weights.Count);
            }
            else if (item is KnotCollection knotCollection && 0 < knotCollection.Count)
            {
                var knots = new DoubleCollection();
                for (int i = 0; i < knotCollection.Count; i++)
                {
                    knots.Add(knotCollection[i]);
                }
                yield return new PropertyItem("Knots", knots, typeof(KnotCollection), true);
            }
        }

        private IEnumerable<PropertyItem> ListResultBufferProperties(ResultBuffer resbuf) =>
            resbuf.Cast<TypedValue>().Select(tv => new PropertyItem(tv.TypeCode.ToString(), tv.Value, typeof(ResultBuffer), CheckIsInspectable(tv.Value)));

        private IEnumerable<PropertyItem> ListCollection(IList collection)
        {
            bool isInspectable = CheckIsInspectable(collection[0]);
            for (int i = 0; i < collection.Count; i++)
            {
                yield return new PropertyItem($"[{i}]", collection[i], collection.GetType(), isInspectable);
            }
        }

        private IEnumerable<PropertyItem> ListDictEnumProperties(Dictionary<string, string>.Enumerator dictEnum)
        {
            while (dictEnum.MoveNext())
            {
                yield return new PropertyItem(dictEnum.Current.Key, dictEnum.Current.Value, typeof(Dictionary<string, string>.Enumerator), false);
            }
        }

        private static bool CheckIsInspectable(object value)
        {
            if (value is Dictionary<string, string>.Enumerator dictEnum && dictEnum.MoveNext())
                return true;
            var type = value.GetType();
            if (!type.Namespace.StartsWith("Autodesk.AutoCAD") && type.Namespace != "Gile.AutoCAD.Inspector")
                return false;
            if (value is ObjectId id && id.IsNull)
            {
                return false;
            }
            if (value is Handle handle && handle == default) 
                return false;  
            if (type.IsClass && value == null)
                return false;
            if (value is IEnumerable collection && !collection.GetEnumerator().MoveNext())
                return false;
            if (type.IsPrimitive || value is Enum || value is Point2d || value is Point3d || value is Vector2d || value is Vector3d)
                return false;
            return true;
        }
        #endregion
    }
}
