using Autodesk.AutoCAD.DatabaseServices;

using System.Collections.Generic;
using System.Linq;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Describes a collection of Annotation Scales.
    /// </summary>
    public class AnnotationScaleCollection
    {
        /// <summary>
        /// Gets the Annotation Scales list.
        /// </summary>
        public List<AnnotationScale> AnnotationScales { get; }

        /// <summary>
        /// Creates an new instance of AnnotationScales.
        /// </summary>
        /// <param name="contextManager">ObjectContextManager instance.</param>
        public AnnotationScaleCollection(ObjectContextManager contextManager)
        {
            AnnotationScales = contextManager
                .GetContextCollection("ACDB_ANNOTATIONSCALES")
                .Cast<AnnotationScale>()
                .ToList();
        }
    }
}
