﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: CommandClass(typeof(Gile.AutoCAD.R25.Inspector.Commands))]

namespace Gile.AutoCAD.R25.Inspector
{
    /// <summary>
    /// Defines AutoCAD commands.
    /// </summary>
    public class Commands
    {
        /// <summary>
        /// Inspects the Database contents of the active drawing.
        /// </summary>
        [CommandMethod("INSPECT_DATABASE", CommandFlags.Modal)]
        public static void InspectDatabase()
        {
            var db = HostApplicationServices.WorkingDatabase;
            new InspectorViewModel(db).ShowDialog();
        }

        /// <summary>
        /// Inspects the SymbolTable collection contents.
        /// </summary>
        [CommandMethod("INSPECT_TABLE", CommandFlags.Modal)]
        public static void InspectTable()
        {
            var db = HostApplicationServices.WorkingDatabase;
            //var ids = new ObjectIdCollection
            //{
            //    db.BlockTableId,
            //    db.DimStyleTableId,
            //    db.LayerTableId,
            //    db.LinetypeTableId,
            //    db.RegAppTableId,
            //    db.TextStyleTableId,
            //    db.UcsTableId,
            //    db.ViewTableId,
            //    db.ViewportTableId
            //};
            ObjectIdCollection ids =
                [
                    db.BlockTableId,
                    db.DimStyleTableId,
                    db.LayerTableId,
                    db.LinetypeTableId,
                    db.RegAppTableId,
                    db.TextStyleTableId,
                    db.UcsTableId,
                    db.ViewTableId,
                    db.ViewportTableId
                ];
            new InspectorViewModel(ids).ShowDialog();
        }

        /// <summary>
        /// Inspects the Named Object Dictionary contents.
        /// </summary>
        [CommandMethod("INSPECT_DICTIONARY", CommandFlags.Modal)]
        public static void InspectDictionary()
        {
            var db = HostApplicationServices.WorkingDatabase;
            new InspectorViewModel(db.NamedObjectsDictionaryId).ShowDialog();
        }

        /// <summary>
        /// Inspects the selected entities.
        /// </summary>
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

        /// <summary>
        /// Inspects the selected nested entity.
        /// </summary>
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
