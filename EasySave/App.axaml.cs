using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using EasySave.ViewModels;
using EasySave.Views;

namespace EasySave;

public partial class App : Application  // The main application class for the EasySave application, it initializes the application and sets up the main window with its data context. It also includes a method to disable Avalonia's built-in data annotation validation to avoid conflicts with the CommunityToolkit's validation system.
{
    public override void Initialize()   // Override of the Initialize method to load the XAML for the application, it is called when the application starts and is used to set up the UI and resources for the application.
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()   // Override of the OnFrameworkInitializationCompleted method to set up the main window of the application, it checks if the application lifetime is a classic desktop style application, and if so, it disables Avalonia's built-in data annotation validation to avoid conflicts with the CommunityToolkit's validation system, and then it creates a new instance of the MainWindow class, sets its data context to a new instance of the MainWindowViewModel class, and assigns it to the MainWindow property of the desktop application lifetime. Finally, it calls the base implementation of the method to complete the initialization process.
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()  // Method to disable Avalonia's built-in data annotation validation, it retrieves an array of plugins that are of type DataAnnotationsValidationPlugin from the BindingPlugins.DataValidators collection, and then it removes each of those plugins from the collection to prevent them from being used for validation in the application, allowing the CommunityToolkit's validation system to handle all validation instead.
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}