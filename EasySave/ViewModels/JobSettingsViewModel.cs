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
    private bool _isDifferential = false; // Par défaut : Complet

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

    // Propriété pour le type de sauvegarde
    public bool IsDifferential
    {
        get => _isDifferential;
        set => SetProperty(ref _isDifferential, value);
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
        IsDifferential = jobToEdit.IsDifferential; // On récupère l'état existant
    }

    private void Save()
    {
        try
        {
            ErrorMessage = string.Empty;

            var job = _originalJob ?? new SavedJob();
            job.Name = Name;
            job.SetSource(Source);
            job.SetDestination(Destination);

            // On enregistre le type de sauvegarde dans le modèle
            job.IsDifferential = IsDifferential;

            OnSaveRequested?.Invoke(job);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"⚠️ {ex.Message}";
        }
    }

    private void Cancel() => OnCancelRequested?.Invoke();
}