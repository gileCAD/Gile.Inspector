using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gile.AutoCAD.Inspector
{
    public class InspectableItem : LabelItem
    {
        #region Properties
        public IEnumerable<InspectableItem> Children { get; private set; }

        public Color Color { get; }

        public CoordinateSystem3d CoordinateSystem3d { get; }

        public Entity3d Entity3d { get; }

        public DynamicBlockReferenceProperty DynamicProperty { get; }

        public EntityColor EntityColor { get; }

        public Extents3d Extents { get; }

        public DoubleCollection Doubles { get; }

        public FitData FitData { get; }

        public bool IsCoordinateSystem3d { get; }

        public bool IsDatabase { get; } = false;

        public bool IsEntityColor { get; }

        public bool IsExpanded { get; set; }

        public bool IsExtents3d { get; }

        public bool IsFitData { get; }

        public bool IsMatrix3d { get; }

        public bool IsNurbsData { get; }

        public bool IsSelected { get; set; }

        public Matrix3d Matrix3d { get; }

        public string Name { get; private set; }

        public NurbsData NurbsData { get; }

        public ObjectId ObjectId { get; }

        public Point3dCollection Points { get; }

        public ResultBuffer ResultBuffer { get; }

        public PolylineVertices PolylineVertices { get; }

        public PolylineVertex PolylineVertex { get; }

        public Spline Spline { get; }
        #endregion

        #region Constructors
        public InspectableItem(Database db) : base(db)
        {
            IsDatabase = true;
            Name = Label;
        }

        public InspectableItem(ObjectId id) : base(id)
        {
            ObjectId = id;
            Initialize(id);
        }

        public InspectableItem(string name, ObjectId id) : base (id)
        {
            ObjectId = id;
            Name = name;
            Initialize(id);
        }

        public InspectableItem(DynamicBlockReferenceProperty prop) : base (prop)
        {
            DynamicProperty = prop;
            Name = Label;
        }

        public InspectableItem(ResultBuffer resbuf) : base(resbuf)
        {
            ResultBuffer = resbuf;
            Name = Label;
        }

        public InspectableItem(Matrix3d matrix) : base(matrix)
        {
            Matrix3d = matrix;
            IsMatrix3d = true;
            Name = Label;
        }

        public InspectableItem(Extents3d extents) : base(extents)
        {
            Extents = extents;
            IsExtents3d = true;
            Name = Label;
        }

        public InspectableItem(CoordinateSystem3d cs) : base(cs)
        {
            CoordinateSystem3d = cs;
            IsCoordinateSystem3d = true;
            Name = Label;
        }

        public InspectableItem(EntityColor co) : base(co)
        {
            EntityColor = co;
            IsEntityColor = true;
            Name = Label;
        }

        public InspectableItem(Color co) : base(co)
        {
            Color = co;
            IsEntityColor = true;
            Name = Label;
        }

        public InspectableItem(PolylineVertices vertices) : base(vertices)
        {
            PolylineVertices = vertices;
            Name = Label;
        }

        public InspectableItem(PolylineVertex vertex) : base(vertex)
        {
            PolylineVertex = vertex;
            Name = Label;
        }

        public InspectableItem(Entity3d entity3d) : base(entity3d)
        {
            Entity3d = entity3d;
            Name = Label;
        }

        public InspectableItem(FitData fitData) : base(fitData)
        {
            FitData = fitData;
            IsFitData = true;
            Name = Label;
        }

        public InspectableItem(NurbsData nurbsData) : base(nurbsData)
        {
            NurbsData = nurbsData;
            IsFitData = true;
            Name = Label;
        }

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

        public InspectableItem(Spline spline) : base(spline)
        {
            Spline = spline;
            Name = Label;
        }

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
