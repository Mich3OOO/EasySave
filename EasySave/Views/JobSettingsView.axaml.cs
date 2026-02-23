using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using EasySave.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasySave.Views;

public partial class JobSettingsView : UserControl
{
    public JobSettingsView()
    {
        InitializeComponent();
    }

    private async void SelectSource_Click(object? sender, RoutedEventArgs e)
    {
        var path = await OpenFolderPickerAsync("Sélectionner le dossier source");
        if (path != null && DataContext is JobSettingsViewModel vm)
        {
            vm.Source = path; // Le SetProperty du VM déclenchera la mise à jour UI
        }
    }

    private async void SelectDestination_Click(object? sender, RoutedEventArgs e)
    {
        var path = await OpenFolderPickerAsync("Sélectionner le dossier cible");
        if (path != null && DataContext is JobSettingsViewModel vm)
        {
            vm.Destination = path;
        }
    }

    private async Task<string?> OpenFolderPickerAsync(string title)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return null;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions  // use OpenFolderPickerAsync for folder selection
        {
            Title = title,
            AllowMultiple = false
        });

        if (folders.Count > 0)  // we check if at least one folder was selected
        {
            var uri = folders[0].Path;

            try
            {
                if (uri.IsAbsoluteUri)  // If it's a valid URI, we can extract the local path
                {
                    var path = uri.LocalPath;
                    return !string.IsNullOrEmpty(path) ? path : uri.AbsolutePath;
                }
            }
            catch (InvalidOperationException)
            {
                // If the path is not a valid URI, return it as is
            }

            return uri.ToString();
        }
        return null;
    }
}