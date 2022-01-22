using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.Inspector
{
    public interface IReferences
    {
        ObjectIdCollection SoftPointerIds { get; }
        ObjectIdCollection HardPointerIds { get; }
        ObjectIdCollection SoftOwnershipIds { get; }
        ObjectIdCollection HardOwnershipIds { get; }
    }
}
