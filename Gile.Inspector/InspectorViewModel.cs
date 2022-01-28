using Autodesk.AutoCAD.BoundaryRepresentation;
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
using AcBr = Autodesk.AutoCAD.BoundaryRepresentation;

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
        #region Collection
        public InspectorViewModel(ObjectIdCollection ids)
        {
            var children = ids
                .Cast<ObjectId>()
                .Select(id => new InspectableItem(id, name: "<_>"));
            var items = ids.Cast<ObjectId>().Select(id => new InspectableItem(id));
            items.First().IsSelected = true;
            ItemTree = items;
            Properties = ListObjectIdProperties(ids[0]);
        }

        public InspectorViewModel(PolylineVertices vertices)
        {
            var items = vertices.Vertices.Select(v => new InspectableItem(v));
            items.First().IsSelected = true;
            ItemTree = items;
            Properties = ListProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(Polyline3dVertices vertices)
        {
            var items = vertices.Vertices.Cast<ObjectId>().Select(id => new InspectableItem(id));
            items.First().IsSelected = true;
            ItemTree = items;
            Properties = ListObjectIdProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(Polyline2dVertices vertices)
        {
            var items = vertices.Vertices.Cast<ObjectId>().Select(id => new InspectableItem(id));
            items.First().IsSelected = true;
            ItemTree = items;
            Properties = ListObjectIdProperties(vertices.Vertices[0]);
        }

        public InspectorViewModel(LayerFilter filter)
        {
            var children = filter
                .NestedFilters
                .Cast<LayerFilter>()
                .Select(f => new InspectableItem(f));
            var items = filter.NestedFilters.Cast<LayerFilter>().Select(f => new InspectableItem(f));
            items.First().IsSelected = true;
            ItemTree = items;
            Properties = ListProperties(filter);
        }

        public InspectorViewModel(IReferences references)
        {
            IEnumerable<InspectableItem> children(ObjectIdCollection ids) =>
                ids
                .Cast<ObjectId>()
                .Select(id => new InspectableItem(id, name: "<_>"));
            ItemTree = new[]
            {
                new InspectableItem(references.HardPointerIds, true, true, children(references.HardPointerIds), name: "Hard pointer"),
                new InspectableItem(references.SoftPointerIds, false, true, children(references.SoftPointerIds), name: "Soft pointer"),
                new InspectableItem(references.HardOwnershipIds, false, true, children(references.HardOwnershipIds), name: "Hard ownership"),
                new InspectableItem(references.SoftOwnershipIds, false, true, children(references.SoftOwnershipIds), name: "Soft ownership"),
            };
            Properties = new PropertyItem[0];
        }

        public InspectorViewModel(CellBorders borders)
        {
            ItemTree = new[]
            {
                new InspectableItem(borders.Vertical, true, name: "Vertical"),
                new InspectableItem(borders.Horizontal, name: "Horizontal"),
                new InspectableItem(borders.Bottom, name: "Bottom"),
                new InspectableItem(borders.Right, name: "Right"),
                new InspectableItem(borders.Top, name: "Top"),
                new InspectableItem(borders.Left, name: "Left")
            };
            Properties = ListProperties(borders.Vertical);
        }

        public InspectorViewModel(EdgeRef[] edges)
        {
            var items = edges.Select(e => new InspectableItem(e));
            items.First().IsSelected = true;
            ItemTree = items;
            Properties = ListGroupedProperties(edges[0], typeof(DisposableWrapper));
        }

        public InspectorViewModel(HatchLoopCollection loops)
        {
            var items = loops.Loops.Select(l => new InspectableItem(l));
            items.First().IsSelected = true;
            ItemTree = items;
            Properties = ListProperties(loops.Loops[0]);
        }

        public InspectorViewModel(Curve2dCollection curves)
        {
            var items = curves.Cast<Entity2d>().Select(e => new InspectableItem(e));
            items.First().IsSelected = true;
            ItemTree = items;
            Properties = ListGroupedProperties(curves[0], typeof(DisposableWrapper));
        }

        public InspectorViewModel(LayerFilterCollection collection) { InitializeCollection<LayerFilter>(collection, ListLayerFilterProperties); }
        public InspectorViewModel(AcDb.AttributeCollection collection) { InitializeCollection<ObjectId>(collection, ListObjectIdProperties); }
        public InspectorViewModel(MlineStyleElementCollection collection) { InitializeCollection<MlineStyleElement>(collection); }
        public InspectorViewModel(DynamicBlockReferencePropertyCollection collection) { InitializeCollection<DynamicBlockReferenceProperty>(collection); }
        public InspectorViewModel(HyperLinkCollection collection) { InitializeCollection<HyperLink>(collection); }
        public InspectorViewModel(BulgeVertexCollection collection) { InitializeCollection<BulgeVertex>(collection); }
        public InspectorViewModel(Entity[] collection) { InitializeCollection(collection); }
        public InspectorViewModel(Profile3d[] collection) { InitializeCollection(collection); }
        public InspectorViewModel(RowsCollection collection) { InitializeCollection(collection); }
        public InspectorViewModel(ColumnsCollection collection) { InitializeCollection(collection); }

        #region Brep
        public InspectorViewModel(BrepVertexCollection value) { InitializeCollection(value); }
        public InspectorViewModel(BrepEdgeCollection value) { InitializeCollection(value); }
        public InspectorViewModel(BrepFaceCollection value) { InitializeCollection(value); }
        public InspectorViewModel(BrepComplexCollection value) { InitializeCollection(value); }
        public InspectorViewModel(BrepShellCollection value) { InitializeCollection(value); }
        public InspectorViewModel(FaceLoopCollection value) { InitializeCollection(value); }
        public InspectorViewModel(EdgeLoopCollection value) { InitializeCollection(value); }
        public InspectorViewModel(ComplexShellCollection value) { InitializeCollection(value); }
        public InspectorViewModel(ShellFaceCollection value) { InitializeCollection(value); }
        public InspectorViewModel(LoopVertexCollection value) { InitializeCollection(value); }
        public InspectorViewModel(LoopEdgeCollection value) { InitializeCollection(value); }
        public InspectorViewModel(VertexEdgeCollection value) { InitializeCollection(value); }
        public InspectorViewModel(VertexLoopCollection value) { InitializeCollection(value); }
        #endregion

        private void InitializeCollection<T>(ICollection collection, Func<T, IEnumerable<PropertyItem>> listFunction = null)
        {
            var items = collection.Cast<T>().Select(x => new InspectableItem(x)).ToList();
            items[0].IsSelected = true;
            ItemTree = items;
            Properties = listFunction == null ?
                ListProperties(collection.Cast<T>().First()) :
                listFunction(collection.Cast<T>().First());
        }

        private void InitializeCollection<T>(IEnumerable<T> collection, Func<T, IEnumerable<PropertyItem>> listFunction = null)
        {
            var items = collection.Select(e => new InspectableItem(e)).ToList();
            items[0].IsSelected = true;
            ItemTree = items;
            Properties = listFunction == null ?
                ListProperties(collection.First()) :
                listFunction(collection.First());
        }
        #endregion

        #region Single
        public InspectorViewModel(Database value) { InitializeSingle(value); }
        public InspectorViewModel(ObjectId value) { InitializeSingle(value, ListObjectIdProperties); }
        public InspectorViewModel(ResultBuffer value) { InitializeSingle(value); }
        public InspectorViewModel(Matrix3d value) { InitializeSingle(value); }
        public InspectorViewModel(Extents3d value) { InitializeSingle(value); }
        public InspectorViewModel(Extents2d value) { InitializeSingle(value); }
        public InspectorViewModel(CoordinateSystem3d value) { InitializeSingle(value); }
        public InspectorViewModel(EntityColor value) { InitializeSingle(value); }
        public InspectorViewModel(Color value) { InitializeSingle(value); }
        public InspectorViewModel(FitData value) { InitializeSingle(value); }
        public InspectorViewModel(NurbsData value) { InitializeSingle(value); }
        public InspectorViewModel(Point3dCollection value) { InitializeSingle(value); }
        public InspectorViewModel(DoubleCollection value) { InitializeSingle(value); }
        public InspectorViewModel(DBObject value) { InitializeSingle(value, ListDBObjectProperties); }
        public InspectorViewModel(LayerFilterTree filterTree) { InitializeSingle(filterTree.Root); }
        public InspectorViewModel(LayerFilterDisplayImages value) { InitializeSingle(value); }
        public InspectorViewModel(DatabaseSummaryInfo value) { InitializeSingle(value); }
        public InspectorViewModel(Dictionary<string, string>.Enumerator value) { InitializeSingle(value, ListDictEnumProperties); }
        public InspectorViewModel(AnnotationScale value) { InitializeSingle(value); }
        public InspectorViewModel(FontDescriptor value) { InitializeSingle(value); }
        public InspectorViewModel(Profile3d value) { InitializeSingle(value); }
        public InspectorViewModel(LoftOptions value) { InitializeSingle(value); }
        public InspectorViewModel(SweepOptions value) { InitializeSingle(value); }
        public InspectorViewModel(RevolveOptions value) { InitializeSingle(value); }
        public InspectorViewModel(Solid3dMassProperties value) { InitializeSingle(value); }
        public InspectorViewModel(MlineVertices vertices) { InitializeSingle(vertices.Vertices); }
        public InspectorViewModel(CellRange value) { InitializeSingle(value); }
        public InspectorViewModel(DataTypeParameter value) { InitializeSingle(value); }
        public InspectorViewModel(SubentityId value) { InitializeSingle(value); }
        public InspectorViewModel(CompoundObjectId value) { InitializeSingle(value); }
        public InspectorViewModel(HatchLoop value) { InitializeSingle(value); }
        public InspectorViewModel(Tolerance value) { InitializeSingle(value); }
        public InspectorViewModel(Point2dCollection value) { InitializeSingle(value, ListCollection); }
        public InspectorViewModel(KnotCollection value) { InitializeSingle(value); }
        public InspectorViewModel(NurbCurve2dData value) { InitializeSingle(value); }
        public InspectorViewModel(NurbCurve2dFitData value) { InitializeSingle(value); }
        public InspectorViewModel(NurbCurve3dData value) { InitializeSingle(value); }
        public InspectorViewModel(NurbCurve3dFitData value) { InitializeSingle(value); }
        public InspectorViewModel(Transparency value) { InitializeSingle(value); }
        public InspectorViewModel(FullDwgVersion value) { InitializeSingle(value); }
        public InspectorViewModel(PlotStyleDescriptor value) { InitializeSingle(value); }
        public InspectorViewModel(Complex value) { InitializeSingle(value); }
        public InspectorViewModel(Shell value) { InitializeSingle(value); }
        public InspectorViewModel(AcBr.Face value) { InitializeSingle(value); }
        public InspectorViewModel(BoundaryLoop value) { InitializeSingle(value); }
        public InspectorViewModel(Edge value) { InitializeSingle(value); }
        public InspectorViewModel(AcBr.Vertex value) { InitializeSingle(value); }
        public InspectorViewModel(MaterialColor value) { InitializeSingle(value); }
        public InspectorViewModel(MaterialMap value) { InitializeSingle(value); }
        public InspectorViewModel(MaterialTexture value) { InitializeSingle(value, typeof(RXObject)); }
        public InspectorViewModel(MaterialNormalMapComponent value) { InitializeSingle(value); }
        public InspectorViewModel(MaterialRefractionComponent value) { InitializeSingle(value); }
        public InspectorViewModel(MaterialOpacityComponent value) { InitializeSingle(value); }
        public InspectorViewModel(MaterialSpecularComponent value) { InitializeSingle(value); }
        public InspectorViewModel(MaterialDiffuseComponent value) { InitializeSingle(value); }
        //public InspectorViewModel(ImageFileTexture value) { InitializeSingle(value); }
        public InspectorViewModel(PhotographicExposureParameters value) { InitializeSingle(value, typeof(RXObject)); }
        public InspectorViewModel(Entity3d value) { InitializeSingle(value, typeof(DisposableWrapper)); }
        public InspectorViewModel(Entity2d value) { InitializeSingle(value, typeof(DisposableWrapper)); }
        public InspectorViewModel(GeomRef value) { InitializeSingle(value, typeof(DisposableWrapper)); }
        public InspectorViewModel(Brep brep)
        {
            BrepSolid = brep.Solid;
            BrepSurf = brep.Surf;
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
            ItemTree = new[] { item };
            Properties = ListProperties(brep);
        }

        private void InitializeSingle<T>(T value, Func<T, IEnumerable<PropertyItem>> listFunction = null)
        {
            var item = new InspectableItem(value, true, true);
            ItemTree = new[] { item };
            Properties = listFunction == null ? ListProperties(value) : listFunction(value);
        }

        private void InitializeSingle<T>(T value, Type topBaseType)
        {
            var item = new InspectableItem(value, true, true);
            ItemTree = new[] { item };
            Properties = ListGroupedProperties(value, topBaseType);
        }
        #endregion
        #endregion

        #region Properties
        public Entity BrepSurf { get; private set; }
        public Entity BrepSolid { get; private set; }

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
                        case FullDwgVersion version: viewModel = new InspectorViewModel(version); break;
                        case Transparency transparency: viewModel = new InspectorViewModel(transparency); break;
                        case PlotStyleDescriptor plotStyleDescriptor: viewModel = new InspectorViewModel(plotStyleDescriptor); break;
                        case PhotographicExposureParameters parameters: viewModel = new InspectorViewModel(parameters); break;
                        case Brep brep: viewModel = new InspectorViewModel(brep); break;
                        case Complex complex: viewModel = new InspectorViewModel(complex); break;
                        case Shell shell: viewModel = new InspectorViewModel(shell); break;
                        case AcBr.Face face: viewModel = new InspectorViewModel(face); break;
                        case BoundaryLoop loop: viewModel = new InspectorViewModel(loop); break;
                        case Edge edge: viewModel = new InspectorViewModel(edge); break;
                        case AcBr.Vertex vertex: viewModel = new InspectorViewModel(vertex); break;
                        case BrepVertexCollection vertices: viewModel = new InspectorViewModel(vertices); break;
                        case BrepEdgeCollection edges: viewModel = new InspectorViewModel(edges); break;
                        case BrepComplexCollection complexes: viewModel = new InspectorViewModel(complexes); break;
                        case BrepFaceCollection faces: viewModel = new InspectorViewModel(faces); break;
                        case BrepShellCollection vertex: viewModel = new InspectorViewModel(vertex); break;
                        case FaceLoopCollection loops: viewModel = new InspectorViewModel(loops); break;
                        case EdgeLoopCollection loops: viewModel = new InspectorViewModel(loops); break;
                        case ComplexShellCollection shells: viewModel = new InspectorViewModel(shells); break;
                        case ShellFaceCollection faces: viewModel = new InspectorViewModel(faces); break;
                        case LoopVertexCollection vertices: viewModel = new InspectorViewModel(vertices); break;
                        case LoopEdgeCollection edges: viewModel = new InspectorViewModel(edges); break;
                        case VertexEdgeCollection edges: viewModel = new InspectorViewModel(edges); break;
                        case VertexLoopCollection loops: viewModel = new InspectorViewModel(loops); break;
                        case MaterialColor color: viewModel = new InspectorViewModel(color); break;
                        case MaterialMap map: viewModel = new InspectorViewModel(map); break;
                        case MaterialTexture texture: viewModel = new InspectorViewModel(texture); break;
                        case MaterialNormalMapComponent normalMap: viewModel = new InspectorViewModel(normalMap); break;
                        case MaterialRefractionComponent refraction: viewModel = new InspectorViewModel(refraction); break;
                        case MaterialOpacityComponent opacity: viewModel = new InspectorViewModel(opacity); break;
                        case MaterialSpecularComponent specular: viewModel = new InspectorViewModel(specular); break;
                        case MaterialDiffuseComponent diffuse: viewModel = new InspectorViewModel(diffuse); break;
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
            switch (item.Value)
            {
                case ObjectId id when !id.IsNull: Properties = ListObjectIdProperties(id); break;
                case DynamicBlockReferenceProperty prop: Properties = ListProperties(prop); break;
                case ResultBuffer resbuf: Properties = ListResultBufferProperties(resbuf); break;
                case PolylineVertex vertex: Properties = ListProperties(vertex); break;
                case Point3dCollection points: Properties = ListCollection(points); break;
                case DoubleCollection doubles: Properties = ListCollection(doubles); break;
                case LayerFilter filter: Properties = ListLayerFilterProperties(filter); break;
                case DBObject dBObject: Properties = ListDBObjectProperties(dBObject); break;
                case MlineStyleElement mlElement: Properties = ListProperties(mlElement); break;
                case CellBorder border: Properties = ListProperties(border); break;
                case Row row: Properties = ListProperties(row); break;
                case Column column: Properties = ListProperties(column); break;
                case HyperLink hyperLink: Properties = ListProperties(hyperLink); break;
                case GeomRef geomRef: Properties = ListProperties(geomRef); break;
                case HatchLoop loop: Properties = ListProperties(loop); break;
                case Entity2d entity2d: Properties = ListGroupedProperties(entity2d, typeof(DisposableWrapper)); break;
                case BulgeVertex vertex: Properties = ListProperties(vertex); break;
                case Complex complex: Properties = ListProperties(complex); break;
                case Shell shell: Properties = ListProperties(shell); break;
                case AcBr.Face face: Properties = ListProperties(face); break;
                case BoundaryLoop vertex: Properties = ListProperties(vertex); break;
                case Edge edge: Properties = ListProperties(edge); break;
                default:
                    break;
            }
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            BrepSolid?.Dispose();
            BrepSurf?.Dispose();
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
                if (dbObj is Region || dbObj is Solid3d || dbObj is AcDb.Surface)
                {
                    yield return new PropertyItem("Boundary representation", new Brep((Entity)dbObj), dbObj.GetType(), true);
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

        private IEnumerable<PropertyItem> ListGroupedProperties<T>(T item, Type stopType)
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
                    if (prop.Name == "Item") continue;
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
                if (name == "Item") continue;
                if (item is Brep && (name == "Surf" || name == "Solid")) continue;
                if (item is DynamicBlockReferenceProperty && name == "Value") continue;
                object value;
                try { value = prop.GetValue(item, null) ?? "(Null)"; }
                catch (System.Exception e) { value = e.Message; }
                bool isInspectable = CheckIsInspectable(value);
                yield return new PropertyItem(name, value, typeof(T), isInspectable);
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
            value is NurbCurve3dFitData ||
            value is FullDwgVersion ||
            value is Transparency ||
            value is PlotStyleDescriptor ||
            value is PhotographicExposureParameters ||
            value is Brep ||
            value is Complex ||
            value is Shell ||
            value is AcBr.Face ||
            value is BoundaryLoop ||
            value is Edge ||
            value is AcBr.Vertex ||
            value is BrepComplexCollection ||
            value is BrepShellCollection ||
            value is ComplexShellCollection ||
            value is BrepFaceCollection ||
            value is ShellFaceCollection ||
            value is FaceLoopCollection ||
            value is BrepEdgeCollection ||
            value is EdgeLoopCollection ||
            value is LoopEdgeCollection ||
            value is BrepVertexCollection ||
            value is LoopVertexCollection ||
            value is VertexEdgeCollection ||
            value is VertexLoopCollection ||
            value is MaterialColor ||
            value is MaterialMap ||
            value is MaterialTexture ||
            value is MaterialNormalMapComponent ||
            value is MaterialRefractionComponent ||
            value is MaterialOpacityComponent ||
            value is MaterialSpecularComponent ||
            value is MaterialDiffuseComponent;
        #endregion
    }
}
