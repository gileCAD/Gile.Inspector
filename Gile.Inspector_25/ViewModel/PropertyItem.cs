using System;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Type bounded to the items in the ListView control.
    /// </summary>
    /// <remarks>
    /// Creates an new intance of PropertyItem.
    /// </remarks>
    /// <param name="name">Name of the property.</param>
    /// <param name="value">Value of the property.</param>
    /// <param name="baseType">Base type this property applies to</param>
    /// <param name="isInspectable">Value that indiquates if the property is inspaectable.</param>
    public class PropertyItem(string name, object? value, Type baseType, bool isInspectable) : ItemBase(value)
    {
        /// <summary>
        /// Gets a value indicating if the property is inspectable.
        /// </summary>
        public bool IsInspectable { get; } = isInspectable;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the base type this property applies to.
        /// </summary>
        public Type BaseType { get; } = baseType;
    }
}
