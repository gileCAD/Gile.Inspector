using System;

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
