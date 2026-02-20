using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;
using Avalonia;
using Avalonia.Styling;

namespace EasySave.ViewModels;

/// <summary>
/// ViewModel for the Settings view.
/// Handles application settings such as language, log format, and priority extensions.
/// </summary>
public class SettingsViewModel : ViewModelBase
{
    public LanguageViewModel _languageViewModel { get; }
    public string T_settings => _languageViewModel.GetTranslation("settings_menu");
    public string T_language_selection => _languageViewModel.GetTranslation("language_selection");
    public string T_logs_format => _languageViewModel.GetTranslation("logs_format");
    public string T_business_software => _languageViewModel.GetTranslation("business_software");
    public string T_business_software_detected => _languageViewModel.GetTranslation("business_software_detected");
    public string T_extensions_to_encrypt => _languageViewModel.GetTranslation("extensions_to_encrypt");
    public string T_save_and_quit => _languageViewModel.GetTranslation("save_and_quit");
    public string T_cancel => _languageViewModel.GetTranslation("cancel");

    /// <summary>
    /// Command to save the settings.
    /// </summary>
    public ICommand SaveCommand { get; }

    /// <summary>
    /// Command to cancel and close the settings view.
    /// </summary>
    public ICommand CancelCommand { get; }

    public Languages SelectedLanguage { get; set; }
    public List<Languages> LanguagesList { get; set; }

    public LogsFormats SelectedLogsFormats { get; set; }
    public List<LogsFormats> LogsFormatsList { get; set; }

    public string Extension { get; set; }
    public string Softwares { get; set; }

    private bool _isDarkMode;
    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
                OnPropertyChanged(nameof(IsDarkMode));

                // Apply new theme
                if (Application.Current != null)
                {
                    Application.Current.RequestedThemeVariant = value ? ThemeVariant.Dark : ThemeVariant.Light;
                }
            }
        }
    }

    private Config _config = Config.S_GetInstance();

    public SettingsViewModel()
    {
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "dictionary.json");
        _languageViewModel = LanguageViewModel.GetInstance(dictionaryPath);

        SelectedLanguage = _config.Language;
        SelectedLogsFormats = _config.LogsFormat;
        Extension = string.Join(",", _config.ExtensionsToEncrypt);
        Softwares = string.Join(",", _config.Softwares);

        LanguagesList = new List<Languages>(Languages.GetValuesAsUnderlyingType<Languages>().Cast<Languages>().ToArray());
        LogsFormatsList = new List<LogsFormats>(LogsFormats.GetValuesAsUnderlyingType<LogsFormats>().Cast<LogsFormats>().ToArray());

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);

        // Update UI Switch with the current value
        if (Application.Current != null)
        {
            _isDarkMode = Application.Current.RequestedThemeVariant == ThemeVariant.Dark;
        }
    }

    private void Save()
    {
        _languageViewModel.SetLanguage(SelectedLanguage);
        _config.Language = SelectedLanguage;
        _config.LogsFormat = SelectedLogsFormats;
        _config.ExtensionsToEncrypt = Extension.Split(',');
        _config.Softwares = Softwares.Split(',');
        _config.SaveConfig();

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