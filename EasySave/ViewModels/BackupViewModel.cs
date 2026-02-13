using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Interfaces;
using EasySave.Models;
using System.Diagnostics;

namespace EasySave.ViewModels;

/// <summary>
/// ViewModel for the Backup view displaying the list of backup jobs.
/// Handles job execution, editing, and deletion.
/// </summary>
public class BackupViewModel : ViewModelBase
{
    public LanguageViewModel _languageViewModel { get; }

    public string T_invalid_backup_id => _languageViewModel.GetTranslation("invalid_backup_id");
    public string T_source_in_use => _languageViewModel.GetTranslation("source_in_use");
    public string T_start_less_or_equal_end => _languageViewModel.GetTranslation("start_less_or_equal_end");
    public string T_invalid_range_format => _languageViewModel.GetTranslation("invalid_range_format");
    public string T_error_occured => _languageViewModel.GetTranslation("error_occured");
    public string T_invalid_job_id_not_integer => _languageViewModel.GetTranslation("invalid_job_id_not_integer");

    private Config _config = Config.S_GetInstance();

    /// <summary>
    /// Observable collection of backup jobs to display in the UI.
    /// </summary>
    public ObservableCollection<SavedJob> Jobs { get; set; } = new();

    /// <summary>
    /// Command to run a specific backup job.
    /// </summary>
    public ICommand RunJobCommand { get; }

    /// <summary>
    /// Command to edit a specific backup job.
    /// </summary>
    public ICommand EditJobCommand { get; }

    /// <summary>
    /// Command to delete a specific backup job.
    /// </summary>
    public ICommand DeleteJobCommand { get; }

    public BackupViewModel()    // Constructor initializes commands and loads jobs from config
    {
        RunJobCommand = new RelayCommand<SavedJob>(RunJob);
        EditJobCommand = new RelayCommand<SavedJob>(EditJob);
        DeleteJobCommand = new RelayCommand<SavedJob>(DeleteJob);

        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "dictionary.json");
        _languageViewModel = new LanguageViewModel(dictionaryPath);

        LoadJobs();
    }

    private void LoadJobs() // Method to load jobs from config and populate the Jobs collection
    {
        Jobs.Clear();
        // TODO: Load jobs from config
        // Example: foreach (var job in _config.GetAllJobs()) Jobs.Add(job);
    }

    private void RunJob(SavedJob? job)  // Method to run a specific backup job, it takes a SavedJob object as a parameter and calls the _runBackup method with the job ID and a default backup type (e.g., Complete)
    {
        if (job == null) return;
        // Run the backup with default type (e.g., Complete)
        _runBackup(job.Id, BackupType.Complete);
    }

    private void EditJob(SavedJob? job) // Method to edit a specific backup job, it takes a SavedJob object as a parameter and opens the edit dialog or navigates to the edit view
    {
        if (job == null) return;
        // TODO: Open edit dialog or navigate to edit view
        System.Diagnostics.Debug.WriteLine($"Editing job: {job.Name}");
    }

    private void DeleteJob(SavedJob? job)   // Method to delete a specific backup job, it takes a SavedJob object as a parameter and removes it from the Jobs collection and deletes it from the config
    {
        if (job == null) return;
        Jobs.Remove(job);
        // TODO: Delete from config
        // _config.DeleteJob(job.Name);
        System.Diagnostics.Debug.WriteLine($"Deleted job: {job.Name}");
    }

    private void _runBackup(int jobId, BackupType backupType)   //private method to run single backup
    {
        SavedJob? savedJob = _config.GetJob(jobId);
        if (savedJob == null) throw new ArgumentException(T_invalid_backup_id);
        if (isASafeJob(savedJob)) throw new Exception(T_source_in_use);
        BackupInfo backupInfo = new BackupInfo() {SavedJobInfo = savedJob};
        backupInfo.TotalFiles = 0;   //initialize total files to 0, will be updated in the backup process

        if (backupType == BackupType.Differential)     //if backup type is differential, create a DiffBackup object and call its ExecuteBackup method
        {
            IBackup backup = new DiffBackup(savedJob, backupInfo);
            backup.ExecuteBackup();
        }
        else if (backupType == BackupType.Complete)
        {
            IBackup backup = new CompBackup(savedJob, backupInfo);
            backup.ExecuteBackup();
        }
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

    public void RunRangeBackup(string range, BackupType backupType)     //Public method, will call _getJobIdsToBackup to get back the job IDs to backup, then call _runBackup for each job ID
    {
        try 
        {
            int[] jobIdsToBackup = _getJobIdsToBackup(range);
            foreach (int jobId in jobIdsToBackup)
            {
                _runBackup(jobId, backupType);
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log the error, show a message to the user, etc.)
            Console.WriteLine($"{T_error_occured}{ex.Message}");
        }
    }

    private int[] _getJobIdsToBackup(string range) //private method to parse the range string and return an array of job IDs to backup (e.g., "1-3" returns [1,2,3], "1;3" returns [1,3])
    {
        if (range.Contains("-"))   //if the range contains a "-", we split it and create a range of job IDs from the first to the second number
        {
            string[] parts = range.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end))
            {
                if (start > end) throw new ArgumentException(T_start_less_or_equal_end);
                return Enumerable.Range(start, end - start + 1).ToArray();
            }
            else
            {
                throw new ArgumentException(T_invalid_range_format);
            }
        }
        else if (range.Contains(";"))    //if the range contains a ";", we split it and parse each part as a separate job ID
        {
            string[] parts = range.Split(';');
            List<int> jobIds = new List<int>();
            foreach (string part in parts)
            {
                if (int.TryParse(part, out int jobId))
                {
                    jobIds.Add(jobId);
                }
                else
                {
                    throw new ArgumentException($"'{T_invalid_job_id_not_integer}''{part}'");
                }
            }
            return jobIds.ToArray();
        }
        else    //if the range is just a single number, we parse it as a single job ID
        {
            if (int.TryParse(range, out int jobId))
            {
                return  [jobId];
            }
            else
            {
                throw new ArgumentException($"'{T_invalid_job_id_not_integer}''{range}'");
            }
        }
    }
}