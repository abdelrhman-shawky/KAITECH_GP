using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using gb.Model.Creation;
using gb.Model.RevitHelper;
using gb.View;

namespace gb
{

    [Transaction(TransactionMode.Manual)]
    public class RoomCommand : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document document = uIDocument.Document;
            UIApplication uIApplication = commandData.Application;
            Application application = uIApplication.Application;


            MainWindow mainWindow = new MainWindow(uIDocument);
            mainWindow.Show();

            return Result.Succeeded;
        }
    }
}
