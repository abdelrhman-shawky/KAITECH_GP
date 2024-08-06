using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace gb
{
    public class RoomExternalApplication : IExternalApplication
    {
        private static readonly string TabName = "KAITECH-BD-R06";
        private static readonly string PanelName = "Architecture";
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();


        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                application.CreateRibbonTab(TabName);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Failed to create ribbon tab: {ex.Message}");
                return Result.Failed;
            }

            RibbonPanel architecturePanel;
            try
            {
                architecturePanel = application.CreateRibbonPanel(TabName, PanelName);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Failed to create ribbon panel: {ex.Message}");
                return Result.Failed;
            }

            string roomCommandDescription = "This command applies finishes as geometry using finishes parameters in the room parameter. " +
                                            "Walls + Ceiling + Floors";

            try
            {
                CreateButton("Room",
                    "Room Finisher",
                    typeof(RoomCommand).FullName,
                    $"{nameof(gb)}.References.Resources.room-32.png",
                    architecturePanel,
                    roomCommandDescription);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Failed to create button: {ex.Message}");
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Creates a button on a given ribbon panel.
        /// </summary>
        /// <param name="buttonText">The text to display on the button.</param>
        /// <param name="buttonName">The internal name of the button.</param>
        /// <param name="commandNamespacePath">The namespace path of the command to be executed.</param>
        /// <param name="iconPath">The path to the icon to display on the button.</param>
        /// <param name="panel">The ribbon panel to add the button to.</param>
        /// <param name="tooltip">The tooltip to display when the button is hovered over.</param>
        private PushButton CreateButton(string buttonName, string buttonText, string className, string imageUrl, RibbonPanel panel, string description = null)
        {
            PushButtonData buttonData = new PushButtonData(buttonName, buttonText, Assembly.Location, className);
            PushButton pushButton = panel.AddItem(buttonData) as PushButton;

            if (pushButton == null)
            {
                throw new InvalidOperationException("Failed to add push button to the ribbon panel.");
            }

            if (!string.IsNullOrEmpty(description))
            {
                pushButton.ToolTip = description;
            }

            BitmapImage image = new BitmapImage();
            Stream stream = Assembly.GetManifestResourceStream(imageUrl);

            if (stream == null)
            {
                throw new InvalidOperationException($"Failed to load image resource: {imageUrl}");
            }

            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            pushButton.LargeImage = image;

            return pushButton;
        }
    }


}
