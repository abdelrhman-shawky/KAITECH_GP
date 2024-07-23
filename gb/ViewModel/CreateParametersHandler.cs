using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using gb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gb.ViewModel
{
    internal class CreateParametersHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            // Get the active UIDocument and its associated Document
            UIDocument uIDocument = app.ActiveUIDocument;
            Document doc = uIDocument.Document;

            // Create a TransactionManager instance to manage transactions for floor creation
            TransactionManager transactionManager = new TransactionManager(doc,app);

            // Call the CreateFloor method of TransactionManager to create floors in the document
            transactionManager.CreatePrameters();
        }

        public string GetName()
        {
            return "CreateParametersHandler";
        }
    }
}
