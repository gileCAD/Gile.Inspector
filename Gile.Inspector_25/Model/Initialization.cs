using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Runtime;

using System;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(Gile.AutoCAD.Inspector.Initialization))]

namespace Gile.AutoCAD.Inspector
{
    /// <summary>
    /// Defines the application initialization.
    /// </summary>
    public class Initialization : IExtensionApplication
    {
        static InspectorContextMenu? contextMenu;

        /// <summary>
        /// Initializes the application, add the context menu.
        /// </summary>
        public void Initialize()
        {
            contextMenu = new InspectorContextMenu();
            AcAp.AddDefaultContextMenuExtension(contextMenu);
            Application.Idle += OnIdle;
        }

        private void OnIdle(object? sender, EventArgs e)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc != null)
            {
                Application.Idle -= OnIdle;
                doc.Editor.WriteMessage("\nGile.Inspector loaded.\n");
            }
        }

        /// <summary>
        /// Terminates the application.
        /// </summary>
        public void Terminate()
        {
            AcAp.RemoveDefaultContextMenuExtension(contextMenu);
        }
    }
}
