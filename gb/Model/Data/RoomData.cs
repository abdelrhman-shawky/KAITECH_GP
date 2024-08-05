using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gb.Model.Data
{
    public class RoomData
    {
        public string roomName { get; set; }
        public double roomNumber { get; set; }
        public string roomLevel { get; set; }
        public bool isUnique { get; set; }       
        public string roomId { get; set; }
    }
}
