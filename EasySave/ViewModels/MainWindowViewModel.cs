using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;
using Avalonia.Controls;
using Avalonia.Platform;
using EasySave.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EasySave.ViewModels;

/// <summary>
/// Main view model for the application. Manages navigation, job CRUD operations, and UI state.
/// </summary>
public class MainWindowViewModel : ObservableObject
{
    // Manage progress window
    private JobsStateViewModel? _sharedProgressViewModel;
    private Window? _progressWindow;

    // UI Display Properties
    public string Greeting { get; } = "Welcome to EasySave!";
    public string CustomCursorPath { get; set; } = "avares://EasySave/Assets/cursor.cur";
    public string CustomHoverCursorPath { get; set; } = "avares://EasySave/Assets/cursor-hover.cur";
    private readonly JobManager _jobManager =  JobManager.GetInstance();

    public LanguageViewModel LanguageViewModel { get; } = LanguageViewModel.GetInstance();

    public string T_save_jobs => LanguageViewModel.GetTranslation("save_jobs");
    public string T_create_job => LanguageViewModel.GetTranslation("create_job");
    public string T_settings_tooltip => LanguageViewModel.GetTranslation("settings_tooltip");
    public string T_run_selection => LanguageViewModel.GetTranslation("run_selection");

    // Navigation - holds the current view model being displayed
    private object? _currentViewModel;
    public object? CurrentViewModel
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
    public ICommand ShowProgressCommand { get; }
    public ObservableCollection<SavedJob> Jobs { get; set; }

    private readonly Config _config = Config.GetInstance();

    private readonly StateManager _stateManager = StateManager.GetInstance();

    public MainWindowViewModel()
    {
        LanguageViewModel.LanguageChanged += OnLanguageChanged;

        ShowSettingsCommand = new RelayCommand(ShowSettings);
        ShowProgressCommand = new RelayCommand(ShowProgress);

        // Initialize command for multiple selection
        RunSelectedJobsCommand = new RelayCommand(RunSelectedJobs);

        Jobs = new ObservableCollection<SavedJob>(_config.SavedJobs);

        LogsManager _logsManager = new();
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
        runJobVM.OnResult += async (confirmed, isDiff, password) =>
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
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    _progressWindow.Closed += (s, e) =>
                    {
                        _progressWindow = null;
                    };
                    _progressWindow.Show();
                }
                else
                {
                    _progressWindow.Activate();
                }

                foreach (SavedJob? job in selectedJobs)
                {
                    if (_stateManager.GetStateFrom(job.Name)?.State == StateLevel.Active) continue;

                    JobProgressViewModel progressBarObserver = _sharedProgressViewModel.AddNewJob(job.Name);

                    EventManager.GetInstance().Subscribe(progressBarObserver);

                    // Run backup in background thread so UI doesn't freeze. 
                    // No 'await' here ensures true parallelism for all selected jobs.
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            var backupInfo = new BackupInfo() { SavedJobInfo = job, TotalFiles = 0 };

                            IBackup backup;
                            if (isDiff)
                                backup = new DiffBackup(job, backupInfo, password);
                            else
                                backup = new CompBackup(job, backupInfo, password);

                            _jobManager.AddJob(job, backup);

                            backup.ExecuteBackup();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[BACKUP ERROR] {job.Name}: {ex.Message}");
                        }
                    });
                }

                foreach (SavedJob? j in selectedJobs) j.IsSelected = false;
                RefreshSelectionStatus();
            }
        };
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
            runJobVM.OnResult += async (confirmed, isDiff, password) =>
            {
                CurrentViewModel = null;

                if (confirmed)
                {
                    // Init shared progress view model if null
                    if (_sharedProgressViewModel == null)
                    {
                        _sharedProgressViewModel = new JobsStateViewModel();
                        _sharedProgressViewModel.OnCloseRequested += () => _progressWindow?.Close();
                    }

                    JobProgressViewModel progressBarObserver = _sharedProgressViewModel.AddNewJob(job.Name);

                    EventManager.GetInstance().Subscribe(progressBarObserver);

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
                            WindowStartupLocation = WindowStartupLocation.CenterScreen
                        };

                        _progressWindow.Closed += (s, e) =>
                        {
                            _progressWindow = null;
                        };
                        _progressWindow.Show();
                    }
                    else
                    {
                        _progressWindow.Activate();
                    }

                    await Task.Run(() =>
                    {
                        try
                        {
                            var backupInfo = new BackupInfo() { SavedJobInfo = job, TotalFiles = 0 };

                            IBackup backup;
                            if (isDiff)
                                backup = new DiffBackup(job, backupInfo, password);
                            else
                                backup = new CompBackup(job, backupInfo, password);

                            _jobManager.AddJob(job, backup);

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

    private void ShowProgress()
    {
        if (_sharedProgressViewModel == null)
        {
            _sharedProgressViewModel = new JobsStateViewModel();
            _sharedProgressViewModel.OnCloseRequested += () => _progressWindow?.Close();
        }
        if (_progressWindow == null)
        {
            _progressWindow = new Window
            {
                Title = "EasySave - Progression",
                Width = 700,
                Height = 450,
                Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://EasySave/Assets/Icon.png"))),
                Content = new EasySave.Views.JobsStateView { DataContext = _sharedProgressViewModel },
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            _progressWindow.Closed += (s, e) =>
            {
                _progressWindow = null;
            };
            _progressWindow.Show();
        }
        else
        {
            _progressWindow.Activate();
        }
    }
}
