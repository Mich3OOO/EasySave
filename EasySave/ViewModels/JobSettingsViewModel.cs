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
        var job = _originalJob ?? new SavedJob();
        job.Name = Name;
        job.Source = Source;
        job.Destination = Destination;
        OnSaveRequested?.Invoke(job);
    }

    private void Cancel() => OnCancelRequested?.Invoke();
}