using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;

using System;

namespace Gile.AutoCAD.Inspector
{
    class InspectorContextMenu : ContextMenuExtension
    {
        private static Document ActiveDocument => Application.DocumentManager.MdiActiveDocument;

        public InspectorContextMenu()
        {
            Title = "Inspector";
            var itemEntities = new MenuItem("Entities...");
            itemEntities.Click += ItemEntities_Click;
            MenuItems.Add(itemEntities);
            var itemNEntity = new MenuItem("Nested entity...");
            itemNEntity.Click += ItemNEntity_Click;
            MenuItems.Add(itemNEntity);
            var itemDatabase = new MenuItem("Database...");
            itemDatabase.Click += ItemDatabase_Click;
            MenuItems.Add(itemDatabase);
            var itemTables = new MenuItem("Tables...");
            itemTables.Click += ItemTables_Click;
            MenuItems.Add(itemTables);
            var itemDictionaries = new MenuItem("Dictionaries...");
            itemDictionaries.Click += ItemDict_Click;
            MenuItems.Add(itemDictionaries);
        }

        private static void ItemDatabase_Click(object sender, EventArgs e)
        {
            ActiveDocument?.SendStringToExecute("INSPECT_DATABASE\n", false, false, false);
        }

        private static void ItemTables_Click(object sender, EventArgs e)
        {
            ActiveDocument?.SendStringToExecute("INSPECT_TABLE\n", false, false, false);
        }

        private static void ItemDict_Click(object sender, EventArgs e)
        {
            ActiveDocument?.SendStringToExecute("INSPECT_DICTIONARY\n", false, false, false);
        }

        private static void ItemEntities_Click(object sender, EventArgs e)
        {
            ActiveDocument?.SendStringToExecute("INSPECT_ENTITIES\n", false, false, false);
        }

        private static void ItemNEntity_Click(object sender, EventArgs e)
        {
            ActiveDocument?.SendStringToExecute("INSPECT_NENTITY\n", false, false, false);
        }

    }
}
