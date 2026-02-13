using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

namespace EasySave.ViewModels;

public class MainWindowViewModel : ViewModelBase
{

    public string CustomCursorPath { get; set; } = "avares://EasySave/Assets/cursor.cur";
    public string CustomHoverCursorPath { get; set; } = "avares://EasySave/Assets/cursor-hover.cur";
    
    private ViewModelBase _currentViewModel;

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public ICommand ShowSettingsCommand { get; }
    public ObservableCollection<SavedJob> Jobs { get; set; }
    
    private Config _config = Config.S_GetInstance();

    public MainWindowViewModel()
    {
        
        ShowSettingsCommand = new RelayCommand(ShowSettings);
        Jobs = new ObservableCollection<SavedJob>(_config.SavedJobs);

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

    public void RunJob(SavedJob job)
    {
        
        
    }

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
                _config.DeleteJob(job);
                _config.SaveConfig();
                Jobs.Remove(job);
            }
        };

        CurrentViewModel = confirmDialog;
    }
}