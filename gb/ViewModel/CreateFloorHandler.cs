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
    public class CreateFloorHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uIDocument = app.ActiveUIDocument;
            Document doc = uIDocument.Document;

            TransactionManager transactionManager = new TransactionManager(doc);

            transactionManager.CreatFloor();
        }

        public string GetName()
        {
            return "CreatFloorHandler";
        }
    }
}
