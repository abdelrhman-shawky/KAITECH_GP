using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gb.Model.RevitHelper
{
    public class RevitFilterCollectors
    {

        // Document field
        private readonly Document _document;

        // Constructor that initializes the document
        public RevitFilterCollectors(Document document)
        {
            _document = document;
        }

        /// <summary>
        /// Collects elements from the document based on the specified category and element type.
        /// </summary>
        /// <param name="category">The built-in category to filter.</param>
        /// <param name="elementType">True for element types, False for instances.</param>
        /// <returns>List of elements.</returns>
        private IList<Element> CollectElements(BuiltInCategory category, bool elementType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(_document).OfCategory(category);
            return elementType
                ? collector.WhereElementIsElementType().ToElements()
                : collector.WhereElementIsNotElementType().ToElements();
        }


        //----------------------------------Walls--------------------------------------//

        /// <summary>
        /// Collects walls as Elements from the document.
        /// </summary>
        /// <param name="elementType">True for element types, False for instances.</param>
        /// <returns>List of walls.</returns>
        public IList<Element> CollectWallsElements(bool elementType)
        {
            
            return CollectElements(BuiltInCategory.OST_Walls, elementType);
        }

        

        /// <summary>
        /// Collects walls Ids from the document.
        /// </summary>
        /// <param name="elementType">True for element types, False for instances.</param>
        /// <returns>List of walls.</returns>
        public IList<ElementId> CollectWallsIds(bool elementType)
        {

            return CollectWallsElements(elementType)
                .Cast<ElementId>()
                .ToList();
        }


        /// <summary>
        /// Collects walls from the document.
        /// </summary>
        /// <param name="elementType">True for element types, False for instances.</param>
        /// <returns>List of walls.</returns>
        public IList<Wall> CollectWalls(bool elementType)
        {

            return CollectWallsIds(elementType)
                .Cast<Wall>()
                .ToList();

        }

        //----------------------------------Floors--------------------------------------//

        /// <summary>
        /// Collects floors as Elements from the document.
        /// </summary>
        /// <param name="elementType">True for element types, False for instances.</param>
        /// <returns>List of floors.</returns>
        public IList<Element> CollectFloorsElements(bool elementType)
        {
            return CollectElements(BuiltInCategory.OST_Floors, elementType);
        }

        /// <summary>
        /// Collects floor IDs from the document.
        /// </summary>
        /// <param name="elementType">True for element types, False for instances.</param>
        /// <returns>List of floor IDs.</returns>
        public IList<ElementId> CollectFloorsIds(bool elementType)
        {
            IList<ElementId> ids = new List<ElementId>();

            return CollectFloorsElements(elementType)
                .Cast<ElementId>()
                .ToList();
                
            //foreach (Element element in CollectFloorsElements(elementType))
            //{
            //    ids.Add(element.Id);
            //}

            //return ids;
        }

        /// <summary>
        /// Collects floors from the document.
        /// </summary>
        /// <param name="elementType">True for element types, False for instances.</param>
        /// <returns>List of floors.</returns>
        public IList<Floor> CollectFloors(bool elementType)
        {
            IList<Floor> floors = new List<Floor>();

            foreach (ElementId floorId in CollectFloorsIds(elementType))
            {
                floors.Add(_document.GetElement(floorId) as Floor);
            }

            return floors;
        }

        //----------------------------------Columns--------------------------------------//

        /// <summary>
        /// Collects structural columns as elements from the document.
        /// </summary>
        /// <param name="elementType">True for element types, False for instances.</param>
        /// <returns>List of structural columns.</returns>
        public IList<Element> CollectStructuralColumnsElements(bool elementType)
        {
            return CollectElements(BuiltInCategory.OST_StructuralColumns, elementType);
        }

        /// <summary>
        /// Collects structural columns Ids from the document.
        /// </summary>
        /// <param name="elementType">True for element types, False for instances.</param>
        /// <returns>List of structural columns.</returns>
        public IList<ElementId> CollectStructuralColumnsElementsIds(bool elementType)
        {
            IList<ElementId> ids = new List<ElementId>();

            foreach (Element element in CollectWallsElements(elementType))
            {
                ids.Add(element.Id);
            }

            return ids;
        }


        //----------------------------------Rooms--------------------------------------//

        /// <summary>
        /// Collects rooms as elements from the document.
        /// </summary>
        /// <returns>List of rooms.</returns>
        public IList<Element> CollectRoomsElements()
        {
            var collector = new FilteredElementCollector(_document).OfCategory(BuiltInCategory.OST_Rooms);
            return collector.ToElements() ?? new List<Element>();
        }

        /// <summary>
        /// Collects rooms Ids from the document.
        /// </summary>
        /// <returns>List of rooms.</returns>
        public IList<ElementId> CollectRoomsElementsIds()
        {
            IList<ElementId> ids = new List<ElementId>();

            foreach (Element element in CollectRoomsElements())
            {
                ids.Add(element.Id);
            }

            return ids;
        }

        /// <summary>
        /// Collects rooms from the document.
        /// </summary>
        /// <returns>List of rooms.</returns>
        public IList<Room> CollectRooms()
        {
            IList<Room> rooms = new List<Room>();

            foreach (ElementId roomId in CollectRoomsElementsIds())
            {

                rooms.Add(_document.GetElement(roomId)as Room);
            }

            return rooms;
        }

    }
}
