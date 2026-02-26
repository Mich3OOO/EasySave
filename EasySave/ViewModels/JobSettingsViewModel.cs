using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;
using EasySave.Models.Exepctions;

namespace EasySave.ViewModels;

public class JobSettingsViewModel : ObservableObject 
{
    public LanguageViewModel LanguageViewModel { get; } = LanguageViewModel.GetInstance();

    public string T_job_settings => LanguageViewModel.GetTranslation("job_settings");
    public string T_job_name => LanguageViewModel.GetTranslation("job_name");
    public string T_example_job_name => LanguageViewModel.GetTranslation("example_job_name");
    public string T_source_folder => LanguageViewModel.GetTranslation("source_folder");
    public string T_source_path => LanguageViewModel.GetTranslation("source_path");
    public string T_target_folder => LanguageViewModel.GetTranslation("target_folder");
    public string T_target_path => LanguageViewModel.GetTranslation("target_path");
    public string T_cancel => LanguageViewModel.GetTranslation("cancel");
    public string T_save => LanguageViewModel.GetTranslation("save");

    private string _name = string.Empty;
    private string _source = string.Empty;
    private string _destination = string.Empty;
    private string _errorMessage = string.Empty;
    private string _password = string.Empty;

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

    public string Password
    {
        get => _destination;
        set => SetProperty(ref _password, value);
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
            ErrorMessage = string.Empty;

            SavedJob job = _originalJob ?? new SavedJob();
            job.Name = Name;
            job.Source = Source;
            job.Destination = Destination;

            OnSaveRequested?.Invoke(job);
        }
        catch (UserException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception)
        {
            Console.WriteLine($"Error saving job {_originalJob?.Name}");
        }
    }

    private void Cancel() => OnCancelRequested?.Invoke();
}
