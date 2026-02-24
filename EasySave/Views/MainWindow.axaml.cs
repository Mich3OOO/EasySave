using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using EasySave.ViewModels;

namespace EasySave.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    //Event handler for when User click on Checkbox, will call RefreshSelectionStatus in MainWindowViewModel to update status of selected jobss
    private void OnCheckBoxClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            // Refresh the selection status of the jobs in the viewmodel
            vm.RefreshSelectionStatus();
        }
    }
}
