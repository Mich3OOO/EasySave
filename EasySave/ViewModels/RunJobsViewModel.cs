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
    public SavedJob Job { get; }
    
    private string _errorMessage = string.Empty;
    
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
    public string T_invalid_backup_id => LanguageViewModel.GetTranslation("invalid_backup_id");
    public string T_source_in_use => LanguageViewModel.GetTranslation("source_in_use");

    public bool IsDifferential
    {
        get => _isDifferential;
        set => SetProperty(ref _isDifferential, value);
    }

    public LanguageViewModel LanguageViewModel { get; }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

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

    /// <summary>
    /// true = Launch, false = Cancel
    /// </summary>
    public event Action<bool>? OnResult;

    /// <summary>
    /// Initializes a new instance for the given job.
    /// </summary>
    public RunJobsViewModel(SavedJob job)
    {
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "Dictionary.json");
        LanguageViewModel = LanguageViewModel.GetInstance(dictionaryPath);
        Job = job;
        ConfirmCommand = new RelayCommand(() =>
        {
            try
            {
                ErrorMessage = string.Empty;
                RunBackup();
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        });
        CancelCommand = new RelayCommand(() => OnResult?.Invoke(false));
    }
    
    /// <summary>
    /// Runs a single backup for the current job.
    /// </summary>
    private void RunBackup()
    {
        if (Job == null) throw new ArgumentException(T_invalid_backup_id);
        if (!CanARunJob(out string openedProcess)) throw new Exception(T_source_in_use + " : " + openedProcess);
        if (!IsPasswordValid(_password)) throw new Exception(LanguageViewModel.GetTranslation("password_policy"));
        BackupInfo backupInfo = new()
        {
            SavedJobInfo = Job,
            TotalFiles = 0
        };
        IBackup backup =  IsDifferential ? new DiffBackup(Job, backupInfo,_password): new CompBackup(Job, backupInfo,_password);

        Task.Run(backup.ExecuteBackup);
        
        JobManager.GetInstance().AddJob(Job,backup);
        OnResult?.Invoke(true);
    }

    /// <summary>
    /// Checks whether any configured software process is currently running
    /// </summary>
    private static bool CanARunJob(out string processName)
    {
        Config conf = Config.GetInstance();
        Process[] allProcesses = Process.GetProcesses();
        processName = "";
        foreach (Process process in allProcesses)
        {
            if(conf.Softwares.Contains(process.ProcessName ))
            {
                processName =  process.ProcessName;
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks whether the password meets the required policy
    /// Min 12 char, with lower, upper, digits and special character
    /// </summary>
    public static bool IsPasswordValid(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$";

        return Regex.IsMatch(password, pattern);
    }
}