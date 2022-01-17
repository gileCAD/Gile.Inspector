using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: CommandClass(typeof(Gile.AutoCAD.Inspector.Commands))]

namespace Gile.AutoCAD.Inspector
{
    public class Commands
    {
        [CommandMethod("INSPECT_DATABASE", CommandFlags.Modal)]
        public static void InspectDatabase()
        {
            var db = HostApplicationServices.WorkingDatabase;
            InspectorViewModel.ShowDialog(db);
        }

        [CommandMethod("INSPECT_TABLE", CommandFlags.Modal)]
        public static void InspectTable()
        {
            var db = HostApplicationServices.WorkingDatabase;
            var ids = new ObjectIdCollection();
            using (var tr = db.TransactionManager.StartOpenCloseTransaction())
            {
                ids.Add(db.BlockTableId);
                ids.Add(db.DimStyleTableId);
                ids.Add(db.LayerTableId);
                ids.Add(db.LinetypeTableId);
                ids.Add(db.RegAppTableId);
                ids.Add(db.TextStyleTableId);
                ids.Add(db.UcsTableId);
                ids.Add(db.ViewTableId);
                ids.Add(db.ViewportTableId);
                tr.Commit();
            }
            InspectorViewModel.ShowDialog(ids);
        }

        [CommandMethod("INSPECT_DICTIONARY", CommandFlags.Modal)]
        public static void InspectDictionary()
        {
            var db = HostApplicationServices.WorkingDatabase;
            InspectorViewModel.ShowDialog(db.NamedObjectsDictionaryId);
        }

        [CommandMethod("INSPECT_ENTITIES", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void InspectEntities()
        {
            var ed = AcAp.DocumentManager.MdiActiveDocument.Editor;
            var psr = ed.GetSelection();
            if (psr.Status == PromptStatus.OK)
            {
                InspectorViewModel.ShowDialog(new ObjectIdCollection(psr.Value.GetObjectIds()));
            }
        }

        [CommandMethod("INSPECT_NENTITY", CommandFlags.Modal)]
        public static void InspectNestedEntity()
        {
            var ed = AcAp.DocumentManager.MdiActiveDocument.Editor;
            var per = ed.GetNestedEntity("\nSelect nested entity: ");
            if (per.Status == PromptStatus.OK)
            {
                InspectorViewModel.ShowDialog(per.ObjectId);
            }
        }
    }
}
