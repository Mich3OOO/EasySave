using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

namespace EasySave.ViewModels;

public class RunJobsViewModel : ViewModelBase
{
    public string T_start_save => LanguageViewModel.GetTranslation("start_save");
    public string T_save_type => LanguageViewModel.GetTranslation("save_type");
    public string T_comp => LanguageViewModel.GetTranslation("comp");
    public string T_confirm_diff => LanguageViewModel.GetTranslation("diff");
    public string T_password => LanguageViewModel.GetTranslation("password");
    public string T_enter_password => LanguageViewModel.GetTranslation("enter_password");
    private bool _isDifferential = false;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public SavedJob Job { get; }

    // single language manager
    public LanguageViewModel _languageViewModel { get; }

    // =======================================================
    // TRANSLATIONS
    // =======================================================
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
    // =======================================================

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

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    // Information to send to MainWindows when user validate or quite window
    public event Action<bool, bool, string>? OnResult;

    public RunJobsViewModel(SavedJob job)   //constructor
    {
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "dictionary.json");
        _languageViewModel = LanguageViewModel.GetInstance(dictionaryPath);
        Job = job;
        ConfirmCommand = new RelayCommand(() =>
        {
            try
            {
                // check if password is valid
                if (!IsPasswordValid(_password))
                {
                    ErrorMessage = _languageViewModel.GetTranslation("password_policy");
                    return;
                }

                ErrorMessage = string.Empty;

                // Send data to MainWindowViewModel (Confirm: yes, Backup type, Password)
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

    public bool IsPasswordValid(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        // Min 12 char, with lower, upper, digits and special character
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$";

        return Regex.IsMatch(password, pattern);
    }
}