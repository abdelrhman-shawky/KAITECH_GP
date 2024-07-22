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

        public MainWindow(UIDocument uIDocument)
        {
            uiDoc = uIDocument;

            document= uiDoc.Document;
            InitializeComponent();
            
        }

        private void FinishRoomsFloorsButton_Click(object sender, RoutedEventArgs e)
        {
            
            MainViewModel mainViewModel = new MainViewModel();

        }

<<<<<<< Updated upstream
=======

        private void FinishRoomsCeilingButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainViewModel.CreateCeilingCommand.CanExecute(null))
            {
                MainViewModel.CreateCeilingCommand.Execute(null);
            }
        }


>>>>>>> Stashed changes
    }

}
