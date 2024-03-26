using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Describes the objects referenced by a DBObject.
    /// </summary>
    public class ReferencedBy : IReferences
    {
        private ObjectId sourceId;

        /// <summary>
        /// Gets the soft pointers.
        /// </summary>
        public ObjectIdCollection SoftPointerIds { get; }

        /// <summary>
        /// Gets the harde pointers.
        /// </summary>
        public ObjectIdCollection HardPointerIds { get; }

        /// <summary>
        /// Gets the soft ownerships.
        /// </summary>
        public ObjectIdCollection SoftOwnershipIds { get; }

        /// <summary>
        /// Gets the hard ownerships.
        /// </summary>
        public ObjectIdCollection HardOwnershipIds { get; }

        /// <summary>
        /// Creates a new instance of ReferencedBy.
        /// </summary>
        /// <param name="id">Instance of ObjectId.</param>
        public ReferencedBy(ObjectId id)
        {
            sourceId = id;
            SoftPointerIds = [];
            HardPointerIds = [];
            SoftOwnershipIds = [];
            HardOwnershipIds = [];
            if (!id.IsNull)
            {
                var db = id.Database;
                using var tr = db.TransactionManager.StartTransaction();
                ProcessObject(tr, db.NamedObjectsDictionaryId);
                ProcessObject(tr, db.BlockTableId);
                ProcessObject(tr, db.DimStyleTableId);
                ProcessObject(tr, db.LayerTableId);
                ProcessObject(tr, db.LinetypeTableId);
                ProcessObject(tr, db.RegAppTableId);
                ProcessObject(tr, db.TextStyleTableId);
                ProcessObject(tr, db.UcsTableId);
                ProcessObject(tr, db.ViewportTableId);
                ProcessObject(tr, db.ViewTableId);
                tr.Commit();
            }
        }

        private void ProcessObject(Transaction tr, ObjectId curId)
        {
            var dbObj = tr.GetObject(curId, OpenMode.ForRead);
            var filer = new ReferenceFiler();
            dbObj.DwgOut(filer);

            if (filer.HardOwnershipIds.Contains(sourceId))
                HardOwnershipIds.Add(dbObj.ObjectId);
            if (filer.SoftOwnershipIds.Contains(sourceId))
                SoftOwnershipIds.Add(dbObj.ObjectId);
            if (filer.HardPointerIds.Contains(sourceId))
                HardPointerIds.Add(dbObj.ObjectId);
            if (filer.SoftPointerIds.Contains(sourceId))
                SoftPointerIds.Add(dbObj.ObjectId);

            foreach (ObjectId id in filer.HardOwnershipIds)
            {
                ProcessObject(tr, id);
            }
            foreach (ObjectId id in filer.SoftOwnershipIds)
            {
                ProcessObject(tr, id);
            }
        }
    }
}
