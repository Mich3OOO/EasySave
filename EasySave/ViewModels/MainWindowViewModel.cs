using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform;
using EasySave.Interfaces;

namespace EasySave.ViewModels;

/// <summary>
/// Main view model for the application. Manages navigation, job CRUD operations, and UI state.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    // Manage progress window
    private JobsStateViewModel? _sharedProgressViewModel;
    private Window? _progressWindow;

    // UI Display Properties
    public string Greeting { get; } = "Welcome to EasySave!";
    public string CustomCursorPath { get; set; } = "avares://EasySave/Assets/cursor.cur";
    public string CustomHoverCursorPath { get; set; } = "avares://EasySave/Assets/cursor-hover.cur";
    private readonly JobManager _jobManager =  JobManager.GetInstance();

    // Localization/Language support
    public LanguageViewModel LanguageViewModel { get; }
    public string T_save_jobs => LanguageViewModel.GetTranslation("save_jobs");
    public string T_create_job => LanguageViewModel.GetTranslation("create_job");
    public string T_settings_tooltip => LanguageViewModel.GetTranslation("settings_tooltip");
    public string T_run_selection => LanguageViewModel.GetTranslation("run_selection");

    // Navigation - holds the current view model being displayed
    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    // Status message display - shows temporary success/error messages
    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    private bool _isStatusVisible;
    public bool IsStatusVisible
    {
        get => _isStatusVisible;
        set => SetProperty(ref _isStatusVisible, value);
    }

    // MULTIPLE SELECTION PROPERTIES

    // Checks if at least one job is checked
    public bool HasSelectedJobs => Jobs.Any(j => j.IsSelected);

    // Command for the "Run Selected Jobs" button
    public ICommand RunSelectedJobsCommand { get; }

    // Public method to force UI refresh from Code-Behind
    public void RefreshSelectionStatus()
    {
        OnPropertyChanged(nameof(HasSelectedJobs));
    }

    /// <summary>
    /// Displays a success message temporarily (4 seconds) in the UI
    /// </summary>
    private async Task ShowSuccessMessage(string message)
    {
        StatusMessage = message;
        IsStatusVisible = true;
        await Task.Delay(4000);
        IsStatusVisible = false;
    }

    // Commands and data collections
    public ICommand ShowSettingsCommand { get; }
    public ObservableCollection<SavedJob> Jobs { get; set; }

    private readonly Config _config = Config.GetInstance();

    private readonly StateManager _stateManager = StateManager.GetInstance();


    // Constructor
    public MainWindowViewModel()
    {
        var dictionaryPath =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "Dictionary.json");
        LanguageViewModel = LanguageViewModel.GetInstance(dictionaryPath);
        LanguageViewModel.LanguageChanged += OnLanguageChanged;

        ShowSettingsCommand = new RelayCommand(ShowSettings);

        // Initialize command for multiple selection
        RunSelectedJobsCommand = new RelayCommand(RunSelectedJobs);

        Jobs = new ObservableCollection<SavedJob>(_config.SavedJobs);

        LogsManager.GetInstance(); // Initialize LogsManager singleton to start listening to events

    }

    private void OnLanguageChanged()
    {
        OnPropertyChanged(nameof(T_save_jobs));
        OnPropertyChanged(nameof(T_create_job));
        OnPropertyChanged(nameof(T_settings_tooltip));
        OnPropertyChanged(nameof(T_run_selection));
    }

    private void ShowSettings()
    {
        var settingsVM = new SettingsViewModel();
        settingsVM.OnSaveRequested += () => CurrentViewModel = null;
        settingsVM.OnCancelRequested += () => CurrentViewModel = null;
        CurrentViewModel = settingsVM;
    }

    public void CreateJob()
    {
        JobSettingsViewModel jobVM = new();
        jobVM.OnSaveRequested += (newJob) =>
        {
            var newId = Jobs.Any() ? Jobs.Max(j => j.Id) + 1 : 1;
            newJob.Id = newId;
            _config.AddJob(newJob);
            _config.SaveConfig();
            Jobs.Add(newJob);
            CurrentViewModel = null;
        };
        jobVM.OnCancelRequested += () => CurrentViewModel = null;
        CurrentViewModel = jobVM;
    }

    public void EditJob(SavedJob job)
    {
        JobSettingsViewModel jobVM = new(job);
        jobVM.OnSaveRequested += (updatedJob) =>
        {
            var index = Jobs.IndexOf(job);
            if (index != -1) Jobs[index] = updatedJob;
            _config.UpdateJob(job.Name, updatedJob);
            _config.SaveConfig();
            CurrentViewModel = null;
        };
        jobVM.OnCancelRequested += () => CurrentViewModel = null;
        CurrentViewModel = jobVM;
    }

    /// <summary>
    /// Method to launch all selected jobs simultaneously via a single configuration popup.
    /// </summary>
    private void RunSelectedJobs()
    {
        var selectedJobs = Jobs.Where(j => j.IsSelected).ToList();
        if (selectedJobs.Count == 0) return;

        // We use combined names for display in the popup, but the actual job objects are passed for execution
        var combinedNames = string.Join(" - ", selectedJobs.Select(j => j.Name));

        // pass the combined names to the popup instead of the count heree
        var runJobVM = new RunJobsViewModel(selectedJobs.First(), isMultiple: true, combinedNames: combinedNames);

        // Wait for popup result
        runJobVM.OnResult +=  (confirmed, isDiff, password) =>  //do not make this async to avoid blocking the UI and ensure all jobs launch in parallel without waiting for each other
        {
            CurrentViewModel = null; // Close config dialog

            if (confirmed)
            {
                // Init shared progress view model if null
                if (_sharedProgressViewModel == null)
                {
                    _sharedProgressViewModel = new JobsStateViewModel();
                    _sharedProgressViewModel.OnCloseRequested += () => _progressWindow?.Close();
                }

                //Prepare progress bars and subscribe to events
                foreach (SavedJob? job in selectedJobs)
                {
                    // Avoid launching a job that is already running
                    if (_stateManager.GetStateFrom(job.Name)?.State == StateLevel.Active) continue;

                    // Check if a progress bar for this job already exist
                    bool exists = _sharedProgressViewModel.ActiveJobs.Any(pb => pb.JobName == job.Name);

                    if (!exists)
                    { 
                      // Create progress bar & Subscribe
                        JobProgressViewModel progressBarObserver = _sharedProgressViewModel.AddNewJob(job.Name);
                        EventManager.GetInstance().Subscribe(progressBarObserver);
                    }
                }

                // Setup separate progress window
                if (_progressWindow == null)
                {
                    _progressWindow = new Window
                    {
                        Title = "EasySave - Progression",
                        Width = 700,
                        Height = 450,
                        Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://EasySave/Assets/Icon.png"))),
                        Content = new EasySave.Views.JobsStateView { DataContext = _sharedProgressViewModel },
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Topmost = true  
                    };

                    _progressWindow.Closed += (s, e) => //Whhen user close the window
                    {
                        _progressWindow = null;

                        if (_sharedProgressViewModel != null)   // Clear progress bars and unsubscribe
                        {
                            foreach (var jobBar in _sharedProgressViewModel.ActiveJobs)
                            {
                                EventManager.GetInstance().Unsubscribe(jobBar);
                            }
                            _sharedProgressViewModel.ActiveJobs.Clear();
                        }
                    };
                    _progressWindow.Show();
                    _progressWindow.Topmost = false;    // Allow other windows to be on top of the progress window
                }
                else
                {
                    _progressWindow.Activate(); // Bring window to front
                }

                // Optional success message
                ShowSuccessMessage($"Launching {combinedNames} backups...");

                // Loop through all true jobs and launch them in parallel
                foreach (SavedJob? job in selectedJobs)
                {
                    // Avoid launching a job that is already running
                    if (_stateManager.GetStateFrom(job.Name)?.State == StateLevel.Active) continue;


                    // Run backup in background thread so UI doesn't freeze. 
                    // No 'await' here ensures true parallelism for all selected jobs.
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            // Init backup info
                            var backupInfo = new BackupInfo() { SavedJobInfo = job, TotalFiles = 0 };

                            // Create the correct backup type
                            IBackup backup;
                            if (isDiff)
                                backup = new DiffBackup(job, backupInfo, password);
                            else
                                backup = new CompBackup(job, backupInfo, password);

                            // Run backup
                            backup.ExecuteBackup();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[BACKUP ERROR] {job.Name}: {ex.Message}");
                        }
                    });
                }

                // Clean up the UI by unchecking all boxes
                foreach (SavedJob? j in selectedJobs) j.IsSelected = false;
                RefreshSelectionStatus();
            }
        };

        // Show the popup
        CurrentViewModel = runJobVM;
    }

    /// <summary>
    /// Executes a single save job and opens the progress window
    /// </summary>
    public void RunJob(SavedJob job)
    {
        if (_stateManager.GetStateFrom(job.Name)?.State != StateLevel.Active)
        {
            RunJobsViewModel runJobVM = new(job);
            runJobVM.OnResult += (confirmed, isDiff, password) =>
            {
                CurrentViewModel = null; // Close config dialog

                if (confirmed)
                {
                    // Init shared progress view model if null
                    if (_sharedProgressViewModel == null)
                    {
                        _sharedProgressViewModel = new JobsStateViewModel();
                        _sharedProgressViewModel.OnCloseRequested += () => _progressWindow?.Close();
                    }
                    //Check if a progress bar for this job already existss
                    bool exists = _sharedProgressViewModel.ActiveJobs.Any(pb => pb.JobName == job.Name);

                    if (!exists)
                    {
                        JobProgressViewModel progressBarObserver = _sharedProgressViewModel.AddNewJob(job.Name);
                        EventManager.GetInstance().Subscribe(progressBarObserver);
                    }

                    // Setup separate progress window
                    if (_progressWindow == null)
                    {
                        _progressWindow = new Window
                        {
                            Title = "EasySave - Progression",
                            Width = 700,
                            Height = 450,
                            Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://EasySave/Assets/Icon.png"))),
                            Content = new EasySave.Views.JobsStateView { DataContext = _sharedProgressViewModel },
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            Topmost = true
                        };

                        _progressWindow.Closed += (s, e) => //Whhen user close the window
                        {
                            _progressWindow = null;

                            if (_sharedProgressViewModel != null)   // Clear all progress bars and unsubscribe from events to avoid memory leaks
                            {
                                foreach (var jobBar in _sharedProgressViewModel.ActiveJobs)
                                {
                                    EventManager.GetInstance().Unsubscribe(jobBar);
                                }
                                _sharedProgressViewModel.ActiveJobs.Clear();
                            }
                        };
                        _progressWindow.Show();
                        _progressWindow.Topmost = false; // Restore normal behavior

                    }
                    else
                    {
                        _progressWindow.Activate(); // Bring window to front
                    }

                    // Optional success message
                    ShowSuccessMessage($"{job.Name} : Backup starting...");

                    // Run backup in background thread so UI doesn't freeze
                    Task.Run(() =>
                    {
                        try
                        {
                            // Init backup info
                            var backupInfo = new BackupInfo() { SavedJobInfo = job, TotalFiles = 0 };

                            // Create the correct backup type
                            IBackup backup;
                            if (isDiff)
                                backup = new DiffBackup(job, backupInfo, password);
                            else
                                backup = new CompBackup(job, backupInfo, password);

                            // Run backup
                            backup.ExecuteBackup();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[BACKUP ERROR] {ex.Message}");
                        }
                    });
                }
            };
            CurrentViewModel = runJobVM;
        }
    }

    public void DeleteJob(SavedJob job)
    {
        var confirmDialog = new DeleteViewModel
        {
            JobName = job.Name
        };
        confirmDialog.OnResult += (confirmed) =>
        {
            CurrentViewModel = null;
            if (confirmed)
            {
                _config.DeleteJob(job);
                _config.SaveConfig();
                Jobs.Remove(job);
            }
        };
        CurrentViewModel = confirmDialog;
    }

    public void PauseJob(SavedJob job)
    {
        _jobManager.PauseJob(job);
    }

    public void ContinueJob(SavedJob job)
    {
        _jobManager.ContinueJob(job);
    }
    public void CancelJob(SavedJob job)
    {
        _jobManager.CancelJob(job);
    }
}
