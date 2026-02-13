using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

namespace EasySave.ViewModels;

public class MainWindowViewModel : ViewModelBase    // ViewModel for the main window of the application, it manages the current view model being displayed, the list of backup jobs, and commands for showing settings, creating, editing, running, and deleting jobs.
{
    public string Greeting { get; } = "Welcome to EasySave!";

    public string CustomCursorPath { get; set; } = "avares://EasySave/Assets/cursor.cur";

    public LanguageViewModel LanguageViewModel { get; }

    // Translated strings properties
    public string T_save_sobs => LanguageViewModel.GetTranslation("save_jobs");
    public string T_create_job => LanguageViewModel.GetTranslation("create_job");
    public string T_settings_tooltip => LanguageViewModel.GetTranslation("settings_tooltip");

    public string CustomHoverCursorPath { get; set; } = "avares://EasySave/Assets/cursor-hover.cur";
    
    private ViewModelBase _currentViewModel;

    public ViewModelBase? CurrentViewModel  // Property for the current view model being displayed in the main window, with getter and setter that raises property change notifications. This is used to switch between different views (e.g., settings, job settings, confirmation dialog) based on user actions.
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public ICommand ShowSettingsCommand { get; }
    public ObservableCollection<SavedJob> Jobs { get; set; }

    public MainWindowViewModel()    // Constructor initializes the ShowSettingsCommand and loads the list of jobs from the configuration (currently with test data)
    {
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "dictionary.json");
        LanguageViewModel = new LanguageViewModel(dictionaryPath);
        ShowSettingsCommand = new RelayCommand(ShowSettings);
        Jobs = new ObservableCollection<SavedJob>();
        LoadJobsFromConfig();
    }

    private void ShowSettings()
    {
        var settingsVM = new SettingsViewModel();

        // CLose the window when the user cancels
        settingsVM.OnCancelRequested += () => CurrentViewModel = null;

        // If the user saves, we should save the settings to the config file and then close the window
        settingsVM.OnSaveRequested += () =>
        {
            // TODO: Add the logic to Save the settings on the JSON file
            CurrentViewModel = null;    //FOR NOW, close the window, later, will only close it after saving the settings to the JSON file

        };

        CurrentViewModel = settingsVM;
    }

    private void LoadJobsFromConfig()   // Method to load the list of backup jobs from the configuration, currently it adds some test data to the Jobs collection, but in a complete implementation, it would read the jobs from the configuration file and populate the Jobs collection accordingly.
    {
        // Test Datas
        Jobs.Add(new SavedJob { Id = 1, Name = "Sauvegarde Documents", Source = "C:/Users/Documents", Destination = "D:/Backup/Documents" });
        Jobs.Add(new SavedJob { Id = 2, Name = "Sauvegarde Photos", Source = "C:/Users/Pictures", Destination = "D:/Backup/Pictures" });
    }

    public void CreateJob() // Method to create a new backup job, it creates a new instance of the JobSettingsViewModel, subscribes to its OnSaveRequested event to add the new job to the Jobs collection and set the CurrentViewModel to null when the job is saved, and also subscribes to its OnCancelRequested event to set the CurrentViewModel to null when the job creation is canceled. Finally, it sets the CurrentViewModel to the new JobSettingsViewModel instance to display it in the main window.
    {
        var jobVM = new JobSettingsViewModel();
        jobVM.OnSaveRequested += (newJob) =>
        {
            int newId = Jobs.Any() ? Jobs.Max(j => j.Id) + 1 : 1;
            newJob.Id = newId;
            Jobs.Add(newJob);
            CurrentViewModel = null;
        };
        jobVM.OnCancelRequested += () => CurrentViewModel = null;
        CurrentViewModel = jobVM;
    }

    public void EditJob(SavedJob job)   // Method to edit an existing backup job, it takes a SavedJob object as a parameter, creates a new instance of the JobSettingsViewModel initialized with the job to edit, subscribes to its OnSaveRequested event to update the job in the Jobs collection and set the CurrentViewModel to null when the job is saved, and also subscribes to its OnCancelRequested event to set the CurrentViewModel to null when the job editing is canceled. Finally, it sets the CurrentViewModel to the new JobSettingsViewModel instance to display it in the main window.
    {
        var jobVM = new JobSettingsViewModel(job);
        jobVM.OnSaveRequested += (updatedJob) =>
        {
            int index = Jobs.IndexOf(job);
            if (index != -1) Jobs[index] = updatedJob;
            CurrentViewModel = null;
        };
        jobVM.OnCancelRequested += () => CurrentViewModel = null;
        CurrentViewModel = jobVM;
    }

    public void RunJob(SavedJob job)
    {
        // On crée le ViewModel du dialogue
        var runJobVM = new RunJobsViewModel(job);

        // On s'abonne au résultat (Démarrer ou Annuler)
        runJobVM.OnResult += (confirmed) =>
        {
            CurrentViewModel = null; // On ferme l'overlay dans tous les cas

            if (confirmed)
            {
                // C'est ici que tu mettras ta logique de sauvegarde plus tard
                Console.WriteLine($"Action confirmée pour {job.Name} !");
            }
        };

        // On affiche l'overlay
        CurrentViewModel = runJobVM;
    }

    public void DeleteJob(SavedJob job)
    {
        // Display a confirmation dialog before deleting the job
        var confirmDialog = new ConfirmDeleteDialogViewModel();
        confirmDialog.JobName = job.Name;
        confirmDialog.OnResult += (confirmed) =>
        {
            CurrentViewModel = null; // close the dialog

            if (confirmed)
            {
                Jobs.Remove(job);

                // TODO: Appeler Config.DeleteJob(job.Name) pour le supprimer du json
            }
        };

        CurrentViewModel = confirmDialog;
    }
}