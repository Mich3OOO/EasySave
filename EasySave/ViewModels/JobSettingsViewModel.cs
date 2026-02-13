using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

namespace EasySave.ViewModels;

public class JobSettingsViewModel : ViewModelBase
{
    public LanguageViewModel _languageViewModel { get; }

    public string T_job_settings => _languageViewModel.GetTranslation("job_settings");
    public string T_job_name => _languageViewModel.GetTranslation("job_name");
    public string T_example_job_name => _languageViewModel.GetTranslation("example_job_name");
    public string T_source_folder => _languageViewModel.GetTranslation("source_folder");
    public string T_source_path => _languageViewModel.GetTranslation("source_path");
    public string T_target_folder => _languageViewModel.GetTranslation("target_folder");
    public string T_target_path => _languageViewModel.GetTranslation("target_path");
    public string T_cancel => _languageViewModel.GetTranslation("cancel");
    public string T_save => _languageViewModel.GetTranslation("save");

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

    public string Password   // Property for the password path, with getter and setter that raises property change notifications
    {
        get => _destination;
        set => SetProperty(ref _password, value);
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

    public event Action<SavedJob>? OnSaveRequested;
    public event Action? OnCancelRequested;

    public JobSettingsViewModel()
    {
        IsEditMode = false;
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);

        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "dictionary.json");
        _languageViewModel = new LanguageViewModel(dictionaryPath);
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

            var job = _originalJob ?? new SavedJob();
            job.Name = Name;
            job.SetSource(Source);
            job.SetDestination(Destination);


            OnSaveRequested?.Invoke(job);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"⚠️ {ex.Message}";
        }
    }

    private void Cancel() => OnCancelRequested?.Invoke();
}