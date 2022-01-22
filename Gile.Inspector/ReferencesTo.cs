using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.Inspector
{
    class ReferencesTo : IReferences
    {
        public ObjectIdCollection SoftPointerIds { get; }
        public ObjectIdCollection HardPointerIds { get; }
        public ObjectIdCollection SoftOwnershipIds { get; }
        public ObjectIdCollection HardOwnershipIds { get; }

        public ReferencesTo(ObjectId id)
        {
            SoftPointerIds = new ObjectIdCollection();
            HardPointerIds = new ObjectIdCollection();
            SoftOwnershipIds = new ObjectIdCollection();
            HardOwnershipIds = new ObjectIdCollection();
            if (!id.IsNull)
            {
                using (var tr = id.Database.TransactionManager.StartTransaction())
                {
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
}
