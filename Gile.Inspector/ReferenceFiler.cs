using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Defines a reference filer.
    /// </summary>
    class ReferenceFiler : DwgFiler
    {
        /// <summary>
        /// Gets the soft pointers.
        /// </summary>
        public ObjectIdCollection SoftPointerIds { get; } = new ObjectIdCollection();

        /// <summary>
        /// Gets the hard pointers.
        /// </summary>
        public ObjectIdCollection HardPointerIds { get; } = new ObjectIdCollection();

        /// <summary>
        /// Gets the soft ownerships.
        /// </summary>
        public ObjectIdCollection SoftOwnershipIds { get; } = new ObjectIdCollection();

        /// <summary>
        /// Gets the hard ownerships.
        /// </summary>
        public ObjectIdCollection HardOwnershipIds { get; } = new ObjectIdCollection();

        /// <summary>
        /// Writes the hard ownership collection.
        /// </summary>
        /// <param name="id">Instance of ObjectId.</param>
        public override void WriteHardOwnershipId(ObjectId id)
        {
            if (!id.IsNull)
                HardOwnershipIds.Add(id);
        }

        /// <summary>
        /// Writes the hard pointer collection.
        /// </summary>
        /// <param name="id">Instance of ObjectId.</param>
        public override void WriteHardPointerId(ObjectId id)
        {
            if (!id.IsNull)
                HardPointerIds.Add(id);
        }

        /// <summary>
        /// Writes the soft ownership collection.
        /// </summary>
        /// <param name="id">Instance of ObjectId.</param>
        public override void WriteSoftOwnershipId(ObjectId id)
        {
            if (!id.IsNull)
                SoftOwnershipIds.Add(id);
        }

        /// <summary>
        /// Writes the soft pointer collection.
        /// </summary>
        /// <param name="id">Instance of ObjectId.</param>
        public override void WriteSoftPointerId(ObjectId id)
        {
            if (!id.IsNull)
                SoftPointerIds.Add(id);
        }

        #region Ignored overrides
        public override long Position => 1;
        public override FilerType FilerType => FilerType.IdFiler;
        public override ErrorStatus FilerStatus
        {
            get { return ErrorStatus.OK; }
            set { }
        }
        public override IntPtr ReadAddress() => IntPtr.Zero;
        public override byte[] ReadBinaryChunk() => new byte[0];
        public override bool ReadBoolean() => false;
        public override byte ReadByte() => 0;
        public override void ReadBytes(byte[] value) { }
        public override double ReadDouble() => 0.0;
        public override Handle ReadHandle() => new Handle();
        public override ObjectId ReadHardOwnershipId() => ObjectId.Null;
        public override ObjectId ReadHardPointerId() => ObjectId.Null;
        public override short ReadInt16() => 0;
        public override int ReadInt32() => 0;
        public override long ReadInt64() => 0;
        public override Point2d ReadPoint2d() => new Point2d();
        public override Point3d ReadPoint3d() => new Point3d();
        public override Scale3d ReadScale3d() => new Scale3d();
        public override ObjectId ReadSoftOwnershipId() => ObjectId.Null;
        public override ObjectId ReadSoftPointerId() => ObjectId.Null;
        public override string ReadString() => "";
        public override ushort ReadUInt16() => 0;
        public override uint ReadUInt32() => 0;
        public override ulong ReadUInt64() => 0;
        public override Vector2d ReadVector2d() => new Vector2d();
        public override Vector3d ReadVector3d() => default;
        public override void ResetFilerStatus() { }
        public override void Seek(long offset, int method) { }
        public override void WriteAddress(IntPtr value) { }
        public override void WriteBinaryChunk(byte[] chunk) { }
        public override void WriteBoolean(bool value) { }
        public override void WriteByte(byte value) { }
        public override void WriteBytes(byte[] value) { }
        public override void WriteDouble(double value) { }
        public override void WriteHandle(Handle handle) { }
        public override void WriteInt16(short value) { }
        public override void WriteInt32(int value) { }
        public override void WriteInt64(long value) { }
        public override void WritePoint2d(Point2d value) { }
        public override void WritePoint3d(Point3d value) { }
        public override void WriteScale3d(Scale3d value) { }
        public override void WriteString(string value) { }
        public override void WriteUInt16(ushort value) { }
        public override void WriteUInt32(uint value) { }
        public override void WriteUInt64(ulong value) { }
        public override void WriteVector2d(Vector2d value) { }
        public override void WriteVector3d(Vector3d value) { }
        #endregion
    }
}
