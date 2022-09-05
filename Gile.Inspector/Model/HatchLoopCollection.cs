using Autodesk.AutoCAD.DatabaseServices;

using System.Collections.Generic;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Describes a collection of hatch loops.
    /// </summary>
    public class HatchLoopCollection
    {
        /// <summary>
        /// Gets the loops for the current hatch.
        /// </summary>
        public List<HatchLoop> Loops { get; }

        /// <summary>
        /// Creates a nes instance of HatchLoopCollection;
        /// </summary>
        /// <param name="hatch">Hatch instance.</param>
        public HatchLoopCollection(Hatch hatch)
        {
            Loops = new List<HatchLoop>();
            for (int i = 0; i < hatch.NumberOfLoops; i++)
            {
                Loops.Add(hatch.GetLoopAt(i));
            }
        }
    }
}
