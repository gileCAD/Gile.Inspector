using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Properties to be implemented for reference types.
    /// </summary>
    public interface IReferences
    {
        ObjectIdCollection SoftPointerIds { get; }
        ObjectIdCollection HardPointerIds { get; }
        ObjectIdCollection SoftOwnershipIds { get; }
        ObjectIdCollection HardOwnershipIds { get; }
    }
}
