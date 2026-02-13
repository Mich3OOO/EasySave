using System;
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
    /// Command to save the settings.
    /// </summary>
    public ICommand SaveCommand { get; }

    /// <summary>
    /// Command to cancel and close the settings view.
    /// </summary>
    public ICommand CancelCommand { get; }

    public SettingsViewModel()
    {
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Save()
    {
        // Signal to save settings (will be handled by MainWindowViewModel)
        OnSaveRequested?.Invoke();
    }

    private void Cancel()
    {
        // Signal to cancel settings (will be handled by MainWindowViewModel)
        OnCancelRequested?.Invoke();
    }

    /// <summary>
    /// Event raised when the user wants to save the settings.
    /// </summary>
    public event Action? OnSaveRequested;

    /// <summary>
    /// Event raised when the user wants to close the settings view without saving.
    /// </summary>
    public event Action? OnCancelRequested;
}