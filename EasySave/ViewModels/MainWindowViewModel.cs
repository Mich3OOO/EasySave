using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models; 

namespace EasySave.ViewModels;

/// <summary>
/// Main window ViewModel that manages navigation between different views.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;

    /// <summary>
    /// The currently displayed ViewModel (BackupViewModel or SettingsViewModel).
    /// </summary>
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    /// <summary>
    /// Command to navigate to the settings view.
    /// </summary>
    public ICommand ShowSettingsCommand { get; }

    public ObservableCollection<SavedJob> Jobs { get; set; }

    public MainWindowViewModel()
    {
        ShowSettingsCommand = new RelayCommand(ShowSettings);

        Jobs = new ObservableCollection<SavedJob>();
        LoadJobsFromConfig();
    }

    private void ShowSettings()
    {
        var settingsViewModel = new SettingsViewModel();
        settingsViewModel.OnCloseRequested += () => CurrentViewModel = null;
        CurrentViewModel = settingsViewModel;
    }

    private void LoadJobsFromConfig()
    {
        // Fake data for testing
        Jobs.Add(new SavedJob { Id = 1, Name = "Sauvegarde Documents", Source = "C:/Users/Documents", Destination = "D:/Backup/Documents" });
        Jobs.Add(new SavedJob { Id = 2, Name = "Sauvegarde Photos", Source = "C:/Users/Pictures", Destination = "D:/Backup/Pictures" });
        Jobs.Add(new SavedJob { Id = 3, Name = "Sauvegarde Projet", Source = "C:/Dev/MyProject", Destination = "D:/Backup/MyProject" });
    }

    // --- COMMANDES MISES À JOUR ---

    // Note le paramètre "SavedJob job" !
    public void CreateJob()
    {
        //Debug.WriteLine("Création d'un nouveau job...");
        // Ici on ouvrira une fenêtre vide
    }

    public void RunJob(SavedJob job)
    {
        //Debug.WriteLine($"Lancement du job : {job.Name} (ID: {job.Id})");
        // Ici tu appelleras ta logique de backup V1
    }

    public void EditJob(SavedJob job)
    {
        //Debug.WriteLine($"Modification du job : {job.Name}");
    }

    public void DeleteJob(SavedJob job)
    {
        //Debug.WriteLine($"Suppression du job : {job.Name}");

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