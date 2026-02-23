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
    
    
    /// <summary>
    /// Used to display validation errors when saving the job settings
    /// </summary>
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
    public string T_invalid_backup_id => _languageViewModel.GetTranslation("invalid_backup_id");
    public string T_source_in_use => _languageViewModel.GetTranslation("source_in_use");

    public bool IsDifferential
    {
        get => _isDifferential;
        set => SetProperty(ref _isDifferential, value);
    }

    /// <summary>
    /// Lnguage view model used to get translations for the UI
    /// </summary>
    public LanguageViewModel _languageViewModel { get; }

    /// <summary>
    /// Password property linked to the password field in the view
    /// </summary>
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string T_launch_save => _languageViewModel.GetTranslation("launch_save");
    public string T_what_type_save => _languageViewModel.GetTranslation("what_type_save");
    public string T_complete => _languageViewModel.GetTranslation("complete");
    public string T_differential => _languageViewModel.GetTranslation("differential");
    public string T_password => _languageViewModel.GetTranslation("password");
    public string T_enter_password => _languageViewModel.GetTranslation("enter_password");
    public string T_cancel => _languageViewModel.GetTranslation("cancel");
    public string T_launch => _languageViewModel.GetTranslation("launch");

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    /// <summary>
    /// true = Launch, false = Cancel
    /// </summary>
    public event Action<bool>? OnResult;

    /// <summary>
    /// Initializes a new instance for the given job.
    /// </summary>
    /// <param name="job">The saved job to run.</param>
    public RunJobsViewModel(SavedJob job)
    {
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "dictionary.json");
        _languageViewModel = LanguageViewModel.GetInstance(dictionaryPath);
        Job = job;
        ConfirmCommand = new RelayCommand(() =>
        {
            try
            {
                ErrorMessage = string.Empty;
                _runBackup();
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
    private void _runBackup()
    {
        if (Job == null) throw new ArgumentException(T_invalid_backup_id);
        if (!_canARunJon(out string openedProcess)) throw new Exception(T_source_in_use + " : " + openedProcess);
        if (!IsPasswordValid(_password)) throw new Exception(_languageViewModel.GetTranslation("password_policy"));
        BackupInfo backupInfo = new BackupInfo() {SavedJobInfo = Job};
        backupInfo.TotalFiles = 0;   //initialize total files to 0, will be updated in the backup process
        IBackup backup =  IsDifferential ? new DiffBackup(Job, backupInfo,_password): new CompBackup(Job, backupInfo,_password);

        Task.Run(backup.ExecuteBackup);
        
        JobManager.GetInstance().AddJob(Job,backup);
        OnResult?.Invoke(true);
    }

    /// <summary>
    /// Checks whether any configured software process is currently running
    /// </summary>
    /// <param name="processName">The name of the blocking process</param>
    /// <returns>true if no blocking process is running; otherwise false</returns>
    private bool _canARunJon(out string processName)
    {
        Config conf = Config.S_GetInstance();
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
    /// <param name="password">The password to check</param>
    /// <returns>true if the password is valid; otherwise false</returns>
    public bool IsPasswordValid(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        // Min 12 char, with lower, upper, digits and special character
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$";

        return Regex.IsMatch(password, pattern);
    }
}

