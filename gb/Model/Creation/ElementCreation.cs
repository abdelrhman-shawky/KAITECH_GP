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


       public ElementCreation(Document document, RevitFilterCollectors revitFilterCollectors)
        {
            _document = document;
            _filterCollectors = revitFilterCollectors;
        }

        /// <summary>
        /// Collects elements from the document based on the specified category and element type.
        /// </summary>
        /// <param room="Room">The built-in category to filter.</param>
        /// <returns>List of CurveLoops.</returns>
        private IList<CurveLoop> GetRoomBaseCurve(Room room)
        {
            SpatialElementBoundaryOptions boundaryOptions = new SpatialElementBoundaryOptions();

            IList<IList<BoundarySegment>> boundarySegments = room.GetBoundarySegments(boundaryOptions);


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

            return curves;
        }



        public Floor CreateRoomFloorFromParam(Room room)
        {
            IList<CurveLoop> curves =GetRoomBaseCurve(room);

           Parameter floorTypeParam = room.LookupParameter("Floor Finish");

            
            IList<Element> FloorElment = _filterCollectors.CollectFloorsElements(true);

            Element specificFloorType= FloorElment.FirstOrDefault(e => e.Name ==floorTypeParam.AsValueString());

            var a =  floorTypeParam.Id;

            using (Transaction transaction = new Transaction(_document, "Create Floor"))
            {
                transaction.Start();

               Floor myFloor= Floor.Create(_document, curves, specificFloorType.Id, room.LevelId);
               
                transaction.Commit();
                return myFloor;
            }

            
        }


    }
}
