using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using gb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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



        private void setParametersButton_Click(object sender, RoutedEventArgs e)
        {
            // Execute the command when the button is clicked
            if (MainViewModel.CreateParameterCommand.CanExecute(null))
            {
                MainViewModel.CreateParameterCommand.Execute(null);
            }
        }
    }

}
