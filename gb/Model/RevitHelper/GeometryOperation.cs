using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gb.Model.RevitHelper
{
    public class GeometryOperation
    {
        // Document field
        private readonly Document _document;

        RevitFilterCollectors _filterCollectors;

        GeometryOperation(Document document, RevitFilterCollectors filterCollectors)
        {
            _document = document; 
            _filterCollectors = filterCollectors;

        }


        //public Curve OffsetCurve(Curve curve, double offsetDistance)
        //{
        //    if (curve is Line line)
        //    {
        //        // Handle line curves
        //        XYZ direction = (line.GetEndPoint(1) - line.GetEndPoint(0)).Normalize();
        //        XYZ normal = XYZ.BasisZ; // Assume vertical offset; adjust if necessary
        //        XYZ offset = normal * offsetDistance;

        //        // Create new line offset from the original
        //        XYZ startPoint = line.GetEndPoint(0) + offset;
        //        XYZ endPoint = line.GetEndPoint(1) + offset;
        //        return Line.CreateBound(startPoint, endPoint) as Curve;
        //    }

        //    else if (curve is Arc arc)
        //    {
        //        // Calculate the new radius
        //        double newRadius = arc.Radius + offsetDistance;

        //        // Get the start and end points of the arc
        //        XYZ center = arc.Center;
        //        XYZ startPoint = arc.GetEndPoint(0);
        //        XYZ endPoint = arc.GetEndPoint(1);

        //        // Calculate angles
        //        double startAngle = Math.Atan2(startPoint.Y - center.Y, startPoint.X - center.X);
        //        double endAngle = Math.Atan2(endPoint.Y - center.Y, endPoint.X - center.X);

        //        // Create the new offset arc
        //        return Arc.Create(center, newRadius, startAngle, endAngle, arc.Plane) as Curve ;
        //    }
        //    else
        //    {
        //        return null;
        //    }
    }
    
}

