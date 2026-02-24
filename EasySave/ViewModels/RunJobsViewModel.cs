using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.ViewModels;

public class RunJobsViewModel : ViewModelBase
{
    private bool _isDifferential = false;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isMultipleSelection;
    private string _displayTitle = string.Empty;

    // Properties 
    public SavedJob Job { get; }

    // Property for the language view model, used to get translations for the UI
    public LanguageViewModel _languageViewModel { get; }

    // Property for error messages, used to display validation errors when saving the job settings.
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsDifferential
    {
        get => _isDifferential;
        set => SetProperty(ref _isDifferential, value);
    }

    // Property for the password path, with getter and setter that raises property change notifications
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    // Properties for multiple selection display logic
    public bool IsMultipleSelection
    {
        get => _isMultipleSelection;
    }

    public string DisplayTitle
    {
        get => _displayTitle;
    }

    // TRANSLATIONS
    public string T_start_save => _languageViewModel.GetTranslation("start_save");
    public string T_save_type => _languageViewModel.GetTranslation("save_type");
    public string T_comp => _languageViewModel.GetTranslation("comp");
    public string T_confirm_diff => _languageViewModel.GetTranslation("diff");
    public string T_launch_save => _languageViewModel.GetTranslation("launch_save");
    public string T_what_type_save => _languageViewModel.GetTranslation("what_type_save");
    public string T_complete => _languageViewModel.GetTranslation("complete");
    public string T_differential => _languageViewModel.GetTranslation("differential");
    public string T_password => _languageViewModel.GetTranslation("password");
    public string T_enter_password => _languageViewModel.GetTranslation("enter_password");
    public string T_cancel => _languageViewModel.GetTranslation("cancel");
    public string T_launch => _languageViewModel.GetTranslation("launch");
    public string T_invalid_backup_id => _languageViewModel.GetTranslation("invalid_backup_id");
    public string T_source_in_use => _languageViewModel.GetTranslation("source_in_use");

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    // Information to send to MainWindows when user validate or quite window
    public event Action<bool, bool, string>? OnResult;

    // Constructor updated to handle multiple selection flag and count
    public RunJobsViewModel(SavedJob job, bool isMultiple = false, string combinedNames = "")
    {
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "dictionary.json");
        _languageViewModel = LanguageViewModel.GetInstance(dictionaryPath);

        Job = job;
        _isMultipleSelection = isMultiple;

        // Set the title dynamically based on the execution mode
        if (_isMultipleSelection && !string.IsNullOrEmpty(combinedNames))
        {
            _displayTitle = combinedNames;
        }
        else
        {
            _displayTitle = Job.Name;
        }

        ConfirmCommand = new RelayCommand(() =>
        {
            try
            {
                if (Job == null) throw new ArgumentException(T_invalid_backup_id);

                // Group feature: check if forbidden software is running
                if (!_canARunJon(out string openedProcess))
                {
                    ErrorMessage = T_source_in_use + " : " + openedProcess;
                    return;
                }

                // Check if password is valid
                if (!IsPasswordValid(_password))
                {
                    ErrorMessage = _languageViewModel.GetTranslation("password_policy");
                    return;
                }

                ErrorMessage = string.Empty;

                // send data to MainWindowViewModel (Confirm: yes, Backup type, Password)
                OnResult?.Invoke(true, IsDifferential, Password);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        });

        // If Cancel, send false and empty strings
        CancelCommand = new RelayCommand(() => OnResult?.Invoke(false, false, string.Empty));
    }

    // Method to check if the source of the backup job is currently being used by another programm
    private bool _canARunJon(out string processName)
    {
        Config conf = Config.S_GetInstance();
        Process[] allProcesses = Process.GetProcesses();
        processName = "";
        foreach (Process process in allProcesses)
        {
            if (conf.Softwares.Contains(process.ProcessName))
            {
                processName = process.ProcessName;
                return false;
            }
        }
        return true;
    }

    // Check password policy
    public bool IsPasswordValid(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        // Min 12 char, with lower, upper, digits and special character
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$";

        return Regex.IsMatch(password, pattern);
    }
}
