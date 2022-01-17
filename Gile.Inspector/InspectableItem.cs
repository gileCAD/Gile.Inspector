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

        public DynamicBlockReferenceProperty DynamicProperty { get; }

        public EntityColor EntityColor { get; }

        public Extents3d Extents { get; }

        public bool IsCoordinateSystem3d { get; } = false;

        public bool IsDatabase { get; } = false;

        public bool IsEntityColor { get; } = false;

        public bool IsExpanded { get; set; }

        public bool IsExtents3d { get; } = false;

        public bool IsMatrix3d { get; } = false;

        public bool IsSelected { get; set; }

        public Matrix3d Matrix3d { get; }

        public string Name { get; private set; }

        public ObjectId ObjectId { get; }

        public ResultBuffer ResultBuffer { get; }
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
