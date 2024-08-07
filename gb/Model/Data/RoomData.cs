
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.ComponentModel;

namespace gb.Model.Data
{

    public class RoomData: INotifyPropertyChanged
    {
        private string _roomName { get; set; }
        private string _roomNumber { get; set; }
        private string _roomLevel { get; set; }
        private bool _isUnique { get; set; }
        private string _roomId { get; set; }
        private string _ceilingFinish { get; set; }
        private string _floorFinish { get; set; }
        private string _wallFinish { get; set; }
        private double _wallHeightLevel { get; set; }

        private Room _room; // Reference to the original Revit Room

        /// <summary>
        /// Event triggered when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Initializes a new instance of the RoomData class.
        /// </summary>
        /// <param name="room">The Revit room object.</param>
        public RoomData(Room room)
        {
            _room = room;
            RoomName = room.get_Parameter(BuiltInParameter.ROOM_NAME).AsValueString();
            RoomNumber = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsValueString();
            RoomId = room.Id.ToString();

            ElementId levelId = room.LevelId;
            Level level = room.Document.GetElement(levelId) as Level;
            RoomLevel = level?.Name ?? "Unknown";

            Parameter uniqueParam = room.LookupParameter("Unique");
            IsUnique = uniqueParam != null && uniqueParam.AsInteger() == 1;

            CeilingFinish = room.LookupParameter("Ceiling Finish")?.AsString();
            FloorFinish = room.LookupParameter("Floor Finish")?.AsString();
            WallFinish = room.LookupParameter("Wall Finish")?.AsString();
            WallHeightLevel = room.LookupParameter("Wall Height Level")?.AsDouble() ?? 0.0;
        }

        /// <summary>
        /// Gets or sets the room name.
        /// </summary>
        public string RoomName
        {
            get { return _roomName; }
            set
            {
                if (_roomName != value)
                {
                    _roomName = value;
                    OnPropertyChanged(nameof(RoomName));

                    UpdateRoomParameter(BuiltInParameter.ROOM_NAME, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the room number.
        /// </summary>
        public string RoomNumber
        {
            get { return _roomNumber; }
            set
            {
                if (_roomNumber != value)
                {
                    _roomNumber = value;
                    OnPropertyChanged(nameof(RoomNumber));
                    UpdateRoomParameter(BuiltInParameter.ROOM_NUMBER, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the room ID.
        /// </summary>
        public string RoomId
        {
            get { return _roomId; }
            set
            {
                if (_roomId != value)
                {
                    _roomId = value;
                    OnPropertyChanged(nameof(RoomId));
                }
            }
        }

        /// <summary>
        /// Gets or sets the room level.
        /// </summary>
        public string RoomLevel
        {
            get { return _roomLevel; }
            set
            {
                if (_roomLevel != value)
                {
                    _roomLevel = value;
                    OnPropertyChanged(nameof(RoomLevel));
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the room is unique.
        /// </summary>
        public bool IsUnique
        {
            get { return _isUnique; }
            set
            {
                if (_isUnique != value)
                {
                    _isUnique = value;
                    OnPropertyChanged(nameof(IsUnique));
                    UpdateRoomParameter("Unique", value ? 1 : 0);
                }
            }
        }


        public string CeilingFinish
        {
            get { return _ceilingFinish; }
            set
            {
                if (_ceilingFinish != value)
                {
                    _ceilingFinish = value;
                    OnPropertyChanged(nameof(CeilingFinish));
                    UpdateRoomParameter("Ceiling Finish", value);
                }
            }
        }

        public string FloorFinish
        {
            get { return _floorFinish; }
            set
            {
                if (_floorFinish != value)
                {
                    _floorFinish = value;
                    OnPropertyChanged(nameof(FloorFinish));
                    UpdateRoomParameter("Floor Finish", value);
                }
            }
        }

        public string WallFinish
        {
            get { return _wallFinish; }
            set
            {
                if (_wallFinish != value)
                {
                    _wallFinish = value;
                    OnPropertyChanged(nameof(WallFinish));
                    UpdateRoomParameter("Wall Finish", value);
                }
            }
        }

        public double WallHeightLevel
        {
            get { return _wallHeightLevel; }
            set
            {
                if (_wallHeightLevel != value)
                {
                    _wallHeightLevel = value;
                    OnPropertyChanged(nameof(WallHeightLevel));
                    UpdateRoomParameter("Wall Height Level", value.ToString());
                }
            }
        }

        /// <summary>
        /// Updates the room parameter with a new string value.
        /// </summary>
        /// <param name="parameter">The built-in parameter.</param>
        /// <param name="value">The new value.</param>
        private void UpdateRoomParameter(BuiltInParameter parameter, string value)
        {
            _room.get_Parameter(parameter).Set(value);
        }


        /// <summary>
        /// Updates the room parameter with a new string value.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The new value.</param>
        private void UpdateRoomParameter(string parameterName, string value)
        {
            // Find the parameter by name
            Parameter param = _room.LookupParameter(parameterName);
            if (param != null && param.StorageType == StorageType.String)
            {
                param.Set(value);
            }
            else
            {
                throw new InvalidOperationException($"Parameter '{parameterName}' not found or is not of type String.");
            }
        }

        /// <summary>
        /// Updates the room parameter with a new integer value.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The new value.</param>
        private void UpdateRoomParameter(string parameterName, int value)
        {
            _room.LookupParameter(parameterName).Set(value);
        }

        /// <summary>
        /// Updates the room parameter with a new double value.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The new value.</param>
        private void UpdateRoomParameter(string parameterName, double value)
        {
            _room.LookupParameter(parameterName)?.Set(value);
        }

        /// <summary>
        /// Invokes the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        
    }
}
