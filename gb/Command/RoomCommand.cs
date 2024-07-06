using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using gb.Model.Creation;
using gb.Model.RevitHelper;

namespace gb
{
    [Transaction(TransactionMode.Manual)]
    public class RoomCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document document = uIDocument.Document;
            RevitFilterCollectors revitFilterCollectors = new RevitFilterCollectors(document);


            IList<Room> rooms = revitFilterCollectors.CollectRooms();
            if (rooms.Count == 0)
            {
                TaskDialog.Show("Error", "No rooms found in the document.");
                return Result.Failed;
            }

            ElementCreation elementCreation =new ElementCreation(document,revitFilterCollectors);

            elementCreation.CreateRoomFloorFromParam(rooms[1]);
            return Result.Succeeded;
        }
    }
}
