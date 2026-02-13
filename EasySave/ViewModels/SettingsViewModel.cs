using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

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
    
    public Languages SelectedLanguage { get; set; }
    public List<Languages> LanguagesList { get; set; }
    
    public LogsFormats SelectedLogsFormats { get; set; }
    public List<LogsFormats> LogsFormatsList { get; set; }
    
    public string Extension { get; set; }
    public string Softwares { get; set; }
    
    
    private Config _config = Config.S_GetInstance();
    

    public SettingsViewModel()
    {
        SelectedLanguage = _config.Language;
        SelectedLogsFormats = _config.LogsFormat;
        Extension = string.Join(",", _config.ExtensionsToEncrypt);
        Softwares = string.Join(",", _config.Softwares);
        
        
        LanguagesList = new List<Languages>(Languages.GetValuesAsUnderlyingType<Languages>().Cast<Languages>().ToArray());
        LogsFormatsList = new List<LogsFormats>(LogsFormats.GetValuesAsUnderlyingType<LogsFormats>().Cast<LogsFormats>().ToArray());
        CloseCommand = new RelayCommand(Close);
    }

    private void Close()
    {
        _config.Language = SelectedLanguage;
        _config.LogsFormat = SelectedLogsFormats;
        _config.ExtensionsToEncrypt = Extension.Split(',');
        _config.Softwares = Softwares.Split(',');
        _config.SaveConfig();
        
        // Signal to close settings (will be handled by MainWindowViewModel)
        OnCloseRequested?.Invoke();
    }

    /// <summary>
    /// Event raised when the user wants to close the settings view.
    /// </summary>
    public event Action? OnCloseRequested;
}
