using System;
using System.Collections.ObjectModel;
using System.Linq;
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
public class MainWindowViewModel : ViewModelBase, IEventListener
{
    // manage progress window
    private JobsStateViewModel? _sharedProgressViewModel;
    private Window? _progressWindow;

    // UI Display Properties
    public string Greeting { get; } = "Welcome to EasySave!";
    public string CustomCursorPath { get; set; } = "avares://EasySave/Assets/cursor.cur";
    public string CustomHoverCursorPath { get; set; } = "avares://EasySave/Assets/cursor-hover.cur";
    private JobManager _jobManager = JobManager.GetInstance();

    // Localization/Language support
    public LanguageViewModel _languageViewModel { get; }
    public string T_save_jobs => _languageViewModel.GetTranslation("save_jobs");
    public string T_create_job => _languageViewModel.GetTranslation("create_job");
    public string T_settings_tooltip => _languageViewModel.GetTranslation("settings_tooltip");

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

    //Logic for mulltiple selection jobs (with the checkbox)
    public bool HasSelectedJobs => Jobs.Any(j => j.IsSelected); // Check if any jobs are selected
    public ICommand RunSelectedJobsCommand { get; }
    public void RefreshSelectionStatus()    //refresh UI
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

    private Config _config = Config.S_GetInstance();
    private StateManager _stateManager = StateManager.GetInstance();

    public MainWindowViewModel()
    {
        // Initialize language support and load saved jobs
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "dictionary.json");
        _languageViewModel = LanguageViewModel.GetInstance(dictionaryPath);
        _languageViewModel.LanguageChanged += OnLanguageChanged;

        ShowSettingsCommand = new RelayCommand(ShowSettings);

        // Command for multiple job executionn
        RunSelectedJobsCommand = new RelayCommand(RunSelectedJobs);

        Jobs = new ObservableCollection<SavedJob>(_config.SavedJobs);

        // init managers (Logs and State)
        LogsManager _logsManager = new LogsManager();
    }

    private void OnLanguageChanged()
    {
        OnPropertyChanged(nameof(T_save_jobs));
        OnPropertyChanged(nameof(T_create_job));
        OnPropertyChanged(nameof(T_settings_tooltip));
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
        JobSettingsViewModel jobVM = new JobSettingsViewModel();
        jobVM.OnSaveRequested += (newJob) =>
        {
            int newId = Jobs.Any() ? Jobs.Max(j => j.Id) + 1 : 1;
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
        JobSettingsViewModel jobVM = new JobSettingsViewModel(job);
        jobVM.OnSaveRequested += (updatedJob) =>
        {
            int index = Jobs.IndexOf(job);
            if (index != -1) Jobs[index] = updatedJob;
            _config.UpdateJob(job.Name, updatedJob);
            _config.SaveConfig();
            CurrentViewModel = null;
        };
        jobVM.OnCancelRequested += () => CurrentViewModel = null;
        CurrentViewModel = jobVM;
    }

    // Method To launch all thz slected jobs at once, linked to "Run Selected Jobs" button in the UI
    private void RunSelectedJobs()
    {
        var selectedJobs = Jobs.Where(j => j.IsSelected).ToList();  // get all selected jobs
        foreach (var job in selectedJobs)   //foreach job selected, we call RunJob
        {
            RunJob(job);
        }
    }

    /// <summary>
    /// Executes a save job and opens the progress window
    /// </summary>
    public void RunJob(SavedJob job)
    {
        if (_stateManager.GetStateFrom(job.Name)?.State != StateLevel.Active)
        {
            var runJobVM = new RunJobsViewModel(job);

            // wait for popup result
            runJobVM.OnResult += async (confirmed, isDiff, password) =>
            {
                CurrentViewModel = null; // close config dialog

                if (confirmed)
                {
                    // init shared progress view model if null
                    if (_sharedProgressViewModel == null)
                    {
                        _sharedProgressViewModel = new JobsStateViewModel();
                        _sharedProgressViewModel.OnCloseRequested += () => _progressWindow?.Close();
                    }

                    // create progress bar
                    var progressBarObserver = _sharedProgressViewModel.AddNewJob(job.Name);

                    // subscribe to event manager
                    EventManager.GetInstance().Subscribe(progressBarObserver);

                    // setup separate progress window
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
                            _sharedProgressViewModel?.ActiveJobs.Clear();
                        };
                        _progressWindow.Show();
                    }
                    else
                    {
                        _progressWindow.Activate(); // bring window to front
                    }

                    // optional success message
                    await ShowSuccessMessage($"🚀 {job.Name} : Backup starting...");

                    // run backup in background thread so UI dont freeze
                    await Task.Run(() =>
                    {
                        try
                        {
                            // init backup info
                            BackupInfo backupInfo = new BackupInfo() { SavedJobInfo = job, TotalFiles = 0 };

                            // create the correct backup type
                            IBackup backup;
                            if (isDiff)
                                backup = new DiffBackup(job, backupInfo, password);
                            else
                                backup = new CompBackup(job, backupInfo, password);

                            // run backup
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
        var confirmDialog = new DeleteViewModel();
        confirmDialog.JobName = job.Name;
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

    public void Update(BackupInfo data)
    {

    }
}
