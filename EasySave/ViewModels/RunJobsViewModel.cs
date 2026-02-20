using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;
using EasySave.Interfaces;
using System.Diagnostics;

namespace EasySave.ViewModels;

public class RunJobsViewModel : ViewModelBase
{
    private bool _isDifferential = false;
    private string _password = string.Empty;
    public SavedJob Job { get; }
    
    private string _errorMessage = string.Empty;
    
    
    public string ErrorMessage  // Property for error messages, with getter and setter that raises property change notifications. This is used to display validation errors when saving the job settings.
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

    public LanguageViewModel _languageViewModel { get; } // Property for the language view model, used to get translations for the UI

    public string Password   // Property for the password path, with getter and setter that raises property change notifications
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

    public event Action<bool>? OnResult; // true = Lancer, false = Annuler

    public RunJobsViewModel(SavedJob job)   //constructor
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
    
    private void _runBackup()   //private method to run single backup
    {
        if (Job == null) throw new ArgumentException(T_invalid_backup_id);
        if (!_canARunJon(out string openedProcess)) throw new Exception(T_source_in_use + " : " + openedProcess);
        BackupInfo backupInfo = new BackupInfo() {SavedJobInfo = Job};
        backupInfo.TotalFiles = 0;   //initialize total files to 0, will be updated in the backup process

        if (IsDifferential)     //if backup type is differential, create a DiffBackup object and call its ExecuteBackup method
        {
            IBackup backup = new DiffBackup(Job, backupInfo,_password);
            backup.ExecuteBackup();
        }
        else if (!IsDifferential)
        {
            IBackup backup = new CompBackup(Job, backupInfo,_password);
            backup.ExecuteBackup();
        }

        OnResult?.Invoke(true);
    }
    private bool _canARunJon(out string processName)  // Method to check if the source of the backup job is currently being used by another program, it gets the list of all running processes and checks if any of them has a main module that contains the source path of the backup job, if it finds one, it returns false, otherwise it returns true
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
}

