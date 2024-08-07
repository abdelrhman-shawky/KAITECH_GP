using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Collections.Generic;
using System.Linq;
using System; // revit 2021 & less

namespace gb.Model.RevitHelper
{
    public class GeneralHelperFunction
    {
        // Document field
        private readonly Document _document;
        RevitFilterCollectors _filterCollectors;


        public GeneralHelperFunction(Document document, RevitFilterCollectors filterCollectors)
        {
            _document = document;
            _filterCollectors = filterCollectors;
        }


        /// <summary>
        /// Retrive the level above a selected level by at least some distance.
        /// </summary>
        /// <param name="levels">List of all levels to search in.</param>
        /// <param name="selectedLevel">selected level e.g., Base level.</param>
        /// <param name="minHeightDifference">minimum height to be between levels.</param>
        /// <returns>A level that is above the selected level by a minimum height Difference.</returns>
        public Level GetNextLevelWithHeightDifference(IList<Level> levels, Level selectedLevel, double minHeightDifference)
        {
            // Get the elevation of the selected level
            double selectedElevation = selectedLevel.Elevation;

            // Sort the levels by elevation
            var sortedLevels = levels.OrderBy(l => l.Elevation).ToList();

            // Find the next level with a height difference greater than minHeightDifference
            foreach (var level in sortedLevels)
            {
                if (level.Elevation > selectedElevation + minHeightDifference)
                {
                    return level;
                }
            }

            // If no such level is found, return null
            return null;
        }


        /// <summary>
        /// Retrieves the base boundary curves of a room.
        /// </summary>
        /// <param name="room">The room from which to retrieve the boundary curves.</param>
        /// <returns>A list of CurveLoop objects representing the room's boundary curves.</returns>
        public IList<CurveLoop> GetRoomBaseCurve(Room room)
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
        /// joins between a wall and all the walls in a selected room.
        /// </summary>
        /// <param name="newWall">List of all levels to search in.</param>
        /// <param name="room">selected level e.g., Base level.</param>
        /// <returns>dose not return , only excute function.</returns>
        public void JoinWallsWithRoomBoundaries(Wall newWall, Room room)
        {
            IList<CurveLoop> curves = GetRoomBaseCurve(room);

            foreach (CurveLoop curveLoop in curves)
            {
                foreach (Curve curve in curveLoop)
                {

                    //Find all walls that intersect with the given curve

                    IList<Wall> wallElements = _filterCollectors.CollectWalls(true);


                    foreach (Wall existingWall in wallElements)
                    {
                        LocationCurve existingWallCurve = existingWall.Location as LocationCurve;

                        if (existingWallCurve != null)
                        {
                            Curve existingCurve = existingWallCurve.Curve;

                            // Check if the curve are close enough to be joind

                            if (AreCurvesIntersectingOrClose(existingCurve, curve))
                            {

                                JoinGeometryUtils.JoinGeometry(_document, newWall, existingWall);
                            }
                        }



                    }
                }
            }

        }

        /// <summary>
        /// Retrive a boolean value to indicate the intersection of closeness between 2 curves.
        /// </summary>
        /// <param name="curve1">Curve A.</param>
        /// <param name="curve2">Curve B.</param>
        /// <returns>A boolean value to indicat if the 2 curves are intersection or close</returns>
        public bool AreCurvesIntersectingOrClose(Curve curve1, Curve curve2)
        {
            // Check if curves intersect
            if (curve1.Intersect(curve2) == SetComparisonResult.Overlap)
            {
                return true;
            }

            // Check if curves are close
            XYZ p1 = curve1.GetEndPoint(0);
            XYZ p2 = curve1.GetEndPoint(1);
            XYZ q1 = curve2.GetEndPoint(0);
            XYZ q2 = curve2.GetEndPoint(1);

            double tolerance = 0.01; // Define a suitable tolerance value

            if (p1.DistanceTo(q1) < tolerance || p1.DistanceTo(q2) < tolerance ||
                p2.DistanceTo(q1) < tolerance || p2.DistanceTo(q2) < tolerance)
            {
                return true;
            }

            // Check if midpoints are close
            XYZ mid1 = (p1 + p2) / 2;
            XYZ mid2 = (q1 + q2) / 2;

            if (mid1.DistanceTo(mid2) < tolerance)
            {
                return true;
            }

            return false;
        }



        //---------------for Revit 2021 & less-----------------------------------//
        public FloorType GetFloorType(Element element, string floorTypeName)
        {
            // Ensure the input element is not null
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            // Ensure the input floor type name is not null or empty
            if (string.IsNullOrEmpty(floorTypeName))
            {
                throw new ArgumentNullException(nameof(floorTypeName));
            }

            // Cast the element to an ElementType
            var elementType = element as ElementType;

            // Check if the element is a FloorType and if its name matches the provided floor type name
            if (elementType is FloorType floorType && floorType.Name == floorTypeName)
            {
                return floorType;
            }

            // Return null if no matching FloorType is found
            return null;
        }



        public  CurveArray ConvertCurveLoopListToCurveArray(IList<CurveLoop> curveLoopList)
        {
            // Ensure the input IList<CurveLoop> is not null
            if (curveLoopList == null)
            {
                throw new ArgumentNullException(nameof(curveLoopList));
            }

            // Create a new CurveArray
            CurveArray curveArray = new CurveArray();

            // Iterate through the CurveLoop list
            foreach (CurveLoop curveLoop in curveLoopList)
            {
                // Ensure the CurveLoop is not null
                if (curveLoop != null)
                {
                    // Add curves from the CurveLoop to the CurveArray
                    foreach (Curve curve in curveLoop)
                    {
                        curveArray.Append(curve);
                    }
                }
            }

            return curveArray;
        }


        public  XYZ GetCentroid(IList<CurveLoop> curveLoops)
        {
            if (curveLoops == null || curveLoops.Count == 0)
            {
                throw new ArgumentException("CurveLoops list is null or empty.");
            }

            List<XYZ> points = new List<XYZ>();

            foreach (CurveLoop curveLoop in curveLoops)
            {
                foreach (Curve curve in curveLoop)
                {
                    // Sample points along the curve. Adjust the sampling density as needed.
                    double step = 0.1; // Sample every 0.1 units along the curve
                    for (double t = 0; t <= 1; t += step)
                    {
                        points.Add(curve.Evaluate(t, true));
                    }
                }
            }

            if (points.Count == 0)
            {
                throw new InvalidOperationException("No points found to calculate centroid.");
            }

            // Calculate the centroid
            XYZ centroid = new XYZ(
                points.Average(p => p.X),
                points.Average(p => p.Y),
                points.Average(p => p.Z)
            );

            return centroid;
        }
    }
}
