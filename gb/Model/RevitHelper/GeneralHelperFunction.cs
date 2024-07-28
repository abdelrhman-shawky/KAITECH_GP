using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using gb.Model.Creation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
