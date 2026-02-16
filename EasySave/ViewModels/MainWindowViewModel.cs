using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;
using System.Threading.Tasks;

namespace EasySave.ViewModels;

/// <summary>
/// Main view model for the application. Manages navigation, job CRUD operations, and UI state.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    // UI Display Properties
    public string Greeting { get; } = "Welcome to EasySave!";
    public string CustomCursorPath { get; set; } = "avares://EasySave/Assets/cursor.cur";
    public string CustomHoverCursorPath { get; set; } = "avares://EasySave/Assets/cursor-hover.cur";

    // Localization/Language support
    public LanguageViewModel _languageViewModel { get; }
    public string T_save_sobs => _languageViewModel.GetTranslation("save_jobs");
    public string T_create_job => _languageViewModel.GetTranslation("create_job");
    public string T_settings_tooltip => _languageViewModel.GetTranslation("settings_tooltip");

    // Navigation - holds the current view model being displayed
    private ViewModelBase _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    // Status message display - shows temporary success/error messages
    private string _statusMessage;
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

    public MainWindowViewModel()
    {
        // Initialize language support and load saved jobs
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "dictionary.json");
        _languageViewModel = LanguageViewModel.GetInstance(dictionaryPath);
        _languageViewModel.LanguageChanged += OnLanguageChanged;
        ShowSettingsCommand = new RelayCommand(ShowSettings);
        Jobs = new ObservableCollection<SavedJob>(_config.SavedJobs);

        LogsManager _logsManager = new LogsManager();
        StateManager _stateManager = new StateManager();
    }

    private void OnLanguageChanged()
    {
        OnPropertyChanged(nameof(T_save_sobs));
        OnPropertyChanged(nameof(T_create_job));
        OnPropertyChanged(nameof(T_settings_tooltip));
    }

    /// <summary>
    /// Opens the settings view
    /// </summary>
    private void ShowSettings()
    {
        var settingsVM = new SettingsViewModel();
        settingsVM.OnSaveRequested += () => CurrentViewModel = null;
        settingsVM.OnCancelRequested += () => CurrentViewModel = null;
        CurrentViewModel = settingsVM;
    }

    /// <summary>
    /// Opens the job creation dialog and adds new job to collection
    /// </summary>
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

    /// <summary>
    /// Opens the job editing dialog and updates existing job
    /// </summary>
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

    /// <summary>
    /// Executes a save job and displays a success message when complete
    /// </summary>
    public void RunJob(SavedJob job)
    {
        var runJobVM = new RunJobsViewModel(job);
        runJobVM.OnResult += async (confirmed) =>
        {
            CurrentViewModel = null;
            if (confirmed)
            {
                Console.WriteLine($"Sauvegarde lancée pour {job.Name}");
                await ShowSuccessMessage($"✔ {job.Name} : Sauvegarde terminée avec succès !");
            }
        };
        CurrentViewModel = runJobVM;
    }

    /// <summary>
    /// Opens a confirmation dialog and deletes the job if confirmed
    /// </summary>
    public void DeleteJob(SavedJob job)
    {
        // Display a confirmation dialog before deleting the job
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
}