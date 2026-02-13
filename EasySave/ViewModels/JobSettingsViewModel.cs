using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

namespace EasySave.ViewModels;

public class JobSettingsViewModel : ViewModelBase
{
    private string _name = string.Empty;
    private string _source = string.Empty;
    private string _destination = string.Empty;
    private string _errorMessage = string.Empty;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Source
    {
        get => _source;
        set => SetProperty(ref _source, value);
    }

    public string Destination
    {
        get => _destination;
        set => SetProperty(ref _destination, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsEditMode { get; }
    private readonly SavedJob? _originalJob;

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<SavedJob>? OnSaveRequested;
    public event Action? OnCancelRequested;

    public JobSettingsViewModel()
    {
        IsEditMode = false;
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public JobSettingsViewModel(SavedJob jobToEdit) : this()
    {
        IsEditMode = true;
        _originalJob = jobToEdit;
        Name = jobToEdit.Name;
        Source = jobToEdit.Source;
        Destination = jobToEdit.Destination;
    }

    private void Save()
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