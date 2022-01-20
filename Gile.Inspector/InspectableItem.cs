using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.LayerManager;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gile.AutoCAD.Inspector
{
    public class InspectableItem : LabelItem
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
        #endregion

        #region Constructors
        public InspectableItem(Database db) : base(db) { Name = Label; }

        public InspectableItem(ObjectId id) : base(id)
        {
            ObjectId = id;
            Initialize(id);
        }

        public InspectableItem(string name, ObjectId id) : base(id)
        {
            ObjectId = id;
            Name = name;
            Initialize(id);
        }

        public InspectableItem(DynamicBlockReferenceProperty prop) : base(prop)
        {
            DynamicProperty = prop;
            Name = Label;
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

        public InspectableItem(PolylineVertex vertex) : base(vertex) { Name = Label; }

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

        public InspectableItem(Spline spline) : base(spline) { Name = Label; }

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

        private void Initialize(ObjectId id)
        {
            using (var tr = new OpenCloseTransaction())
            {
                var dbObj = tr.GetObject(id, OpenMode.ForRead);
                if (string.IsNullOrEmpty(Name))
                {
                    Name = dbObj is SymbolTableRecord r ? r.Name : $"< {dbObj.GetType().Name} >";
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
                        .Select(e => new InspectableItem((string)e.Key, (ObjectId)e.Value));
                }
            }
        }
        #endregion
    }
}
