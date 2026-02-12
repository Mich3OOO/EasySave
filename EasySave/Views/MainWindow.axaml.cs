using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EasySave.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    // Methode appelée lors du clic sur le bouton "Open Settings"
    public void OpenSettings_Click(object source, RoutedEventArgs args)
    {
        // crée la fenêtre de paramètres
        var settingsWindow = new SettingsWindow();

        // afficher par-dessus la fenêtre actuelle
        // "this" signifie que la MainWindow est le parent
        settingsWindow.ShowDialog(this);
    }
}