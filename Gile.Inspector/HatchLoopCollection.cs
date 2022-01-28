using Autodesk.AutoCAD.DatabaseServices;

using System.Collections.Generic;

namespace Gile.AutoCAD.Inspector
{
    public class HatchLoopCollection
    {
        public List<HatchLoop> Loops { get; }
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
