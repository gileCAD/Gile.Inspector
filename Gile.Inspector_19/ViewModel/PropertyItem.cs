using System;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Type bounded to the items in the ListView control.
    /// </summary>
    public class PropertyItem : ItemBase
    {
        /// <summary>
        /// Gets a value indicating if the property is inspectable.
        /// </summary>
        public bool IsInspectable { get; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the base type this property applies to.
        /// </summary>
        public Type BaseType { get; }

        /// <summary>
        /// Creates an new intance of PropertyItem.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <param name="baseType">Base type this property applies to</param>
        /// <param name="isInspectable">Value that indiquates if the property is inspaectable.</param>
        public PropertyItem(string name, object value, Type baseType, bool isInspectable) : base(value)
        {
            Name = name;
            BaseType = baseType;
            IsInspectable = isInspectable;
        }
    }
}
