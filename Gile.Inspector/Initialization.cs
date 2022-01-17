using Autodesk.AutoCAD.Runtime;

using System;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(Gile.AutoCAD.Inspector.Initialization))]

namespace Gile.AutoCAD.Inspector
{
    public class Initialization : IExtensionApplication
    {
        static InspectorContextMenu contextMenu;

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

        public void Terminate()
        {
            AcAp.RemoveDefaultContextMenuExtension(contextMenu);
        }
    }
}
