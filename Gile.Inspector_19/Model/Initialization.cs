using Autodesk.AutoCAD.Runtime;

using System;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(Gile.AutoCAD.R19.Inspector.Initialization))]

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Defines the application initialization.
    /// </summary>
    public class Initialization : IExtensionApplication
    {
        static InspectorContextMenu contextMenu;

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public void Initialize()
        {
            contextMenu = new InspectorContextMenu();
            AcAp.AddDefaultContextMenuExtension(contextMenu);
            AcAp.Idle += OnIdle;
        }

        private void OnIdle(object sender, EventArgs e)
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
            AcAp.RemoveDefaultContextMenuExtension(contextMenu);
        }
    }
}
