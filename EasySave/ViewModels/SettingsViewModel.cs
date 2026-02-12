using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.ViewModels;

/// <summary>
/// ViewModel for the Settings view.
/// Handles application settings such as language, log format, and priority extensions.
/// </summary>
public class SettingsViewModel : ViewModelBase
{
    /// <summary>
    /// Command to close the settings view.
    /// </summary>
    public ICommand CloseCommand { get; }

    public SettingsViewModel()
    {
        CloseCommand = new RelayCommand(Close);
    }

    private void Close()
    {
        // Signal to close settings (will be handled by MainWindowViewModel)
        OnCloseRequested?.Invoke();
    }

    /// <summary>
    /// Event raised when the user wants to close the settings view.
    /// </summary>
    public event Action? OnCloseRequested;
}
