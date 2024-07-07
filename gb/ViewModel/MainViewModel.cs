using Autodesk.Revit.Creation;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace gb.Model
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private ExternalEvent createFloorEvent;

        private CreateFloorHandler createFloorHandler;


        public MainViewModel()
        {

            //Initialize the createFloorHandler and create an ExternalEvent for it 
            createFloorHandler = new CreateFloorHandler();

            createFloorEvent = ExternalEvent.Create(createFloorHandler);

            //create a RelayCommand for the CreateFloorCommand
            CreateFloorCommand = new RelayCommand(CreatFloor);

        }


        //ICommand property bound to a UI element (e.g., button)
        public ICommand CreateFloorCommand { get; }


        // Method that gets called when the CreateFloorCommand is executed
        private void CreatFloor()
        {
            // Raise the ExternalEvent to execute the CreateFloorHandler

            createFloorEvent.Raise();
        }


        // Implementation of INotifyPropertyChanged for property change notifications

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


    // RelayCommand class that implements ICommand for command handling
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        // Constructor to initialize the RelayCommand with execute and canExecute delegates
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute; // Action delegate to execute the command logic
            this.canExecute = canExecute; // Func<bool> delegate to determine if the command can execute
        }

        // ICommand method to determine if the command can execute
        public bool CanExecute(object parameter) => canExecute == null || canExecute();

        // ICommand method to execute the command logic
        public void Execute(object parameter) => execute();

        // ICommand event to notify changes in the command's ability to execute
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; } // Subscribe to CommandManager's RequerySuggested event
            remove { CommandManager.RequerySuggested -= value; } // Unsubscribe from CommandManager's RequerySuggested event
        }
    }
}
