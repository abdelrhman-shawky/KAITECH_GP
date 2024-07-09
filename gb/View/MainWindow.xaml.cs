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
        /// Event handler for the FinishRoomsFloorsButton Click event.
        /// Executes the CreateFloorCommand defined in MainViewModel when the button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void FinishRoomsFloorsButton_Click(object sender, RoutedEventArgs e)
        {

            // Execute the command when the button is clicked
            if (MainViewModel.CreateFloorCommand.CanExecute(null))
            {
                MainViewModel.CreateFloorCommand.Execute(null);
            }

        }

    }

}
