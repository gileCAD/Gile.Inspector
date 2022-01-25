using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.LayerManager;
using Autodesk.AutoCAD.Runtime;

using System;
using System.Collections;
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
            var item = new InspectableItem(db) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(db);
        }

        public InspectorViewModel(ObjectId id)
        {
            var item = new InspectableItem(id) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListObjectIdProperties(id);
        }

        public InspectorViewModel(ResultBuffer resbuf)
        {
            var item = new InspectableItem(resbuf) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(resbuf);
        }

        public InspectorViewModel(ObjectIdCollection ids)
        {
            var item = new InspectableItem(ids[0]) { IsSelected = true };
            ItemTree = ids.Cast<ObjectId>().Select(id => new InspectableItem(id));
            Properties = ListObjectIdProperties(ids[0]);
        }

        public InspectorViewModel(AcDb.AttributeCollection attribs)
        {
            var item = new InspectableItem(attribs[0]) { IsSelected = true };
            ItemTree = attribs.Cast<ObjectId>().Select(id => new InspectableItem(id));
            Properties = ListObjectIdProperties(attribs[0]);
        }

        public InspectorViewModel(DynamicBlockReferencePropertyCollection props)
        {
            var item = new InspectableItem(props[0]) { IsSelected = true };
            ItemTree = props.Cast<DynamicBlockReferenceProperty>().Select(p => new InspectableItem(p));
            Properties = ListProperties(props[0]);
        }

        public InspectorViewModel(Matrix3d matrix)
        {
            var item = new InspectableItem(matrix) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(matrix);
        }

        public InspectorViewModel(Extents3d extents)
        {
            var item = new InspectableItem(extents) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(extents);
        }

        public InspectorViewModel(Extents2d extents)
        {
            var item = new InspectableItem(extents) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(extents);
        }

        public InspectorViewModel(CoordinateSystem3d cs)
        {
            var item = new InspectableItem(cs) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(cs);
        }

        public InspectorViewModel(EntityColor co)
        {
            var item = new InspectableItem(co) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(co);
        }

        public InspectorViewModel(Color co)
        {
            var item = new InspectableItem(co) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(co);
        }

        public InspectorViewModel(PolylineVertices vertices)
        {
            var item = new InspectableItem(vertices.Vertices[0]) { IsSelected = true };
            ItemTree = vertices.Vertices.Select(v => new InspectableItem(v));
            Properties = ListProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(Entity3d entity3d)
        {
            var item = new InspectableItem(entity3d) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(entity3d, typeof(DisposableWrapper));
        }

        public InspectorViewModel(Polyline3dVertices vertices)
        {
            var item = new InspectableItem(vertices.Vertices[0]) { IsSelected = true };
            ItemTree = vertices.Vertices.Cast<ObjectId>().Select(id => new InspectableItem(id));
            Properties = ListObjectIdProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(Polyline2dVertices vertices)
        {
            var item = new InspectableItem(vertices.Vertices[0]) { IsSelected = true };
            ItemTree = vertices.Vertices.Cast<ObjectId>().Select(id => new InspectableItem(id));
            Properties = ListObjectIdProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(FitData fitData)
        {
            var item = new InspectableItem(fitData) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListFitDataProperties(fitData);
        }

        public InspectorViewModel(NurbsData nurbsData)
        {
            var item = new InspectableItem(nurbsData) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListNurbsDataProperties(nurbsData);
        }

        public InspectorViewModel(Point3dCollection points)
        {
            var item = new InspectableItem(points) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(points);
        }

        public InspectorViewModel(DoubleCollection doubles)
        {
            var item = new InspectableItem(doubles) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(doubles);
        }

        public InspectorViewModel(DBObject dbObject)
        {
            var item = new InspectableItem(dbObject) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListDBObjectProperties(dbObject);
        }

        public InspectorViewModel(LayerFilterTree filterTree)
        {
            var item = new InspectableItem(filterTree.Root) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(filterTree.Root);
        }

        public InspectorViewModel(LayerFilterCollection filters)
        {
            var item = new InspectableItem(filters[0]) { IsSelected = true };
            ItemTree = filters.Cast<LayerFilter>().Select(f => new InspectableItem(f));
            Properties = ListLayerFilterProperties(filters[0]);
        }

        public InspectorViewModel(LayerFilter filter)
        {
            var item = new InspectableItem(filter) { IsSelected = true };
            ItemTree = filter.NestedFilters.Cast<LayerFilter>().Select(f => new InspectableItem(f));
            Properties = ListProperties(filter);
        }

        public InspectorViewModel(LayerFilterDisplayImages images)
        {
            var item = new InspectableItem(images) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(images);
        }

        public InspectorViewModel(DatabaseSummaryInfo info)
        {
            var item = new InspectableItem(info) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(info);
        }

        public InspectorViewModel(Dictionary<string, string>.Enumerator dict)
        {
            var item = new InspectableItem(dict) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListDictEnumProperties(dict);
        }

        public InspectorViewModel(AnnotationScale scale)
        {
            var item = new InspectableItem(scale) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(scale);
        }

        public InspectorViewModel(FontDescriptor font)
        {
            var item = new InspectableItem(font) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(font);
        }

        public InspectorViewModel(IReferences references)
        {
            var item = new InspectableItem("Hard pointer", references.HardPointerIds) { IsSelected = true };
            ItemTree = new[]
            {
                item,
                new InspectableItem("Soft pointer", references.SoftPointerIds),
                new InspectableItem("Hard ownership", references.HardOwnershipIds),
                new InspectableItem("Soft ownership", references.SoftOwnershipIds),
            };
            Properties = new PropertyItem[0];
        }

        public InspectorViewModel(Profile3d profile)
        {
            var item = new InspectableItem(profile);
            ItemTree = new[] { item };
            Properties = ListProperties(profile);
        }

        public InspectorViewModel(Entity[] entities)
        {
            var item = new InspectableItem(entities[0]);
            ItemTree = entities.Select(e => new InspectableItem(e));
            Properties = ListProperties(entities[0]);
        }

        public InspectorViewModel(Profile3d[] profiles)
        {
            var item = new InspectableItem(profiles[0]);
            ItemTree = profiles.Select(p => new InspectableItem(p));
            Properties = ListProperties(profiles[0]);
        }

        public InspectorViewModel(LoftOptions options)
        {
            var item = new InspectableItem(options);
            ItemTree = new[] { item };
            Properties = ListProperties(options);
        }

        public InspectorViewModel(SweepOptions options)
        {
            var item = new InspectableItem(options);
            ItemTree = new[] { item };
            Properties = ListProperties(options);
        }

        public InspectorViewModel(RevolveOptions options)
        {
            var item = new InspectableItem(options);
            ItemTree = new[] { item };
            Properties = ListProperties(options);
        }

        public InspectorViewModel(Solid3dMassProperties massProps)
        {
            var item = new InspectableItem(massProps);
            ItemTree = new[] { item };
            Properties = ListProperties(massProps);
        }

        public InspectorViewModel(MlineStyleElementCollection styles)
        {
            var item = new InspectableItem(styles[0]) { IsSelected = true };
            ItemTree = styles.Cast<MlineStyleElement>().Select(e => new InspectableItem(e));
            Properties = ListProperties(styles[0]);
        }

        public InspectorViewModel(MlineVertices vertices)
        {
            var item = new InspectableItem(vertices.Vertices) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(vertices.Vertices);
        }

        public InspectorViewModel(CellRange range)
        {
            var item = new InspectableItem(range);
            ItemTree = new[] { item };
            Properties = ListProperties(range);
        }

        public InspectorViewModel(CellBorders borders)
        {
            var item = new InspectableItem(borders.Vertical, "Vertical");
            ItemTree = new[]
            {
                item,
                new InspectableItem(borders.Horizontal, "Horizontal"),
                new InspectableItem(borders.Bottom, "Bottom"),
                new InspectableItem(borders.Right, "Right"),
                new InspectableItem(borders.Top, "Top"),
                new InspectableItem(borders.Left, "Left")
            };
            Properties = ListProperties(borders.Vertical);
        }

        public InspectorViewModel(DataTypeParameter param)
        {
            var item = new InspectableItem(param);
            ItemTree = new[] { item };
            Properties = ListProperties(param);
        }

        public InspectorViewModel(RowsCollection rows)
        {
            var item = new InspectableItem(rows[0]) { IsSelected = true };
            ItemTree = rows.Cast<Row>().Select(r => new InspectableItem(r));
            Properties = ListProperties(rows[0]);
        }

        public InspectorViewModel(ColumnsCollection columns)
        {
            var item = new InspectableItem(columns[0]) { IsSelected = true };
            ItemTree = columns.Cast<Column>().Select(r => new InspectableItem(r));
            Properties = ListProperties(columns[0]);
        }

        public InspectorViewModel(HyperLinkCollection links)
        {
            var item = new InspectableItem(links[0]) { IsSelected = true };
            ItemTree = links.Cast<HyperLink>().Select(l => new InspectableItem(l));
            Properties = ListProperties(links[0]);
        }

        public InspectorViewModel(GeomRef geomRef)
        {
            var item = new InspectableItem(geomRef);
            ItemTree = new[] { item };
            Properties = ListProperties(geomRef, typeof(DisposableWrapper));
        }

        public InspectorViewModel(EdgeRef[] edges)
        {
            var item = new InspectableItem(edges[0]) { IsSelected = true };
            ItemTree = edges.Select(e => new InspectableItem(e));
            Properties = ListProperties(edges[0], typeof(DisposableWrapper));
        }

        public InspectorViewModel(SubentityId id)
        {
            var item = new InspectableItem(id);
            ItemTree = new[] { item };
            Properties = ListProperties(id);
        }

        public InspectorViewModel(CompoundObjectId id)
        {
            var item = new InspectableItem(id);
            ItemTree = new[] { item };
            Properties = ListProperties(id);
        }

        public InspectorViewModel(HatchLoopCollection loops)
        {
            var item = new InspectableItem(loops.Loops[0]) { IsSelected = true };
            ItemTree = loops.Loops.Select(l => new InspectableItem(l));
            Properties = ListProperties(loops.Loops[0]);
        }

        public InspectorViewModel(HatchLoop loop)
        {
            var item = new InspectableItem(loop);
            ItemTree = new[] { item };
            Properties = ListProperties(loop);
        }

        public InspectorViewModel(Curve2dCollection curves)
        {
            var item = new InspectableItem(curves[0]) { IsSelected = true };
            ItemTree = curves.Cast<Entity2d>().Select(e => new InspectableItem(e));
            Properties = ListProperties(curves[0], typeof(DisposableWrapper));
        }

        public InspectorViewModel(Entity2d entity2D)
        {
            var item = new InspectableItem(entity2D) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(entity2D, typeof(DisposableWrapper));
        }

        public InspectorViewModel(BulgeVertexCollection bulgeVertices)
        {
            var item = new InspectableItem(bulgeVertices[0]) { IsSelected = true };
            ItemTree = bulgeVertices.Cast<BulgeVertex>().Select(e => new InspectableItem(e));
            Properties = ListProperties(bulgeVertices[0]);
        }

        public InspectorViewModel(Tolerance tolerance)
        {
            var item = new InspectableItem(tolerance) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListProperties(tolerance);
        }

        public InspectorViewModel(Point2dCollection points)
        {
            var item = new InspectableItem(points) { IsSelected = true };
            ItemTree = new[] { item };
            Properties = ListCollection(points);
        }

        public InspectorViewModel(KnotCollection knots)
        {
            var item = new InspectableItem(knots);
            ItemTree = new[] { item };
            Properties = ListKnotCollectionProperties(knots);
        }

        public InspectorViewModel(NurbCurve2dData curve2dData)
        {
            var item = new InspectableItem(curve2dData);
            ItemTree = new[] { item };
            Properties = ListProperties(curve2dData);
        }

        public InspectorViewModel(NurbCurve2dFitData curve2dFitData)
        {
            var item = new InspectableItem(curve2dFitData);
            ItemTree = new[] { item };
            Properties = ListProperties(curve2dFitData);
        }

        public InspectorViewModel(NurbCurve3dData curve3dData)
        {
            var item = new InspectableItem(curve3dData);
            ItemTree = new[] { item };
            Properties = ListProperties(curve3dData);
        }

        public InspectorViewModel(NurbCurve3dFitData curve3dFitData)
        {
            var item = new InspectableItem(curve3dFitData);
            ItemTree = new[] { item };
            Properties = ListProperties(curve3dFitData);
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
                        case Extents2d extents: viewModel = new InspectorViewModel(extents); break;
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
                        case DBObject dbObject: viewModel = new InspectorViewModel(dbObject); break;
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
                        case IReferences references: viewModel = new InspectorViewModel(references); break;
                        case Profile3d profile: viewModel = new InspectorViewModel(profile); break;
                        case Entity[] entities: viewModel = new InspectorViewModel(entities); break;
                        case Profile3d[] profiles: viewModel = new InspectorViewModel(profiles); break;
                        case LoftOptions options: viewModel = new InspectorViewModel(options); break;
                        case SweepOptions options: viewModel = new InspectorViewModel(options); break;
                        case RevolveOptions options: viewModel = new InspectorViewModel(options); break;
                        case Solid3dMassProperties massProps: viewModel = new InspectorViewModel(massProps); break;
                        case MlineStyleElementCollection styles: viewModel = new InspectorViewModel(styles); break;
                        case MlineVertices vertices: viewModel = new InspectorViewModel(vertices); break;
                        case CellRange range: viewModel = new InspectorViewModel(range); break;
                        case CellBorders borders: viewModel = new InspectorViewModel(borders); break;
                        case DataTypeParameter param: viewModel = new InspectorViewModel(param); break;
                        case RowsCollection rows: viewModel = new InspectorViewModel(rows); break;
                        case ColumnsCollection columns: viewModel = new InspectorViewModel(columns); break;
                        case HyperLinkCollection links: viewModel = new InspectorViewModel(links); break;
                        case GeomRef geomRef: viewModel = new InspectorViewModel(geomRef); break;
                        case EdgeRef[] edges: viewModel = new InspectorViewModel(edges); break;
                        case SubentityId id: viewModel = new InspectorViewModel(id); break;
                        case CompoundObjectId id: viewModel = new InspectorViewModel(id); break;
                        case HatchLoopCollection loops: viewModel = new InspectorViewModel(loops); break;
                        case Curve2dCollection curves: viewModel = new InspectorViewModel(curves); break;
                        case BulgeVertexCollection bulgeVertices: viewModel = new InspectorViewModel(bulgeVertices); break;
                        case Entity2d entity2d: viewModel = new InspectorViewModel(entity2d); break;
                        case Tolerance tolerance: viewModel = new InspectorViewModel(tolerance); break;
                        case Point2dCollection points: viewModel = new InspectorViewModel(points); break;
                        case KnotCollection knots: viewModel = new InspectorViewModel(knots); break;
                        case NurbCurve2dData curve2dData: viewModel = new InspectorViewModel(curve2dData); break;
                        case NurbCurve2dFitData curve2dFitData: viewModel = new InspectorViewModel(curve2dFitData); break;
                        case NurbCurve3dData curve3dData: viewModel = new InspectorViewModel(curve3dData); break;
                        case NurbCurve3dFitData curve3dFitData: viewModel = new InspectorViewModel(curve3dFitData); break;
                        default: break;
                    }
                    viewModel?.ShowDialog();
                }
            }
        }
        #endregion

        public void ShowDialog() => AcAp.ShowModalWindow(new InspectorDialog(this));

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
            else if (item.Point3dCollection != null)
                Properties = ListCollection(item.Point3dCollection);
            else if (item.Doubles != null)
                Properties = ListCollection(item.Doubles);
            else if (item.LayerFilter != null)
                Properties = ListLayerFilterProperties(item.LayerFilter);
            else if (item.DBObject != null)
                Properties = ListDBObjectProperties(item.DBObject);
            else if (item.IsMlineStyleElement)
                Properties = ListProperties(item.MlineStyleElement);
            else if (item.CellBorder != null)
                Properties = ListProperties(item.CellBorder);
            else if (item.Row != null)
                Properties = ListProperties(item.Row);
            else if (item.Column != null)
                Properties = ListProperties(item.Column);
            else if (item.HyperLink != null)
                Properties = ListProperties(item.HyperLink);
            else if (item.GeomRef != null)
                Properties = ListProperties(item.GeomRef, typeof(DisposableWrapper));
            else if (item.HatchLoop != null)
                Properties = ListProperties(item.HatchLoop);
            else if (item.Entity2D != null)
                Properties = ListProperties(item.Entity2D, typeof(DisposableWrapper));
            else if (item.BulgeVertex != null)
                Properties = ListProperties(item.BulgeVertex);
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
                if (dbObj is AcDb.Polyline pl)
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
                    object value;
                    // From Jeff_M http://www.theswamp.org/index.php?topic=57317.msg608371#msg608371
                    var obsAtt = prop.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "ObsoleteAttribute");
                    if (obsAtt != null)
                    {
                        value = obsAtt.ConstructorArguments.FirstOrDefault().Value;
                    }
                    else
                    {
                        try { value = prop.GetValue(dbObj, null) ?? "(Null)"; }
                        catch (System.Exception e) { value = e.Message; }
                    }
                    bool isInspectable =
                        CheckIsInspectable(value) &&
                        !((value is ObjectId id) && id == dbObj.ObjectId) &&
                        !((value is DBObject obj) && obj.GetType() == dbObj.GetType() && obj.Handle == dbObj.Handle);
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

        private IEnumerable<PropertyItem> ListProperties<T>(T item, Type stopType)
        {
            var types = new List<Type>();
            var type = item.GetType();
            while (type != null && type != stopType)
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
                    try { value = prop.GetValue(item, null) ?? "(Null)"; }
                    catch (System.Exception e) { value = e.Message; }
                    bool isInspectable = CheckIsInspectable(value);
                    yield return new PropertyItem(name, value, subType, isInspectable);
                }
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

        private IEnumerable<PropertyItem> ListCollection(IList collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                yield return new PropertyItem($"[{i}]", collection[i], collection.GetType(), false);
            }
        }

        private IEnumerable<PropertyItem> ListDictEnumProperties(Dictionary<string, string>.Enumerator dictEnum)
        {
            while (dictEnum.MoveNext())
            {
                yield return new PropertyItem(dictEnum.Current.Key, dictEnum.Current.Value, typeof(Dictionary<string, string>.Enumerator), false);
            }
        }

        private IEnumerable<PropertyItem> ListKnotCollectionProperties(KnotCollection knotCollection)
        {
            foreach (var item in ListProperties(knotCollection))
            {
                yield return item;
            }
            if (0 < knotCollection.Count)
            {
                var knots = new DoubleCollection();
                for (int i = 0; i < knotCollection.Count; i++)
                {
                    knots.Add(knotCollection[i]);
                }
                yield return new PropertyItem("Knots", knots, typeof(KnotCollection), true);
            }
        }

        private static bool CheckIsInspectable(object value) =>
            (value is ObjectId id && !id.IsNull) ||
            (value is ResultBuffer && value != null) ||
            value is Matrix3d ||
            (value is Extents3d && value != null) ||
            (value is Extents2d && value != null) ||
            value is CoordinateSystem3d ||
            (value is AcDb.AttributeCollection attCol && 0 < attCol.Count) ||
            (value is DynamicBlockReferencePropertyCollection props && 0 < props.Count) ||
            value is EntityColor ||
            value is Color ||
            value is Entity3d ||
            value is FitData ||
            value is NurbsData ||
            value is DBObject ||
            value is Database ||
            value is LayerFilterTree ||
            (value is LayerFilterCollection filters && 0 < filters.Count) ||
            value is LayerFilter ||
            value is LayerFilterDisplayImages ||
            value is DatabaseSummaryInfo ||
            value is Dictionary<string, string>.Enumerator dictEnum && dictEnum.MoveNext() ||
            value is AnnotationScale ||
            value is FontDescriptor ||
            value is Profile3d ||
            value is Entity[] entities && 0 < entities.Length ||
            value is Profile3d[] profiles && 0 < profiles.Length ||
            value is LoftOptions ||
            value is SweepOptions ||
            value is RevolveOptions ||
            value is Solid3dMassProperties ||
            value is MlineStyleElementCollection styles && 0 < styles.Count ||
            value is MlineVertices ||
            value is CellRange ||
            value is CellBorders ||
            value is DataTypeParameter ||
            value is RowsCollection ||
            value is ColumnsCollection ||
            value is HyperLinkCollection links && 0 < links.Count ||
            value is GeomRef ||
            value is EdgeRef[] edges && 0 < edges.Length ||
            value is SubentityId ||
            value is CompoundObjectId ||
            value is Curve2dCollection curves && 0 < curves.Count ||
            value is BulgeVertexCollection vertices && 0 < vertices.Count ||
            value is Entity2d ||
            value is Tolerance ||
            value is Point2dCollection points && 0 < points.Count ||
            value is KnotCollection ||
            value is NurbCurve2dData ||
            value is NurbCurve2dFitData ||
            value is NurbCurve3dData ||
            value is NurbCurve3dFitData;
        #endregion
    }
}
