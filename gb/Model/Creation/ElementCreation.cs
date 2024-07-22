//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using gb.Model.RevitHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gb.Model.Creation
{
    public class ElementCreation
    {
        private Document _document;

        RevitFilterCollectors _filterCollectors;

        /// <summary>
        /// ElementCreation is a class that is conserned with creating geomtry eg. floors,walls,celing etc...
        /// </summary>
        /// <param document="room">The room from which to retrieve the boundary curves.</param>
        /// <returns>ElementCreation instance.</returns>
        public ElementCreation(Document document, RevitFilterCollectors revitFilterCollectors)
        {
            _document = document;
            _filterCollectors = revitFilterCollectors;
        }

        /// <summary>
        /// Retrieves the base boundary curves of a room.
        /// </summary>
        /// <param name="room">The room from which to retrieve the boundary curves.</param>
        /// <returns>A list of CurveLoop objects representing the room's boundary curves.</returns>
        private IList<CurveLoop> GetRoomBaseCurve(Room room)
        {
            // Create a new SpatialElementBoundaryOptions object
            SpatialElementBoundaryOptions boundaryOptions = new SpatialElementBoundaryOptions();

            // Get the boundary segments of the room based on the boundary options
            IList<IList<BoundarySegment>> boundarySegments = room.GetBoundarySegments(boundaryOptions);

            // Initialize a list to hold the CurveLoop objects
            IList<CurveLoop> curves = new List<CurveLoop>();

            // Iterate through each list of boundary segments
            foreach (IList<BoundarySegment> segmentList in boundarySegments)
            {
                // Create a new CurveLoop for each set of boundary segments
                CurveLoop curveLoop = new CurveLoop();

                foreach (BoundarySegment segment in segmentList)
                {
                    // Get the curve from the boundary segment and append it to the CurveLoop
                    Curve curve = segment.GetCurve();
                    curveLoop.Append(curve);
                }
                curves.Add(curveLoop);
            }

            return curves;
        }


        /// <summary>
        /// Creats floor from rooms base boundry curves.
        /// </summary>
        /// <param room="Room">The built-in category to filter.</param>
        /// <returns>Floor floor.</returns>
        public Floor CreateRoomFloorFromParam(Room room)
        {
            // Retrieve the base boundary curves of the room
            IList<CurveLoop> curves =GetRoomBaseCurve(room);

            // Lookup the "Floor Finish" parameter in the room
            Parameter floorTypeParam = room.LookupParameter("Floor Finish");
           
            // check if the floor finis is null or not spicefied
            if (floorTypeParam.AsValueString()== null)
            {
                return null;
            }

            // Collect all floor elements
            IList<Element> FloorElment = _filterCollectors.CollectFloorsElements(true);


            // Find the specific floor type based on the parameter's value string
            Element specificFloorType = FloorElment.FirstOrDefault(e => e.Name ==floorTypeParam.AsValueString());



            // Start a transaction to create the floor
            using (Transaction transaction = new Transaction(_document, "Create Floor"))
            {
                transaction.Start();

                // Create the floor using the specified floor type and room level
                Floor myFloor = Floor.Create(_document, curves, specificFloorType.Id, room.LevelId);

                // Commit the transaction
                transaction.Commit();

                // Return the created floor
                return myFloor;
            }      
            

        }


        public Ceiling createRoomCelinginFromParam(Room room)
        {

            IList<CurveLoop> curves = GetRoomBaseCurve(room);


            Parameter ceilingTypeParam = room.LookupParameter("Ceiling Finish");

            if (ceilingTypeParam.AsValueString() == null)
            {
                return null;
            }

            IList<Element> ceilingElment = _filterCollectors.collectCeilingElements(true);


            Element specificCeilingType = ceilingElment.FirstOrDefault(e => e.Name == ceilingTypeParam.AsValueString());


            using (Transaction transaction = new Transaction(_document, "Create ceiling"))
            {
                transaction.Start();

                // Create the floor using the specified floor type and room level
                Ceiling myCeiling = Ceiling.Create(_document, curves, specificCeilingType.Id, room.LevelId);

                // Commit the transaction
                transaction.Commit();

                // Return the created floor
                return myCeiling;
            }

        }


    }
}
