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

namespace gb.Model
{
    public class TransactionManager
    {
        private readonly Document _document;



        public TransactionManager(Document document)
        {
        _document = document; 

        }


        public void CreatFloor()
        {
            RevitFilterCollectors revitFilterCollectors = new RevitFilterCollectors(_document);

            IList<Room> rooms = revitFilterCollectors.CollectRooms();

            if (rooms.Count != 0)
            {
                ElementCreation elementCreation = new ElementCreation(_document, revitFilterCollectors);

                foreach (Room room in rooms)
                {
                    elementCreation.CreateRoomFloorFromParam(room);
                }

            }
            else {
                TaskDialog.Show("Error", "No rooms found in the document.");
            }

        }
    }
    


}
