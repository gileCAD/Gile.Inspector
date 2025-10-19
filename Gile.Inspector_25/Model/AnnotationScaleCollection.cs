using Autodesk.AutoCAD.DatabaseServices;

using System.Collections.Generic;
using System.Linq;

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Describes a collection of Annotation Scales.
    /// </summary>
    /// <remarks>
    /// Creates an new instance of AnnotationScales.
    /// </remarks>
    /// <param name="contextManager">ObjectContextManager instance.</param>
    public class AnnotationScaleCollection(ObjectContextManager contextManager)
    {
        /// <summary>
        /// Gets the Annotation Scales list.
        /// </summary>
        public List<AnnotationScale> AnnotationScales { get; } = [.. contextManager
                .GetContextCollection("ACDB_ANNOTATIONSCALES")
                .Cast<AnnotationScale>()];
    }
}
