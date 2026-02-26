using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;
using Avalonia;
using Avalonia.Styling;
using EasySave.Models.Exepctions;

namespace EasySave.ViewModels;

/// <summary>
/// ViewModel for the Settings view.
/// Handles application settings such as language, log format, and priority extensions.
/// </summary>
public class SettingsViewModel : ObservableObject
{
    public LanguageViewModel LanguageViewModel { get; } = LanguageViewModel.GetInstance();

    public string T_settings => LanguageViewModel.GetTranslation("settings_menu");
    public string T_language_selection => LanguageViewModel.GetTranslation("language_selection");
    public string T_logs_format => LanguageViewModel.GetTranslation("logs_format");
    public string T_logs_mods => LanguageViewModel.GetTranslation("logs_mods");
    public string T_business_software => LanguageViewModel.GetTranslation("business_software");
    public string T_business_software_detected => LanguageViewModel.GetTranslation("business_software_detected");
    public string T_extensions_to_encrypt => LanguageViewModel.GetTranslation("extensions_to_encrypt");
    public string T_save_and_quit => LanguageViewModel.GetTranslation("save_and_quit");
    public string T_cancel => LanguageViewModel.GetTranslation("cancel");
    public string T_size_file => LanguageViewModel.GetTranslation("file_size");
    public string T_explain_file_size => LanguageViewModel.GetTranslation("explain_file_size");
    public string T_dark_mode => LanguageViewModel.GetTranslation("dark_mode");
    public string T_light_mode => LanguageViewModel.GetTranslation("light_mode");
    public string T_api_url => LanguageViewModel.GetTranslation("api_url");
    public string T_critical_extensions => LanguageViewModel.GetTranslation("critical_extensions");
    public string ErrorMessage { get; set; } = "";
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

    public string Extension { get; set; } = "";
    public string Softwares { get; set; } = "";
    public string CriticalExtensions { get; set; } = "";
    public string API_URL { get; set; } = "";

    private LogsMods _selectedLogsMods;
    public LogsMods SelectedLogsMods
    {
        get => _selectedLogsMods;
        set
        {
            if (_selectedLogsMods != value)
            {
                _selectedLogsMods = value;
                OnPropertyChanged(nameof(SelectedLogsMods));
            }
        }
    }

    private List<LogsModsItem> _logsModsList = [];
    public List<LogsModsItem> LogsModsList
    {
        get => _logsModsList;
        set
        {
            if (_logsModsList != value)
            {
                _logsModsList = value;
                OnPropertyChanged(nameof(LogsModsList));
            }
        }
    }

    private LogsModsItem? _selectedLogsModsItem;
    public LogsModsItem? SelectedLogsModsItem
    {
        get => _selectedLogsModsItem;
        set
        {
            if (_selectedLogsModsItem != value)
            {
                _selectedLogsModsItem = value;
                if (value != null)
                    SelectedLogsMods = value.Value;
                OnPropertyChanged(nameof(SelectedLogsModsItem));
            }
        }
    }

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

                if (Application.Current != null)
                {
                    Application.Current.RequestedThemeVariant = value ? ThemeVariant.Dark : ThemeVariant.Light;
                }
            }
        }
    }

    private uint? _maxParallelLargeFileSizeKo;
    public uint? MaxParallelLargeFileSizeKo
    {
        get => _maxParallelLargeFileSizeKo;
        set
        {
            if (_maxParallelLargeFileSizeKo != value)
            {
                _maxParallelLargeFileSizeKo = value;
                OnPropertyChanged(nameof(MaxParallelLargeFileSizeKo));
            }
        }
    }

    private readonly Config _config = Config.GetInstance();

    public SettingsViewModel()
    {
        SelectedLanguage = _config.Language;
        SelectedLogsFormats = _config.LogsFormat;
        SelectedLogsMods = _config.LogsMods;
        Extension = string.Join(",", _config.ExtensionsToEncrypt);
        Softwares = string.Join(",", _config.Softwares);
        CriticalExtensions = string.Join(",", _config.CriticalExtensions);
        API_URL = _config.API_URL;

        MaxParallelLargeFileSizeKo = _config.MaxParallelLargeFileSizeKo;

        LanguagesList = [.. Languages.GetValuesAsUnderlyingType<Languages>().Cast<Languages>().ToArray()];
        LogsFormatsList = [.. LogsFormats.GetValuesAsUnderlyingType<LogsFormats>().Cast<LogsFormats>().ToArray()];
        LogsModsList = [.. Enum.GetValues(typeof(LogsMods)).Cast<LogsMods>().Select(m => new LogsModsItem(m, GetLogsModTranslation(m)))];
        
        SelectedLogsModsItem = LogsModsList.FirstOrDefault(i => i.Value == _config.LogsMods);

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);

        if (Application.Current != null)
        {
            _isDarkMode = Application.Current.RequestedThemeVariant == ThemeVariant.Dark;
        }
    }
    private void Save()
    {
        try
        {
            ErrorMessage = "";
            LanguageViewModel.SetLanguage(SelectedLanguage);
            _config.Language = SelectedLanguage;
            _config.LogsFormat = SelectedLogsFormats;
            _config.ExtensionsToEncrypt = Extension.ToLower().Split(',');
            _config.Softwares = Softwares.Split(',');
            _config.LogsMods = SelectedLogsMods;
            _config.CriticalExtensions = CriticalExtensions.ToLower().Split(',');
            _config.API_URL = API_URL;
            _config.MaxParallelLargeFileSizeKo = MaxParallelLargeFileSizeKo ?? throw new UserException("error_bad_filesize");
            _config.SaveConfig();
            OnSaveRequested?.Invoke();

        }
        catch (UserException ex)
        {
            ErrorMessage = ex.Message;
            OnPropertyChanged(nameof(ErrorMessage));
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {ex.Message}");
        }
        
    }

    private void Cancel()
    {
        OnCancelRequested?.Invoke();
    }

    private string GetLogsModTranslation(LogsMods mod) => mod switch
    {
        LogsMods.Local => LanguageViewModel.GetTranslation("logs_mod_local"),
        LogsMods.Centralized => LanguageViewModel.GetTranslation("logs_mod_centralized"),
        LogsMods.Both => LanguageViewModel.GetTranslation("logs_mod_both"),
        _ => mod.ToString()
    };

    /// <summary>
    /// Event raised when the user wants to save the settings.
    /// </summary>
    public event Action? OnSaveRequested;

    /// <summary>
    /// Event raised when the user wants to close the settings view without saving.
    /// </summary>
    public event Action? OnCancelRequested;
}

public class LogsModsItem
{
    public LogsMods Value { get; }
    public string DisplayName { get; }

    public LogsModsItem(LogsMods value, string displayName)
    {
        Value = value;
        DisplayName = displayName;
    }

    public override string ToString() => DisplayName;
}