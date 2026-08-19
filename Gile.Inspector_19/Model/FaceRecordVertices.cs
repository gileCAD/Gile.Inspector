using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Gile.AutoCAD.R19.Inspector.Model
{
    /// <summary>
    /// Describes the indices of a FaceRecord vertices.
    /// </summary>
    public class FaceRecordVertices
    {
        /// <summary>
        /// Gets the indices of the FaceRecord vertices.
        /// </summary>
        public Int32Collection VertexIndices { get; }

        /// <summary>
        /// Creates a new instance of FaceRecordVertices.
        /// </summary>
        /// <param name="mesh">FaceRecord instance.</param>
        public FaceRecordVertices(FaceRecord face)
        {
            VertexIndices = new Int32Collection(4);
            for (short i = 0; i < 4; i++)
            {
                VertexIndices.Add(face.GetVertexAt(i));
            }
        }
    }
}
