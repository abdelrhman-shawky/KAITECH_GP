using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using gb.Model;
using gb.Model.Data;
using gb.Model.RevitHelper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;


namespace gb.View
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public UIDocument uiDoc { get; }
        public Document document { get; }
        public MainViewModel MainViewModel { get; }


        /// <summary>
        /// Initializes a new instance of the MainWindow class with the provided UIDocument.
        /// Sets up the UI and initializes MainViewModel as the DataContext.
        /// </summary>
        /// <param name="uIDocument">The UIDocument instance representing the active Revit document.</param>
        public MainWindow(UIDocument uIDocument)
        {
            uiDoc = uIDocument;

            document= uiDoc.Document;


            InitializeComponent();

            LoadRoomData();
            // Initialize the MainViewModel and set it as the DataContext for data binding
            MainViewModel = new MainViewModel();
            DataContext = MainViewModel;



        }


        /// <summary>
        /// Event handler for the placeCeilingButton_Click event.
        /// Executes the CreateCeilingCommand defined in MainViewModel when the button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void placeCeilingButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainViewModel.CreateCeilingCommand.CanExecute(null))
            {
                MainViewModel.CreateCeilingCommand.Execute(null);
            }
        }



        /// <summary>
        /// Event handler for the placeWallButton_Click event.
        /// Executes the CreateWallCommand defined in MainViewModel when the button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void placeWallButton_Click(object sender, RoutedEventArgs e)
        {
            // Execute the command when the button is clicked
            if (MainViewModel.CreateWallCommand.CanExecute(null))
            {
                MainViewModel.CreateWallCommand.Execute(null);
            }
        }


        /// <summary>
        /// Event handler for the FinishRoomsFloorsButton Click event.
        /// Executes the CreateFloorCommand defined in MainViewModel when the button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void placeFloorsButton_Click(object sender, RoutedEventArgs e)
        {
            // Execute the command when the button is clicked
            if (MainViewModel.CreateFloorCommand.CanExecute(null))
            {
                MainViewModel.CreateFloorCommand.Execute(null);
            }
        }


        /// <summary>
        /// Event handler for the setParametersButton Click event.
        /// Executes the CreateParameterCommand defined in MainViewModel when the button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void setParametersButton_Click(object sender, RoutedEventArgs e)
        {
            // Execute the command when the button is clicked
            if (MainViewModel.CreateParameterCommand.CanExecute(null))
            {
                MainViewModel.CreateParameterCommand.Execute(null);
            }
        }


        /// <summary>
        /// Loads room data from the Revit document and populates an ObservableCollection with the data.
        /// The collection is then set as the ItemsSource for the RoomsDataGrid.
        /// </summary>
        private void LoadRoomData()
        {
            // Initialize an empty ObservableCollection to hold the room data.
            ObservableCollection<RoomData> roomDataList = new ObservableCollection<RoomData>();

            // Create an instance of the RevitFilterCollectors class to collect rooms from the document.
            RevitFilterCollectors revitFilterCollectors = new RevitFilterCollectors(document);

            // Collect all rooms in the document.
            IList<Room> rooms = revitFilterCollectors.CollectRooms();

            // Iterate over each room in the collected rooms.
            foreach (Room room in rooms)
            {
                // Create a new RoomData object and populate it with room details.
                RoomData roomData = new RoomData(room);

                // Add the RoomData object to the collection.
                roomDataList.Add(roomData);

            }

            // Set the ItemsSource of the RoomsDataGrid to the populated collection.
            RoomsDataGrid.ItemsSource = roomDataList;
        }

    
    }

}
