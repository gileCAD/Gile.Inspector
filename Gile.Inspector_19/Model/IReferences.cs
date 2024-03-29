using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Properties to be implemented for reference types.
    /// </summary>
    public interface IReferences
    {
        /// <summary>
        /// Gets the soft pointers.
        /// </summary>
        ObjectIdCollection SoftPointerIds { get; }

        /// <summary>
        /// Gets the harde pointers.
        /// </summary>
        ObjectIdCollection HardPointerIds { get; }

        /// <summary>
        /// Gets the soft ownerships.
        /// </summary>
        ObjectIdCollection SoftOwnershipIds { get; }

        /// <summary>
        /// Gets the hard ownerships.
        /// </summary>
        ObjectIdCollection HardOwnershipIds { get; }
    }
}
