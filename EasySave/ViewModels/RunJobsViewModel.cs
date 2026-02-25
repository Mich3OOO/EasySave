using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.ViewModels;

public class RunJobsViewModel : ObservableObject
{
    private bool _isDifferential = false;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isMultipleSelection;
    private string _displayTitle = string.Empty;

    public SavedJob Job { get; }

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

    public bool IsMultipleSelection
    {
        get => _isMultipleSelection;
    }

    public string DisplayTitle
    {
        get => _displayTitle;
    }

    public LanguageViewModel LanguageViewModel { get; } = LanguageViewModel.GetInstance();
    public string T_start_save => LanguageViewModel.GetTranslation("start_save");
    public string T_save_type => LanguageViewModel.GetTranslation("save_type");
    public string T_comp => LanguageViewModel.GetTranslation("comp");
    public string T_confirm_diff => LanguageViewModel.GetTranslation("diff");
    public string T_invalid_backup_id => LanguageViewModel.GetTranslation("invalid_backup_id");
    public string T_source_in_use => LanguageViewModel.GetTranslation("source_in_use");
    public string T_launch_save => LanguageViewModel.GetTranslation("launch_save");
    public string T_what_type_save => LanguageViewModel.GetTranslation("what_type_save");
    public string T_complete => LanguageViewModel.GetTranslation("complete");
    public string T_differential => LanguageViewModel.GetTranslation("differential");
    public string T_password => LanguageViewModel.GetTranslation("password");
    public string T_enter_password => LanguageViewModel.GetTranslation("enter_password");
    public string T_cancel => LanguageViewModel.GetTranslation("cancel");
    public string T_launch => LanguageViewModel.GetTranslation("launch");

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<bool, bool, string>? OnResult;

    // Constructor updated to handle multiple selection flag and count
    public RunJobsViewModel(SavedJob job, bool isMultiple = false, string combinedNames = "")
    {
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

                // Check if password is valid
                if (!IsPasswordValid(_password))
                {
                    ErrorMessage = LanguageViewModel.GetTranslation("password_policy");
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

        CancelCommand = new RelayCommand(() => OnResult?.Invoke(false, false, string.Empty));
    }

    // Check password policy
    public static bool IsPasswordValid(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$";

        return Regex.IsMatch(password, pattern);
    }
}
