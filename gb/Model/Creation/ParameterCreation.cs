using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace gb.Model.Creation
{
    public class ParameterCreation
    {

        private readonly UIApplication _uiApplication;
        private readonly Application _application;
        private readonly Document _document;

        public ParameterCreation(UIApplication uiApplication)
        {
            _uiApplication = uiApplication;
            _application = _uiApplication.Application;
            _document = _uiApplication.ActiveUIDocument.Document;

        }

        /// <summary>
        /// Creates or updates a shared parameter.
        /// </summary>
        /// <param name="groupName">The group name for the parameter.</param>
        /// <param name="definitionName">The name of the parameter definition.</param>
        /// <param name="builtInCategory">the Built in category (e.g.,Wall,Room,Floor,....)</param>
        /// <param name="specTypeId">The type of the parameter (e.g., Boolean).</param>
        /// <param name="groupTypeId">The group type ID for the parameter, (e.g., Identity data, Phasing)</param>
        /// <param name="visibilityState">Visibility state of the parameter.</param>
        private void CreateOrUpdateSharedParameter(string groupName,
            BuiltInCategory builtInCategory,
            string definitionName,
            ForgeTypeId specTypeId,
            ForgeTypeId groupTypeId,
            bool visibelityState=true)
        {

            // Open the shared parameter file from the application.
            DefinitionFile definitionFile = _application.OpenSharedParameterFile();


            // If the shared parameter file is missing, display a task dialog with instructions.
            if (definitionFile == null )
            {
                TaskDialog.Show("Shared Parameter File Missing",
                    "The shared parameter file has no path." +
                    " Please go to the Manage tab, open Shared Parameters," +
                    " and assign a shared parameter file.");
                return; // Exit the function as there's no shared parameter file to work with.
            }

            // Retrieve the groups of definitions from the shared parameter file.
            DefinitionGroups definitionGroups =definitionFile.Groups;

            // Retrieve the specific group of definitions by name.
            // If the group does not exist, create it.
            DefinitionGroup definitionGroup = definitionGroups.get_Item(groupName);

            //string groupName = "Room";

            if (definitionGroup == null)
            {
                definitionGroup = definitionGroups.Create(groupName);
            }

            // Retrieve the existing definition from the group by name.
            // If it exists, remove it.
            Definition existingDefinition = definitionGroup.Definitions.get_Item(definitionName);

            if (existingDefinition != null)
            {
                // Start a transaction to remove the existing parameter.
                using (Transaction transaction = new Transaction(_document,"Remove exisiting paramter"))
                {
                    transaction.Start();

                    // Get the binding map of the document and remove the existing definition.
                    BindingMap bindingMap =_document.ParameterBindings;                    
                    bindingMap.Remove(existingDefinition);
                    transaction.Commit(); // Commit the transaction to apply changes.
                }

            }

            // Create options for a new external definition with the specified name and type.
            // Set the visibility state of the new definition.
            ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(definitionName, specTypeId)
            {
                Visible = visibelityState
            };

            // Create the definition, either using the existing one or creating a new one.
            Definition definition = existingDefinition ?? definitionGroup.Definitions.Create(options);


            // Create a new category set and add the specified category to it.
            CategorySet categorySet = _application.Create.NewCategorySet();
          
            categorySet.Insert(Category.GetCategory(_document,builtInCategory));

            // Create an instance binding for the new parameter.
            InstanceBinding instanceBinding =_application
                .Create.NewInstanceBinding(categorySet);

            // Start a transaction to create the new parameter and bind it to the category.
            using (Transaction transaction = new Transaction(_document, "Creating Parameters"))
            {
                transaction .Start();

                // Insert the new parameter binding into the document's parameter bindings.
                _document.ParameterBindings.Insert(
                    definition,
                    instanceBinding,
                    groupTypeId);

                transaction.Commit(); // Commit the transaction to apply changes.
            }
        }


        /// <summary>
        /// Creates or updates a room parameter.
        /// </summary>
        /// <param name="definitionName">The name of the room parameter.</param>
        /// <param name="specTypeId">The type of the room parameter (e.g., boolean).</param>
        /// <param name="groupTypeId">The group type ID for the parameter, (e.g., Identity data, Phasing)</param>
        /// <param name="visibilityState">Visibility state of the parameter.</param>
        public void CreateOrUpdateRoomParameter(string definitionName,ForgeTypeId specTypeId,ForgeTypeId groupTypeId,bool visibilityState)
        {
            // Calls the helper method to create or update the shared parameter.
            CreateOrUpdateSharedParameter("Room",BuiltInCategory.OST_Rooms, definitionName, specTypeId, groupTypeId, visibilityState);
        }




        

    }
}
