using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EasySave.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        // Plus tard, on sauvegardera les données ici
        this.Close();
    }
}