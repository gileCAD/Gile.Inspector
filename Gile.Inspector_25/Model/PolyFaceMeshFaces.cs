using Autodesk.AutoCAD.DatabaseServices;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes a collection of PolyFaceMesh faces.
    /// </summary>
    public class PolyFaceMeshFaces
    {
        /// <summary>
        /// Gets the faces collection.
        /// </summary>
        public DBObjectCollection Faces { get; }

        /// <summary>
        /// Creates a new instance of PolyFaceMeshFaces.
        /// </summary>
        /// <param name="mesh">PolyFaceMesh instance.</param>
        public PolyFaceMeshFaces(PolyFaceMesh mesh)
        {
            Faces = [];
            foreach (var obj in mesh)
            {
                if (obj is ObjectId id && id.ObjectClass.Name.EndsWith("FaceRecord"))
                    Faces.Add((FaceRecord)id.GetObject(OpenMode.ForRead));
                else if (obj is FaceRecord face)
                    Faces.Add(face);
            }
        }
    }
}
