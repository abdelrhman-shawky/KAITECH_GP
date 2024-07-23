//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using gb.Model.RevitHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
                TaskDialog.Show("parameteris missing", "floor Finish parameter is missing");
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

        /// <summary>
        /// Creats ceiling from rooms base boundry curves.
        /// </summary>
        /// <param room="Room">The built-in category to filter.</param>
        /// <returns>Ceiling ceiling.</returns>
        public Ceiling createRoomCelinginFromParam(Room room)
        {
            // Retrieve the base boundary curves of the room
            IList<CurveLoop> curves = GetRoomBaseCurve(room);

            // Lookup the "Ceiling Finish" parameter in the room
            Parameter ceilingTypeParam = room.LookupParameter("Ceiling Finish");

            // check if the Ceiling finis is null or not spicefied
            if (ceilingTypeParam.AsValueString() == null)
            {
                TaskDialog.Show("parameteris missing", "ceiling Finish parameter is missing");
                return null;
            }

            // Collect all Ceiling elements
            IList<Element> ceilingElment = _filterCollectors.collectCeilingElements(true);

            // Find the specific Ceiling type based on the parameter's value string
            Element specificCeilingType = ceilingElment.FirstOrDefault(e => e.Name == ceilingTypeParam.AsValueString());


            using (Transaction transaction = new Transaction(_document, "Create ceiling"))
            {
                transaction.Start();

                // Create the Ceiling using the specified Ceiling type and room level
                Ceiling myCeiling = Ceiling.Create(_document, curves, specificCeilingType.Id, room.LevelId);

                // Commit the transaction
                transaction.Commit();

                // Return the created Ceiling
                return myCeiling;
            }

        }


        /// <summary>
        /// Creats walls from rooms base boundry curves.
        /// </summary>
        /// <param room="Room">The built-in category to filter.</param>
        /// <returns>iList<Wall> Wall.</returns>
        public IList<Wall> createRoomWallFromParam(Room room)
        {
            // Retrieve the base boundary curves of the room
            IList<CurveLoop> curves = GetRoomBaseCurve(room);

            // Lookup the "Wall Finish" parameter in the room
            Parameter wallTypeParam = room.LookupParameter("Wall Finish");



            // check if the Wall finis is null or not spicefied
            if (wallTypeParam.AsValueString() == null)
            {
                TaskDialog.Show("parameteris missing", "wall Finish parameter is missing");
                return null;
            }

            // Collect all Wall elements
            IList<Element> wallElment = _filterCollectors.CollectWallsElements(true);

            
            // Find the specific wall type based on the parameter's value string
            Element specificWallType = wallElment.FirstOrDefault(e => e.Name == wallTypeParam.AsValueString());

            if (specificWallType == null)
            {
                TaskDialog.Show("Wall Type Missing", "The specified wall type was not found.");
                return null;
            }
            // double a=(specificCeilingType as Wall).Width / 2;


            


            IList<Wall> myWalls = new List<Wall>();

          

             using (Transaction transaction = new Transaction(_document, "Create ceiling"))
             {
                transaction.Start();

                foreach (CurveLoop curveLoop in curves)
                {

                    IList<Curve> curveList = curveLoop.ToList();

                    foreach (Curve curve in curveLoop)
                    {

                        // Calculate offset distance (half of wall width)
                        double offsetDistance = (specificWallType as Wall).Width / 2;


                        // curve.CreateOffset(offsetDistance, );

                        

                        Wall wall = Wall.Create(_document, curve, specificWallType.Id, room.LevelId,5000,0,false,false);
                        myWalls.Add(wall);
                    }


                }
                // Commit the transaction
                transaction.Commit();

                
                
             }
            // Return the created Ceiling
            return myWalls;



        }

    }
}
