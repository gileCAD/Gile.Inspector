using Autodesk.AutoCAD.DatabaseServices;

using System.Collections;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Base class for complex enities (DXF POLYLINE) vertices;
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    public abstract class ComplexEntityVertices<TEntity, TSubEntity>
        where TEntity : DBObject, IEnumerable
        where TSubEntity : DBObject
    {
        /// <summary>
        /// Gets the vertices collection.
        /// </summary>
        public DBObjectCollection Vertices { get; }

        /// <summary>
        /// Creates a new instance of PolygonMeshVertices.
        /// </summary>
        /// <param name="mesh">PolygonMesh instance.</param>
        public ComplexEntityVertices(TEntity mesh)
        {
            Vertices = new DBObjectCollection();
            foreach (var obj in mesh)
            {
                if (obj is ObjectId id && id.ObjectClass.Name.EndsWith("Vertex"))
                    Vertices.Add((TSubEntity)id.GetObject(OpenMode.ForRead));
                else if (obj is TSubEntity subEntity)
                    Vertices.Add(subEntity);
            }
        }
    }
}
