using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

namespace EasySave.ViewModels;

public class JobSettingsViewModel : ViewModelBase      // ViewModel for the job settings view, used for creating or editing backup jobs. It contains properties for the job name, source path, destination path, and error messages. It also has commands for saving or canceling the job settings and events to notify when a save or cancel action is requested.
{
    private string _name = string.Empty;
    private string _source = string.Empty;
    private string _destination = string.Empty;
    private string _errorMessage = string.Empty;

    public string Name  // Property for the job name, with getter and setter that raises property change notifications
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Source    // Property for the source path, with getter and setter that raises property change notifications
    {
        get => _source;
        set => SetProperty(ref _source, value);
    }

    public string Destination   // Property for the destination path, with getter and setter that raises property change notifications
    {
        get => _destination;
        set => SetProperty(ref _destination, value);
    }

    public string ErrorMessage  // Property for error messages, with getter and setter that raises property change notifications. This is used to display validation errors when saving the job settings.
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsEditMode { get; }
    private readonly SavedJob? _originalJob;

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<SavedJob>? OnSaveRequested; // Event to notify when a save action is requested, it takes a SavedJob object as a parameter containing the job settings to save
    public event Action? OnCancelRequested;         // Event to notify when a cancel action is requested, it does not take any parameters

    public JobSettingsViewModel()   // Constructor for creating a new job, it initializes the commands and sets IsEditMode to false
    {
        IsEditMode = false;
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public JobSettingsViewModel(SavedJob jobToEdit) : this()    // Constructor for editing an existing job, it initializes the commands, sets IsEditMode to true, and populates the properties with the values from the job to edit
    {
        IsEditMode = true;
        _originalJob = jobToEdit;
        Name = jobToEdit.Name;
        Source = jobToEdit.Source;
        Destination = jobToEdit.Destination;
    }

    private void Save() // Method to handle the save action, it validates the job settings and invokes the OnSaveRequested event with a SavedJob object if the settings are valid. If there are validation errors, it sets the ErrorMessage property to display the error message in the UI.
    {
        try
        {
            // Reset de l'erreur avant la tentative
            ErrorMessage = string.Empty;

            var job = _originalJob ?? new SavedJob();
            job.Name = Name;

            // Appel des méthodes de validation de votre modèle SavedJob
            // Si l'un des chemins est invalide, l'exception est levée ici
            job.SetSource(Source);
            job.SetDestination(Destination);

            OnSaveRequested?.Invoke(job);
        }
        catch (Exception ex)
        {
            // Affiche le message d'erreur (ex: "Invalid source path" ou "Invalid destination path")
            ErrorMessage = $"⚠️ {ex.Message}";
        }
    }

    private void Cancel() => OnCancelRequested?.Invoke();
}