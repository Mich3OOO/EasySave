using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

namespace EasySave.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to EasySave!";

    public string CustomCursorPath { get; set; } = "avares://EasySave/Assets/cursor.cur";

    public LanguageViewModel LanguageViewModel { get; }

    // Translated strings properties
    public string T_save_sobs => LanguageViewModel.GetTranslation("save_jobs");
    public string T_create_job => LanguageViewModel.GetTranslation("create_job");
    public string T_settings_tooltip => LanguageViewModel.GetTranslation("settings_tooltip");

    private ViewModelBase _currentViewModel;

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public ICommand ShowSettingsCommand { get; }
    public ObservableCollection<SavedJob> Jobs { get; set; }

    public MainWindowViewModel()
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
        settingsVM.OnCloseRequested += () => CurrentViewModel = null;
        CurrentViewModel = settingsVM;
    }

    private void LoadJobsFromConfig()
    {
        // Données de test
        Jobs.Add(new SavedJob { Id = 1, Name = "Sauvegarde Documents", Source = "C:/Users/Documents", Destination = "D:/Backup/Documents" });
        Jobs.Add(new SavedJob { Id = 2, Name = "Sauvegarde Photos", Source = "C:/Users/Pictures", Destination = "D:/Backup/Pictures" });
    }

    public void CreateJob()
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

    public void EditJob(SavedJob job)
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

    public void RunJob(SavedJob job) { /* Logique V1 */ }

    public void DeleteJob(SavedJob job)
    {
        // Afficher la popup de confirmation
        var confirmDialog = new ConfirmDeleteDialogViewModel();
        confirmDialog.JobName = job.Name;
        confirmDialog.OnResult += (confirmed) =>
        {
            CurrentViewModel = null; // Fermer le dialogue

            if (confirmed)
            {
                // On le retire de la liste visuelle
                Jobs.Remove(job);

                // TODO: Appeler Config.DeleteJob(job.Name) pour le supprimer du json
            }
        };

        CurrentViewModel = confirmDialog;
    }
}