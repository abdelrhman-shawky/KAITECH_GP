using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using gb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gb.ViewModel
{
    /// <summary>
    /// Handles an external event to execute Wall creation in a Revit document.
    /// Implements the IExternalEventHandler interface.
    /// </summary>
    public class CreateWallHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            // Get the active UIDocument and its associated Document
            UIDocument uIDocument = app.ActiveUIDocument;
            Document doc = uIDocument.Document;

            // Create a TransactionManager instance to manage transactions for floor creation
            TransactionManager transactionManager = new TransactionManager(doc, app);

            // Call the CreateFloor method of TransactionManager to create floors in the document
            transactionManager.CreateWall();
        }


        public string GetName()
        {
            return "createWallHandler";
        }
    }
}
