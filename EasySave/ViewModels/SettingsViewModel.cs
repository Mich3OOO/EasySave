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
    public LanguageViewModel LanguageViewModel { get; }
    public string T_settings => LanguageViewModel.GetTranslation("settings_menu");
    public string T_language_selection => LanguageViewModel.GetTranslation("language_selection");
    public string T_logs_format => LanguageViewModel.GetTranslation("logs_format");
    public string T_business_software => LanguageViewModel.GetTranslation("business_software");
    public string T_business_software_detected => LanguageViewModel.GetTranslation("business_software_detected");
    public string T_extensions_to_encrypt => LanguageViewModel.GetTranslation("extensions_to_encrypt");
    public string T_save_and_quit => LanguageViewModel.GetTranslation("save_and_quit");

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
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "dictionary.json");
        LanguageViewModel = new LanguageViewModel(dictionaryPath);
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