using Autodesk.Revit.DB;
using System.Collections.Generic;
using gb.Model.RevitHelper;
using gb.Model.Creation;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

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
        /// <param name="uiApplication"> revit ui application</param>
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
            GeneralHelperFunction generalHelperFunction =new GeneralHelperFunction(_document, revitFilterCollectors);

            IList<Room> rooms = revitFilterCollectors.CollectRooms();

            // Check if there are rooms in the document
            if (rooms.Count != 0)
            {
                // Initialize ElementCreation class to create floors
                ElementCreation elementCreation = new ElementCreation(_document, revitFilterCollectors, generalHelperFunction);

                // Create floors for each room
                foreach (Room room in rooms)
                {
                    if (room.LookupParameter("Unique").AsInteger() == 0)
                    {
                        elementCreation.CreateRoomFloorFromParam(room);
                    }
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
            GeneralHelperFunction generalHelperFunction = new GeneralHelperFunction(_document, revitFilterCollectors);

            IList<Room> rooms = revitFilterCollectors.CollectRooms();

            if (rooms.Count != 0)
            {
                // Initialize ElementCreation class to create Celing
                ElementCreation elementCreation = new ElementCreation(_document, revitFilterCollectors, generalHelperFunction);

                // Create Ceiling for each room
                foreach (Room room in rooms)
                {
                    if (room.LookupParameter("Unique").AsInteger() == 0)
                    {
                        elementCreation.createRoomCelinginFromParam(room); // for revit 2022+
                        //elementCreation.creatRoomCeilingFromParamOld(room); // for revit 2021 -
                    }
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
            GeneralHelperFunction generalHelperFunction = new GeneralHelperFunction(_document, revitFilterCollectors);

            IList<Room> rooms =revitFilterCollectors.CollectRooms();

            if (rooms.Count != 0)
            {
                // Initialize ElementCreation class to create floors
                ElementCreation elementCreation = new ElementCreation(_document, revitFilterCollectors, generalHelperFunction);

                // Create Wall for each room
                foreach (Room room in rooms)
                {
                    if (room.LookupParameter("Unique").AsInteger() == 0)
                    {
                        elementCreation.createRoomWallFromParam(room);
                    }
                }

            }
            else
            {
                // Show error message if no rooms are found
                TaskDialog.Show("Error", "No rooms found in the document.");
            }


        }

        /// <summary>
        /// Collects rooms parameters in the document and add parameters for each room.
        /// </summary>
        public void CreatePrameters()
        {

            List<string> stringParameters = new List<string>();
            List<string> boolParameters = new List<string>();
            List<string> doubleParameters = new List<string>();


            string celingHight = "Ceiling Height";   // double
            doubleParameters.Add(celingHight);

            

            string uniqueState = "Unique";          //bool
            boolParameters.Add(uniqueState);

            string wallHightLevel = "Wall Height Level";   //string
            stringParameters.Add(wallHightLevel);



            string furnishedState = "Furnished";     //bool //invs
            string isParameterExsists = "isParameterExsists"; //bool // invs

            ParameterCreation parameterCreation = new ParameterCreation(_uiApplication);


            foreach (string param in stringParameters)
            {
                parameterCreation.CreateOrUpdateRoomParameter(param, SpecTypeId.String.Text, GroupTypeId.IdentityData, true); // revit 2022+

                //parameterCreation.CreateOrUpdateRoomParameterOldVersion(param,ParameterType.Text,BuiltInParameterGroup.PG_IDENTITY_DATA, true); // revit 2021-
                
            }

            foreach (string param in boolParameters)
            {
                parameterCreation.CreateOrUpdateRoomParameter(param, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData, true);// revit 2022+
                //parameterCreation.CreateOrUpdateRoomParameterOldVersion(param, ParameterType.YesNo, BuiltInParameterGroup.PG_IDENTITY_DATA, true); //revit 2021-
            }


            //foreach (string param in doubleParameters)
            //{
            //    parameterCreation.CreateOrUpdateRoomParameter(param, SpecTypeId.Length, GroupTypeId.IdentityData, true);// revit 2022+
            //    parameterCreation.CreateOrUpdateRoomParameterOldVersion(param, ParameterType.Length, BuiltInParameterGroup.PG_IDENTITY_DATA, true);// revit 2021 -

            //}



            //-invis parameters
            parameterCreation.CreateOrUpdateRoomParameter(furnishedState, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData, false); // when everything is done  //revit 2022+
            //parameterCreation.CreateOrUpdateRoomParameterOldVersion(furnishedState, ParameterType.YesNo, BuiltInParameterGroup.PG_IDENTITY_DATA, false); //revit 2021-


            parameterCreation.CreateOrUpdateRoomParameter(isParameterExsists, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData, false); //all the rooms has the parameters //revit 2022+
            //parameterCreation.CreateOrUpdateRoomParameterOldVersion(isParameterExsists, ParameterType.YesNo, BuiltInParameterGroup.PG_IDENTITY_DATA, false); //revit 2021-
        }

      
    }
    


}
