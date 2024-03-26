using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.LayerManager;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Type bounded to the items of the TreeView control.
    /// </summary>
    public class InspectableItem : ItemBase, INotifyPropertyChanged
    {
        #region INotitfyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        bool isExpanded, isSelected;
        /// <summary>
        /// Gets the children tree of the current item.
        /// </summary>
        public IEnumerable<InspectableItem> Children { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating if the node is exapanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; NotifyPropertyChanged(nameof(IsExpanded)); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the node is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; NotifyPropertyChanged(nameof(isSelected)); }
        }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Creates a new instance of InspectableItem
        /// </summary>
        /// <param name="value">Object to be inspected.</param>
        /// <param name="isSelected">Value indicating if the item is selected.</param>
        /// <param name="isExpanded">Value indicating if the item is expanded.</param>
        /// <param name="children">Children of the object.</param>
        /// <param name="name">Name of the object.</param>
        public InspectableItem(object value, bool isSelected = false, bool isExpanded = false, IEnumerable<InspectableItem> children = null, string name = null)
            : base(value)
        {
            Children = children;
            IsSelected = isSelected;
            IsExpanded = isExpanded;
            if (value is ObjectId id)
                Initialize(id, name);
            else if (value is LayerFilter filter)
                Initialize(filter);
            else
                Name = name ?? Label;
        }

        private void Initialize(LayerFilter filter)
        {
            Name = filter.Name;
            Children = filter.NestedFilters.Cast<LayerFilter>().Select(f => new InspectableItem(f));
        }

        private void Initialize(ObjectId id, string name)
        {
            using (var tr = id.Database.TransactionManager.StartTransaction())
            {
                var dbObj = tr.GetObject(id, OpenMode.ForRead);
                if (string.IsNullOrEmpty(name))
                {
                    Name = dbObj is SymbolTableRecord r ? r.Name : $"< {dbObj.GetType().Name} \t{dbObj.Handle} >";
                }
                else if (name == "<_>")
                {
                    Name = $"< {dbObj.GetType().Name} \t{dbObj.Handle} >";
                }
                else
                {
                    Name = name;
                }

                if (dbObj is SymbolTable table)
                {
                    Children = table
                        .Cast<ObjectId>()
                        .Select(x => new InspectableItem(x));
                }
                else if (dbObj is DBDictionary dict)
                {
                    if (id == id.Database.NamedObjectsDictionaryId)
                    {
                        Name = "Named Objects Dictionary";
                        IsExpanded = true;
                    }
                    Children = ((DBDictionary)dbObj)
                        .Cast<DictionaryEntry>()
                        .Select(e => new InspectableItem((ObjectId)e.Value, name: (string)e.Key));
                }
            }
        }
    }
}
