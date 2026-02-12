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

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });

        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }
}