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
    
    public string T_invalid_backup_id => LanguageViewModel.GetTranslation("invalid_backup_id");
    public string T_source_in_use => LanguageViewModel.GetTranslation("source_in_use");
    public LanguageViewModel LanguageViewModel { get; }

    public bool IsDifferential
    {
        get => _isDifferential;
        set => SetProperty(ref _isDifferential, value);
    }

    public LanguageViewModel LanguageViewModel { get; } // Property for the language view model, used to get translations for the UI

    public string Password   // Property for the password path, with getter and setter that raises property change notifications
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string T_launch_save => LanguageViewModel.GetTranslation("launch_save");
    public string T_what_type_save => LanguageViewModel.GetTranslation("what_type_save");
    public string T_complete => LanguageViewModel.GetTranslation("complete");
    public string T_differential => LanguageViewModel.GetTranslation("differential");
    public string T_password => LanguageViewModel.GetTranslation("password");
    public string T_cancel => LanguageViewModel.GetTranslation("cancel");
    public string T_launch => LanguageViewModel.GetTranslation("launch");





    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<bool>? OnResult; // true = Lancer, false = Annuler

    public RunJobsViewModel(SavedJob job)   //constructor
    {
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "dictionary.json");
        LanguageViewModel = new LanguageViewModel(dictionaryPath);
        Job = job;
        ConfirmCommand = new RelayCommand(_runBackup);
        CancelCommand = new RelayCommand(() => OnResult?.Invoke(false));
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "dictionary.json");
        LanguageViewModel = new LanguageViewModel(dictionaryPath);
    }
    
    private void _runBackup()   //private method to run single backup
    {
        if (Job == null) throw new ArgumentException(T_invalid_backup_id);
        //if (isASafeJob(Job)) throw new Exception(T_source_in_use);
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
    private bool isASafeJob(SavedJob savedJob)  // Method to check if the source of the backup job is currently being used by another program, it gets the list of all running processes and checks if any of them has a main module that contains the source path of the backup job, if it finds one, it returns false, otherwise it returns true
    {
        Process[] allProcesses = Process.GetProcesses();

        foreach (Process process in allProcesses)
        {
            if(process.MainModule?.FileName.Contains(savedJob.Source) == true) return false;
            
        }
        
        return true;
    }
}

