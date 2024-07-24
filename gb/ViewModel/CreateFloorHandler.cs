using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using gb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gb
{
    /// <summary>
    /// Handles an external event to execute floor creation in a Revit document.
    /// Implements the IExternalEventHandler interface.
    /// </summary>
    public class CreateFloorHandler : IExternalEventHandler
    {

        /// <summary>
        /// Executes the floor creation process when the external event is triggered.
        /// </summary>
        /// <param name="app">The UIApplication object representing the current Revit application.</param>
        public void Execute(UIApplication app)
        {
            // Get the active UIDocument and its associated Document
            UIDocument uIDocument = app.ActiveUIDocument;
            Document doc = uIDocument.Document;

            // Create a TransactionManager instance to manage transactions for floor creation
            TransactionManager transactionManager = new TransactionManager(doc,app);

            // Call the CreateFloor method of TransactionManager to create floors in the document
            transactionManager.CreateFloor();
        }

        /// <summary>
        /// Returns the name of the external event handler.
        /// </summary>
        /// <returns>The name of the external event handler ("CreateFloorHandler").</returns>
        public string GetName()
        {
            return "CreatFloorHandler";
        }
    }
}
