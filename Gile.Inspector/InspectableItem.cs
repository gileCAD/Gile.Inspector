using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.LayerManager;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Type bounded to the items of the TreeView control.
    /// </summary>
    public class InspectableItem : ItemBase
    {
        #region Properties
        public IEnumerable<InspectableItem> Children { get; private set; }
        public DynamicBlockReferenceProperty DynamicProperty { get; }
        public DoubleCollection Doubles { get; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
        public string Name { get; private set; }
        public ObjectId ObjectId { get; }
        public Point3dCollection Points { get; }
        public ResultBuffer ResultBuffer { get; }
        public PolylineVertex PolylineVertex { get; }
        public LayerFilter LayerFilter { get; }
        public DBObject DBObject { get; }
        public MlineStyleElement MlineStyleElement { get; }
        public bool IsMlineStyleElement { get; }
        public CellBorder CellBorder { get; }
        public Row Row { get; }
        public Column Column { get; }
        #endregion

        #region Constructors
        public InspectableItem(Database db) : base(db) { Name = Label; }

        public InspectableItem(ObjectId id) : base(id)
        {
            ObjectId = id;
            Initialize(id);
        }

        public InspectableItem(ObjectId id, string name) : base(id)
        {
            ObjectId = id;
            Name = name;
            Initialize(id);
        }

        public InspectableItem(DynamicBlockReferenceProperty prop) : base(prop)
        {
            DynamicProperty = prop;
            Name = prop.PropertyName;
        }

        public InspectableItem(ResultBuffer resbuf) : base(resbuf)
        {
            ResultBuffer = resbuf;
            Name = Label;
        }

        public InspectableItem(Matrix3d matrix) : base(matrix) { Name = Label; }

        public InspectableItem(Extents3d extents) : base(extents) { Name = Label; }

        public InspectableItem(CoordinateSystem3d cs) : base(cs) { Name = Label; }

        public InspectableItem(EntityColor co) : base(co) { Name = Label; }

        public InspectableItem(Color co) : base(co) { Name = Label; }

        public InspectableItem(PolylineVertex vertex) : base(vertex)
        {
            PolylineVertex = vertex;
            Name = Label;
        }

        public InspectableItem(Entity3d entity3d) : base(entity3d) { Name = Label; }

        public InspectableItem(FitData fitData) : base(fitData) { Name = Label; }

        public InspectableItem(NurbsData nurbsData) : base(nurbsData) { Name = Label; }

        public InspectableItem(DoubleCollection doubles) : base(doubles)
        {
            Doubles = doubles;
            Name = Label;
        }

        public InspectableItem(Point3dCollection points) : base(points)
        {
            Points = points;
            Name = Label;
        }

        public InspectableItem(DBObject dbObject) : base(dbObject)
        {
            DBObject = dbObject;
            Name = Label;
        }

        public InspectableItem(LayerFilter filter) : base(filter)
        {
            LayerFilter = filter;
            Name = filter.Name;
            Children = filter
                .NestedFilters
                .Cast<LayerFilter>()
                .Select(f => new InspectableItem(f));
            if (filter.Parent == null)
                IsExpanded = true;
        }

        public InspectableItem(LayerFilterDisplayImages images) : base(images) { Name = Label; }

        public InspectableItem(DatabaseSummaryInfo info) : base(info) { Name = Label; }

        public InspectableItem(Dictionary<string, string>.Enumerator dictEnum) : base(dictEnum) { Name = Label; }

        public InspectableItem(AnnotationScale scale) : base(scale) { Name = Label; }

        public InspectableItem(FontDescriptor font) : base(font) { Name = Label; }

        public InspectableItem(string name, ObjectIdCollection ids) : base(ids)
        {
            Name = name;
            Children = ids
                .Cast<ObjectId>()
                .Select(id => new InspectableItem(id, "<_>"));
            IsExpanded = true;
        }

        public InspectableItem(Profile3d profile) : base(profile) { Name = Label; }

        public InspectableItem(LoftOptions options) : base(options) { Name = Label; }

        public InspectableItem(SweepOptions options) : base(options) { Name = Label; }

        public InspectableItem(RevolveOptions options) : base(options) { Name = Label; }

        public InspectableItem(Solid3dMassProperties massProps) : base(massProps) { Name = Label; }

        public InspectableItem(MlineStyleElementCollection mlineStyles) : base(mlineStyles) { Name = Label; }

        public InspectableItem(MlineStyleElement mlineStyle) : base(mlineStyle) 
        { 
            Name = Label;
            IsMlineStyleElement = true;
            MlineStyleElement = mlineStyle;
        }

        public InspectableItem(CellRange range) : base(range) { Name = Label; }

        public InspectableItem(CellBorder border, string name) : base(border) 
        { 
            Name = name;
            CellBorder = border;
        }

        public InspectableItem(Row row) : base(row) { Name = Label; Row = row; }

        public InspectableItem(Column column) : base(column) { Name = Label; Column = column; }

        public InspectableItem(DataTypeParameter param) : base(param) { Name = Label; }

        private void Initialize(ObjectId id)
        {
            using (var tr = id.Database.TransactionManager.StartTransaction())
            {
                var dbObj = tr.GetObject(id, OpenMode.ForRead);
                if (string.IsNullOrEmpty(Name))
                {
                    Name = dbObj is SymbolTableRecord r ? r.Name : $"< {dbObj.GetType().Name} >";
                }
                else if (Name == "<_>")
                {
                    Name = $"< {dbObj.GetType().Name} >";
                }
                if (dbObj is SymbolTable)
                {
                    Children = ((SymbolTable)dbObj)
                        .Cast<ObjectId>()
                        .Select(x => new InspectableItem(x));
                }
                else if (dbObj is DBDictionary)
                {
                    if (id == id.Database.NamedObjectsDictionaryId)
                    {
                        Name = "Named Objects Dictionary";
                        IsExpanded = true;
                    }
                    Children = ((DBDictionary)dbObj)
                        .Cast<DictionaryEntry>()
                        .Select(e => new InspectableItem((ObjectId)e.Value, (string)e.Key));
                }
            }
        }
        #endregion
    }
}
