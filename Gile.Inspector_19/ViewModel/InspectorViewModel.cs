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
using System.Data.Common;
using System.Linq;
using System.Reflection;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using AcDb = Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Interaction logic for InspectorDialog.xaml
    /// </summary>
    public class InspectorViewModel : INotifyPropertyChanged
    {
        private IEnumerable<PropertyItem> properties;
        private IEnumerable<InspectableItem> inspectables;
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
            var items = Enumerable.Empty<InspectableItem>();

            IEnumerable<InspectableItem> fromICollection<T>(ICollection collection) =>
                collection.Cast<T>().Select(x => new InspectableItem(x));

            IEnumerable<InspectableItem> fromIEnumerable<T>(IEnumerable<T> sequence) =>
                sequence.Select(e => new InspectableItem(e));

            IEnumerable<InspectableItem> fromObject<T>(T obj) =>
                new[] { new InspectableItem(obj, true, true) };

            var type = value.GetType();
            if (type.Namespace.StartsWith("Autodesk.AutoCAD"))
            {
                switch (value)
                {
                    case ObjectId id: items = fromObject(id); break;
                    case DBObject dbObj: items = fromObject(dbObj); break;
                    case Point2dCollection pts: items = fromObject(pts); break;
                    case LayerFilterTree filterTree: items = fromObject(filterTree.Root); break;
                    case LayerFilter filter: items = fromICollection<LayerFilter>(filter.NestedFilters); break;
                    case LayerFilterCollection collection: items = fromICollection<LayerFilter>(collection); break;
                    case Curve2dCollection collection: items = fromICollection<Entity2d>(collection); break;
                    case AcDb.AttributeCollection collection: items = fromICollection<ObjectId>(collection); break;
                    case MlineStyleElementCollection collection: items = fromICollection<MlineStyleElement>(collection); break;
                    case DynamicBlockReferencePropertyCollection collection: items = fromICollection<DynamicBlockReferenceProperty>(collection); break;
                    case HyperLinkCollection collection: items = fromICollection<HyperLink>(collection); break;
                    case BulgeVertexCollection collection: items = fromICollection<BulgeVertex>(collection); break;
                    case IEnumerable<DBObject> sequence: items = fromIEnumerable(sequence); break;
                    case EdgeRef[] edges: items = fromIEnumerable(edges); break;
                    case RowsCollection sequence: items = fromIEnumerable(sequence); break;
                    case ColumnsCollection sequence: items = fromIEnumerable(sequence); break;
                    case BrepVertexCollection sequence: items = fromIEnumerable(sequence); break;
                    case BrepEdgeCollection sequence: items = fromIEnumerable(sequence); break;
                    case BrepFaceCollection sequence: items = fromIEnumerable(sequence); break;
                    case BrepComplexCollection sequence: items = fromIEnumerable(sequence); break;
                    case BrepShellCollection sequence: items = fromIEnumerable(sequence); break;
                    case FaceLoopCollection sequence: items = fromIEnumerable(sequence); break;
                    case EdgeLoopCollection sequence: items = fromIEnumerable(sequence); break;
                    case ComplexShellCollection sequence: items = fromIEnumerable(sequence); break;
                    case ShellFaceCollection sequence: items = fromIEnumerable(sequence); break;
                    case LoopVertexCollection sequence: items = fromIEnumerable(sequence); break;
                    case LoopEdgeCollection sequence: items = fromIEnumerable(sequence); break;
                    case VertexEdgeCollection sequence: items = fromIEnumerable(sequence); break;
                    case VertexLoopCollection sequence: items = fromIEnumerable(sequence); break;
                    case ObjectIdCollection ids:
                        items = ids.Cast<ObjectId>().Select(id => new InspectableItem(id, name: "<_>"));
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
                    default: items = fromObject(value); break;
                }
            }
            else if (type.Namespace == "Gile.AutoCAD.R19.Inspector")
            {
                switch (value)
                {
                    case MlineVertices vertices: items = fromObject(vertices.Vertices); break;
                    case SplineControlPoints points: items = fromObject(points.ControlPoints); break;
                    case SplineFitPoints points: items = fromObject(points.FitPoints); break;
                    case HatchLoopCollection loops: items = fromIEnumerable(loops.Loops); break;
                    case DataColumnCollection columns: items = fromIEnumerable(columns.Columns); break;
                    case DataRowCollection rows: items = fromIEnumerable(rows.Rows); break;
                    case DataCellCollection cells: items = fromIEnumerable(cells.Cells); break;
                    case PolylineVertices vertices: items = fromIEnumerable(vertices.Vertices); break;
                    case Polyline3dVertices vertices: items = fromICollection<DBObject>(vertices.Vertices); break;
                    case Polyline2dVertices vertices: items = fromICollection<DBObject>(vertices.Vertices); break;
                    case ViewportCollection viewports: items = fromICollection<ObjectId>(viewports.Viewports); break;
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
                    default: break;
                }
            }
            else
            {
                switch (value)
                {
                    case IEnumerable<Entity> collection: items = fromIEnumerable(collection); break;
                    case IEnumerable<Profile3d> collection: items = fromIEnumerable(collection); break;
                    case Dictionary<string, string>.Enumerator dict: items = fromObject(dict); break;
                    case object[] objs: items = fromObject(objs); break;
                    default: break;
                }
            }

            ItemTree = new ObservableCollection<InspectableItem>(items);
            if (items.Any())
                ItemTree.First().IsSelected = true;
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
                case object[] objs: Properties = ListCollection(objs); break;
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
                    if (name == "OwnerId" && dbObj.Handle == default)
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
                        if (prop.DeclaringType.Name == "MPolygon" && prop.Name == "PatternColor")
                        {
                            value = new AccessViolationException().Message;
                        }
                        else
                        {
                            try { value = prop.GetValue(dbObj, null) ?? "(Null)"; }
                            catch (System.Exception e) { value = e.Message; isInspectable = false; }
                        }
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
            ObjectIdCollection ids;
            switch (dbObj)
            {
                case Spline spline:
                    yield return new PropertyItem("Control points", new SplineControlPoints(spline), typeof(Spline), 0 < spline.NumControlPoints);
                    yield return new PropertyItem("Fit points", new SplineFitPoints(spline), typeof(Spline), 0 < spline.NumFitPoints);
                    break;
                case AcDb.Polyline pl:
                    yield return new PropertyItem("Vertices", new PolylineVertices(pl), typeof(AcDb.Polyline), true);
                    break;
                case Polyline3d pl3d:
                    yield return new PropertyItem("Vertices", new Polyline3dVertices(pl3d), typeof(Polyline3d), true);
                    break;
                case Polyline2d pl2d:
                    yield return new PropertyItem("Vertices", new Polyline2dVertices(pl2d), typeof(Polyline2d), true);
                    break;
                case Mline mline:
                    yield return new PropertyItem("Vertices", new MlineVertices(mline), typeof(Mline), true);
                    break;
                case BlockTableRecord btr:
                    ids = new ObjectIdCollection();
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
                    break;
                case Hatch hatch:
                    yield return new PropertyItem("Hatch Loops", new HatchLoopCollection(hatch), typeof(Hatch), true);
                    break;
                case DataTable table:
                    yield return new PropertyItem("Columns", new DataColumnCollection(table), typeof(DataTable), true);
                    yield return new PropertyItem("Rows", new DataRowCollection(table), typeof(DataTable), true);
                    break;
                case Layout layout:
                    var vpCol = new ViewportCollection(layout);
                    yield return new PropertyItem("Viewports", vpCol, typeof(Layout), 0 < vpCol.Viewports.Count);
                    break;
                case Region _:
                case Solid3d _:
                case AcDb.Surface _:
                    var fullSubentityPath = new FullSubentityPath(new[] { dbObj.ObjectId }, new SubentityId(SubentityType.Null, IntPtr.Zero));
                    yield return new PropertyItem("Boundary representation", new Brep(fullSubentityPath), dbObj.GetType(), true);
                    break;
                case Group group:
                    ids = new ObjectIdCollection();
                    foreach (var id in group.GetAllEntityIds())
                    {
                        ids.Add(id);
                    }
                    yield return new PropertyItem("Entities within group", ids, typeof(Group), 0 < ids.Count);
                    break;
                default:
                    break;
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
            switch (item)
            {
                case FitData fitData:
                    var fitPoints = fitData.GetFitPoints();
                    yield return new PropertyItem("FitPoints", fitPoints, typeof(FitData), 0 < fitPoints.Count);
                    break;
                case NurbsData nurbsData:
                    var controlPoints = nurbsData.GetControlPoints();
                    var knots = nurbsData.GetKnots();
                    var weights = nurbsData.GetWeights();
                    yield return new PropertyItem("ControlPoints", controlPoints, typeof(NurbsData), 0 < controlPoints.Count);
                    yield return new PropertyItem("Knots", knots, typeof(NurbsData), 0 < knots.Count);
                    yield return new PropertyItem("Weights", weights, typeof(NurbsData), 0 < weights.Count);
                    break;
                case KnotCollection knotCollection when 0 < knotCollection.Count:
                    var knotCol = new DoubleCollection();
                    for (int i = 0; i < knotCollection.Count; i++)
                    {
                        knotCol.Add(knotCollection[i]);
                    }
                    yield return new PropertyItem("Knots", knotCol, typeof(KnotCollection), true);
                    break;
                case DynamicBlockReferenceProperty dynProp:
                    var allowedValues = dynProp.GetAllowedValues();
                    yield return new PropertyItem(
                        "AllowedValues", allowedValues, typeof(DynamicBlockReferenceProperty), 0 < allowedValues.Length);
                    break;
                case DataColumn column:
                    var cells = new DataCellCollection(column);
                    yield return new PropertyItem("Cells", cells, typeof(DataColumn), true);
                    break;
                default:
                    break;
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
            if (value == null)
                return false;
            var type = value.GetType();
            string nameSpace = type.Namespace;
            return
                nameSpace != null &&
                (nameSpace.StartsWith("Autodesk.AutoCAD") || nameSpace == "Gile.AutoCAD.R19.Inspector") &&
                !type.IsPrimitive &&
                !(value is string) &&
                !(value is Enum) &&
                !(value is Point2d) &&
                !(value is Point3d) &&
                !(value is Vector2d) &&
                !(value is Vector3d) &&
                !(value is ObjectId id && id.IsNull) &&
                !(value is Handle handle && handle == default) &&
                !(value is Dictionary<string, string>.Enumerator dictEnum && !dictEnum.MoveNext()) &&
                !(value is IEnumerable collection && !collection.GetEnumerator().MoveNext());
        }
        #endregion
    }
}
