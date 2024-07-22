using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using gb.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace gb.Model
{

    /// <summary>
    /// View model class that facilitates user interactions and commands for floor creation in Revit.
    /// Implements INotifyPropertyChanged to notify UI of property changes.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {

        private readonly ExternalEvent createFloorEvent;
        private readonly CreateFloorHandler createFloorHandler;

        private readonly ExternalEvent createCeilingEvent;
        private readonly CreateCeilingHandler createCeilingHandler;


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// Sets up command bindings and event handlers for floor creation.
        /// </summary>
        public MainViewModel()
        {

            //Initialize the createFloorHandler and create an ExternalEvent for it 
            createFloorHandler = new CreateFloorHandler();
            createFloorEvent = ExternalEvent.Create(createFloorHandler);

            createCeilingHandler = new CreateCeilingHandler();
            createCeilingEvent = ExternalEvent.Create(createCeilingHandler);

            //create a RelayCommand for the CreateFloorCommand
            CreateFloorCommand = new RelayCommand(CreateFloor);

            CreateCeilingCommand = new RelayCommand(CreateCeiling);


        }


        /// <summary>
        /// ICommand property bound to a UI element (e.g., button) to create floors.
        /// </summary>
        public ICommand CreateFloorCommand { get; }

        public ICommand CreateCeilingCommand { get; }

        /// <summary>
        /// Method called when the CreateFloorCommand is executed.
        /// Raises the ExternalEvent to execute the CreateFloorHandler.
        /// </summary>
        private void CreateFloor()
        {
            // Raise the ExternalEvent to execute the CreateFloorHandler

            createFloorEvent.Raise();
        }


        private void CreateCeiling()
        {
            createCeilingEvent.Raise();
        }


        /// <summary>
        /// Implementation of INotifyPropertyChanged for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Helper method to invoke property change events.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


    /// <summary>
    /// Implements the ICommand interface to provide command execution logic for UI elements.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;


        /// <summary>
        /// Initializes a new instance of the RelayCommand class with the specified execute and canExecute delegates.
        /// </summary>
        /// <param name="execute">The action delegate to execute the command logic.</param>
        /// <param name="canExecute">The function delegate to determine if the command can execute.</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute; // Action delegate to execute the command logic
            this.canExecute = canExecute; // Func<bool> delegate to determine if the command can execute
        }


        /// <summary>
        /// Determines if the command can execute.
        /// </summary>
        /// <param name="parameter">Command parameter (not used in this implementation).</param>
        /// <returns>True if the command can execute, false otherwise.</returns>
        public bool CanExecute(object parameter) => canExecute == null || canExecute();

        /// <summary>
        /// Executes the command logic.
        /// </summary>
        /// <param name="parameter">Command parameter (not used in this implementation).</param>
        public void Execute(object parameter) => execute();

        /// <summary>
        /// Event that notifies changes in the command's ability to execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; } // Subscribe to CommandManager's RequerySuggested event
            remove { CommandManager.RequerySuggested -= value; } // Unsubscribe from CommandManager's RequerySuggested event
        }
    }
}
