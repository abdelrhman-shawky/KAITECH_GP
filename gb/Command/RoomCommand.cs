using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
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

            SpatialElementBoundaryOptions boundaryOptions = new SpatialElementBoundaryOptions();
            IList<IList<BoundarySegment>> boundarySegments = rooms[0].GetBoundarySegments(boundaryOptions);
            if (boundarySegments.Count == 0)
            {
                TaskDialog.Show("Error", "No boundary segments found for the first room.");
                return Result.Failed;
            }

            IList<CurveLoop> curves = new List<CurveLoop>();
            foreach (IList<BoundarySegment> segmentList in boundarySegments)
            {
                CurveLoop curveLoop = new CurveLoop();
                foreach (BoundarySegment segment in segmentList)
                {
                    Curve curve = segment.GetCurve();
                    curveLoop.Append(curve);
                }
                curves.Add(curveLoop);
            }

            IList<Element> floorTypes = revitFilterCollectors.CollectFloorsElements(true);
            Element specificFloorType = floorTypes.FirstOrDefault(e => e.Name == "Concrete 250mm");
            if (specificFloorType == null)
            {
                TaskDialog.Show("Error", "Floor type 'Concrete 250mm' not found.");
                return Result.Failed;
            }

            using (Transaction transaction = new Transaction(document, "Create Floor"))
            {
                transaction.Start();

                Floor.Create(document, curves, specificFloorType.Id, rooms[0].LevelId);

                transaction.Commit();
            }

            Parameter nameParameter = rooms[0].LookupParameter("Name");
            if (nameParameter != null)
            {
                string roomName = nameParameter.AsString();
                TaskDialog.Show("Room Name", $"Room Name: {roomName}");
            }
            else
            {
                TaskDialog.Show("Room Name", "Room name parameter not found.");
            }

            return Result.Succeeded;
        }
    }
}
