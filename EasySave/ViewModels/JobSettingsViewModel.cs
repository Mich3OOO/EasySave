using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

namespace EasySave.ViewModels;

public class JobSettingsViewModel : ViewModelBase
{
    public LanguageViewModel LanguageViewModel { get; }

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

        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "dictionary.json");
        LanguageViewModel = new LanguageViewModel(dictionaryPath);
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