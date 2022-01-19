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
            new InspectorViewModel(db).ShowDialog();
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
            new InspectorViewModel(ids).ShowDialog();
        }

        [CommandMethod("INSPECT_DICTIONARY", CommandFlags.Modal)]
        public static void InspectDictionary()
        {
            var db = HostApplicationServices.WorkingDatabase;
            new InspectorViewModel(db.NamedObjectsDictionaryId).ShowDialog();
        }

        [CommandMethod("INSPECT_ENTITIES", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void InspectEntities()
        {
            var ed = AcAp.DocumentManager.MdiActiveDocument.Editor;
            var psr = ed.GetSelection();
            if (psr.Status == PromptStatus.OK)
            {
                new InspectorViewModel(new ObjectIdCollection(psr.Value.GetObjectIds())).ShowDialog();
            }
        }

        [CommandMethod("INSPECT_NENTITY", CommandFlags.Modal)]
        public static void InspectNestedEntity()
        {
            var ed = AcAp.DocumentManager.MdiActiveDocument.Editor;
            var per = ed.GetNestedEntity("\nSelect nested entity: ");
            if (per.Status == PromptStatus.OK)
            {
                new InspectorViewModel(per.ObjectId).ShowDialog();
            }
        }
    }
}
