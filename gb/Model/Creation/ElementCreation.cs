//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
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

        SpatialElementBoundaryOptions boundaryOptions = new SpatialElementBoundaryOptions();


       public ElementCreation(Document document)
        {
            _document = document;
        }

        //IList<IList<BoundarySegment>>
    }
}
