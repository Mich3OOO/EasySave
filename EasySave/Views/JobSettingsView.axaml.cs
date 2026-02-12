using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using EasySave.ViewModels;
using System.Linq;
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
            vm.Source = path;
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
        // On récupère la fenêtre principale pour afficher la boîte de dialogue
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return null;

        // Ouvre l'explorateur de dossiers
        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });

        // Retourne le chemin local si un dossier a été choisi
        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }
}