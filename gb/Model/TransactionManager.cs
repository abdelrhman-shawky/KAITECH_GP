using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gb.Model.RevitHelper;
using gb.Model.Creation;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace gb.Model
{
    /// <summary>
    /// Manages transactions for creating floors in a Revit document.
    /// </summary>
    public class TransactionManager
    {
        private readonly Document _document;
        private readonly UIApplication _uiApplication;

        /// <summary>
        /// Initializes a new instance of the TransactionManager class with the specified Revit document.
        /// </summary>
        /// <param name="document">The Revit document to perform operations on.</param>
        public TransactionManager(Document document, UIApplication uiApplication)
        {
            _uiApplication = uiApplication;
            _document = document; 

        }

        /// <summary>
        /// Collects rooms in the document and creates floor for each room.
        /// </summary>
        public void CreateFloor()
        {
            // Collect rooms using RevitFilterCollectors helper class
            RevitFilterCollectors revitFilterCollectors = new RevitFilterCollectors(_document);

            IList<Room> rooms = revitFilterCollectors.CollectRooms();

            // Check if there are rooms in the document
            if (rooms.Count != 0)
            {
                // Initialize ElementCreation class to create floors
                ElementCreation elementCreation = new ElementCreation(_document, revitFilterCollectors);

                // Create floors for each room
                foreach (Room room in rooms)
                {
                    elementCreation.CreateRoomFloorFromParam(room);
                }

            }
            else {
                // Show error message if no rooms are found
                TaskDialog.Show("Error", "No rooms found in the document.");
            }

        }

        /// <summary>
        /// Collects rooms in the document and creates Ceiling for each room.
        /// </summary>
        public void CreateCeiling()
        {
            RevitFilterCollectors revitFilterCollectors = new RevitFilterCollectors(_document);

            IList<Room> rooms = revitFilterCollectors.CollectRooms();

            if (rooms.Count != 0)
            {
                // Initialize ElementCreation class to create floors
                ElementCreation elementCreation = new ElementCreation(_document, revitFilterCollectors);

                // Create Ceiling for each room
                foreach (Room room in rooms)
                {
                    elementCreation.createRoomCelinginFromParam(room);
                }

            }
            else
            {
                // Show error message if no rooms are found
                TaskDialog.Show("Error", "No rooms found in the document.");
            }
            
        }

        /// <summary>
        /// Collects rooms in the document and creates Walls for each room.
        /// </summary>
        public void CreateWall()
        {
            RevitFilterCollectors revitFilterCollectors = new RevitFilterCollectors (_document);

            IList<Room> rooms =revitFilterCollectors.CollectRooms();

            if (rooms.Count != 0)
            {
                // Initialize ElementCreation class to create floors
                ElementCreation elementCreation = new ElementCreation(_document, revitFilterCollectors);

                // Create Wall for each room
                foreach (Room room in rooms)
                {
                    elementCreation.createRoomWallFromParam(room);
                }

            }
            else
            {
                // Show error message if no rooms are found
                TaskDialog.Show("Error", "No rooms found in the document.");
            }


        }

        public void CreatePrameters()
        {
            string celingHight = "Ceiling Height";   // double
            string furnishedState = "Furnished";     //bool
            string uniqueState = "Unique";          //bool
            string isParameterExsists = "isParameterExsists"; //bool

            ParameterCreation parameterCreation = new ParameterCreation(_uiApplication);

            parameterCreation.CreateOrUpdateRoomParameter(celingHight, SpecTypeId.Length, GroupTypeId.IdentityData, true);
            parameterCreation.CreateOrUpdateRoomParameter(uniqueState, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData, true);
            parameterCreation.CreateOrUpdateRoomParameter(furnishedState, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData, false); // when everything is done

            parameterCreation.CreateOrUpdateRoomParameter(isParameterExsists, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData, false); //all the rooms has the parameters
        }
    }
    


}
