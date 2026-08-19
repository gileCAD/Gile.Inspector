using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

using Gile.AutoCAD.R25.Inspector.Model;

using System;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(Initialization))]

namespace Gile.AutoCAD.R25.Inspector.Model
{
    /// <summary>
    /// Defines the application initialization.
    /// </summary>
    public class Initialization : IExtensionApplication
    {
        static InspectorContextMenu? defaultContextMenu;
        static InspectorContextMenu? objectContextMenu;
        static readonly RXClass entityClass = RXObject.GetClass(typeof(Entity));

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public void Initialize()
        {
            defaultContextMenu = new InspectorContextMenu(true);
            AcAp.AddDefaultContextMenuExtension(defaultContextMenu);
            objectContextMenu = new InspectorContextMenu(false);
            AcAp.AddObjectContextMenuExtension(entityClass, objectContextMenu);
            Autodesk.AutoCAD.ApplicationServices.Core.Application.Idle += OnIdle;
        }

        private void OnIdle(object? sender, EventArgs e)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (doc != null)
            {
                Autodesk.AutoCAD.ApplicationServices.Core.Application.Idle -= OnIdle;
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
