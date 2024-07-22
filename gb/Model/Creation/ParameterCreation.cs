using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            DefinitionFile definitionFile = _application.OpenSharedParameterFile();

            if (definitionFile == null )
            {
                TaskDialog.Show("Shared Parameter File Missing",
                    "The shared parameter file has no path." +
                    " Please go to the Manage tab, open Shared Parameters," +
                    " and assign a shared parameter file.");
                return;
            }

            DefinitionGroups definitionGroups =definitionFile.Groups;
            DefinitionGroup definitionGroup = definitionGroups.get_Item(groupName);

            //string groupName = "Room";

            if (definitionGroup == null)
            {
                definitionGroup = definitionGroups.Create(groupName);
            }


            Definition existingDefinition = definitionGroup.Definitions.get_Item(definitionName);

            if (existingDefinition != null)
            {
                using (Transaction transaction = new Transaction(_document,"Remove exisiting paramter"))
                {
                    transaction.Start();

                    BindingMap bindingMap =_document.ParameterBindings;                    
                    bindingMap.Remove(existingDefinition);
                    transaction.Commit();
                }

            }


            ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(definitionName, specTypeId)
            {
                Visible = visibelityState
            };

            Definition definition = existingDefinition ?? definitionGroup.Definitions.Create(options);

           // definition = definitionGroup.Definitions.Create(options);

            CategorySet categorySet = _application.Create.NewCategorySet();

            
            categorySet.Insert(Category.GetCategory(_document,builtInCategory));

            InstanceBinding instanceBinding =_application
                .Create.NewInstanceBinding(categorySet);


            using (Transaction transaction = new Transaction(_document, "Creating Parameters"))
            {
                transaction .Start();

                _document.ParameterBindings.Insert(
                    definition,
                    instanceBinding,
                    groupTypeId);

                transaction.Commit();
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
            CreateOrUpdateSharedParameter("Room",BuiltInCategory.OST_Rooms, definitionName, specTypeId, groupTypeId, visibilityState);
        }
    }
}
