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
        GeneralHelperFunction _GeneralHelperFunction;

        /// <summary>
        /// ElementCreation is a class that is conserned with creating geomtry eg. floors,walls,celing etc...
        /// </summary>
        /// <param document="room">The room from which to retrieve the boundary curves.</param>
        /// <returns>ElementCreation instance.</returns>
        public ElementCreation(Document document, RevitFilterCollectors revitFilterCollectors, GeneralHelperFunction generalHelperFunction)
        {
            _document = document;
            _filterCollectors = revitFilterCollectors;
            _GeneralHelperFunction = generalHelperFunction;
        }



        /// <summary>
        /// Creats floor from rooms base boundry curves.
        /// </summary>
        /// <param room="Room">The built-in category to filter.</param>
        /// <returns>Floor floor.</returns>
        public Floor CreateRoomFloorFromParam(Room room)
        {            

            // Retrieve the base boundary curves of the room
            IList<CurveLoop> curves = _GeneralHelperFunction.GetRoomBaseCurve(room);

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
            IList<CurveLoop> curves = _GeneralHelperFunction.GetRoomBaseCurve(room);

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
            CeilingType specificCeilingType = ceilingElment.FirstOrDefault(e => e.Name == ceilingTypeParam.AsValueString()) as CeilingType;


            Parameter ceilingHeightParam = room.LookupParameter("Ceiling Height");
            double ceilingHeightOfsset = 0.0;

            if (ceilingHeightParam != null && ceilingHeightParam.AsDouble() > 0)
            {
                ceilingHeightOfsset=ceilingHeightParam.AsDouble();
            }

            using (Transaction transaction = new Transaction(_document, "Create ceiling"))
            {
                transaction.Start();

                // Create a new CeilingType to use for this ceiling with the specified height
                CeilingType newCeilingType = specificCeilingType.Duplicate(specificCeilingType.Name + "_Temp") as CeilingType;

                if (newCeilingType != null)
                {
                    newCeilingType.get_Parameter(BuiltInParameter.CEILING_HEIGHTABOVELEVEL_PARAM).Set(ceilingHeightOfsset);
                }
                

                // Create the Ceiling using the specified Ceiling type and room level
                Ceiling myCeiling = Ceiling.Create(_document, curves, newCeilingType.Id, room.LevelId);

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
            IList<CurveLoop> curves = _GeneralHelperFunction.GetRoomBaseCurve(room);

            // Lookup the "Wall Finish" parameter in the room
            Parameter wallTypeParam = room.LookupParameter("Wall Finish");

            // Check if the Wall Finish parameter is null or not specified
            if (wallTypeParam == null || wallTypeParam.AsValueString() == null)
            {
                TaskDialog.Show("parameteris missing", "wall Finish parameter is missing");
                return null;
            }

            // Collect all Wall elements
            IList<Element> wallElements = _filterCollectors.CollectWallsElements(true);


            // Find the specific wall type based on the parameter's value string
            WallType specificWallType = wallElements.OfType<WallType>().FirstOrDefault(e => e.Name == wallTypeParam.AsValueString());


            if (specificWallType == null)
            {
                TaskDialog.Show("Wall Type Missing", "The specified wall type was not found.");
                return null;
            }

            IList<Level> levels = _filterCollectors.collectLevels();

            // Retrieve the level name from the room parameter
            Parameter wallHeightLevelParam = room.LookupParameter("Wall Height Level");
            Level wallHeightLevel = null;

            if (wallHeightLevelParam != null && !string.IsNullOrWhiteSpace(wallHeightLevelParam.AsString()))
            {
                wallHeightLevel= levels.FirstOrDefault(l=> l.Name.Equals(wallHeightLevelParam.AsString(), StringComparison.OrdinalIgnoreCase));
            }

            Level nxtLevel= _GeneralHelperFunction.GetNextLevelWithHeightDifference(levels, room.Level, 1.8);

            double wallHeight = nxtLevel.Elevation;

             if(wallHeightLevel != null)
            {
                wallHeight = wallHeightLevel.Elevation;
            }

            // Create a list to store the created walls
            IList<Wall> myWalls = new List<Wall>();
            
             using (Transaction transaction = new Transaction(_document, "Create Walls"))
             {
                transaction.Start();

                foreach (CurveLoop curveLoop in curves)
                {

                    //IList<Curve> curveList = curveLoop.ToList();

                    foreach (Curve curve in curveLoop)
                    {
                        // Calculate offset distance (half of wall width)
                        double offsetDistance = specificWallType.Width / 2 ;

                        // Create an offset curve
                        XYZ direction = (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();

                        XYZ offsetVector = new XYZ(-direction.Y, direction.X, 0).Normalize() * offsetDistance;

                        Curve offsetCurve = curve.CreateTransformed(Transform.CreateTranslation(offsetVector));

                        // Create the wall using the offset curve
                        Wall newWall = Wall.Create(_document, offsetCurve, specificWallType.Id, room.LevelId, wallHeight, 0,false,false);
                        myWalls.Add(newWall);
                       
                    }
                }

                // Commit the transaction
                transaction.Commit();                                
             }


            //this dose not worrrkkkkk ---------------------------------------------------------!_!_!_!_!_!

            //using (Transaction transaction = new Transaction(_document, "Joining Walls"))
            //{
            //    transaction.Start();

            //    foreach (Wall wall in myWalls)
            //    {
            //        _GeneralHelperFunction.JoinWallsWithRoomBoundaries(wall, room);

            //    }
            //    transaction.Commit();
            //}


            // Return the created wall
            return myWalls;

        }

       


    }
}
