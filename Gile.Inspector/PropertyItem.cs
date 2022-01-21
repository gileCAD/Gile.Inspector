using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.LayerManager;
using Autodesk.AutoCAD.Runtime;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Type bounded to the items in the ListView control.
    /// </summary>
    public class PropertyItem : ItemBase
    {
        public bool IsInspectable { get; }

        public string Name { get; }

        public Type SubType { get; }

        public PropertyItem(string name, object value, Type subType, bool isInspectable) : base(value)
        {
            Name = name;
            SubType = subType;
            IsInspectable = isInspectable;
        }
    }
}
