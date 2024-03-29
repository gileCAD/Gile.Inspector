using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes the objects that a DBObject references to.
    /// </summary>
    class ReferencesTo : IReferences
    {
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
        /// Creates a new instance of ReferencesTo.
        /// </summary>
        /// <param name="id">Instance of ObjectId.</param>
        public ReferencesTo(ObjectId id)
        {
            SoftPointerIds = [];
            HardPointerIds = [];
            SoftOwnershipIds = [];
            HardOwnershipIds = [];
            if (!id.IsNull)
            {
                using var tr = id.Database.TransactionManager.StartTransaction();
                var dbObj = tr.GetObject(id, OpenMode.ForRead);
                var filer = new ReferenceFiler();
                dbObj.DwgOut(filer);
                HardOwnershipIds = filer.HardOwnershipIds;
                SoftOwnershipIds = filer.SoftOwnershipIds;
                HardPointerIds = filer.HardPointerIds;
                SoftPointerIds = filer.SoftPointerIds;
                tr.Commit();
            }
        }
    }
}
