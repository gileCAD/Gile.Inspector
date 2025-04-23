using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

using System;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(Gile.AutoCAD.R25.Inspector.Initialization))]

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Defines the application initialization.
    /// </summary>
    public class Initialization : IExtensionApplication
    {
        static InspectorContextMenu? defaultContextMenu;
        static InspectorContextMenu? objectContextMenu;
        static RXClass entityClass = RXObject.GetClass(typeof(Entity));

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public void Initialize()
        {
            defaultContextMenu = new InspectorContextMenu(true);
            AcAp.AddDefaultContextMenuExtension(defaultContextMenu);
            objectContextMenu = new InspectorContextMenu(false);
            AcAp.AddObjectContextMenuExtension(entityClass, objectContextMenu);
            AcAp.Idle += OnIdle;
        }

        private void OnIdle(object? sender, EventArgs e)
        {
            var doc = AcAp.DocumentManager.MdiActiveDocument;
            if (doc != null)
            {
                AcAp.Idle -= OnIdle;
                doc.Editor.WriteMessage("\nGile.Inspector loaded.\n");
            }
        }

        /// <summary>
        /// Terminates the application.
        /// </summary>
        public void Terminate()
        {
            AcAp.RemoveDefaultContextMenuExtension(defaultContextMenu);
            AcAp.RemoveObjectContextMenuExtension(entityClass, objectContextMenu);
        }
    }
}
