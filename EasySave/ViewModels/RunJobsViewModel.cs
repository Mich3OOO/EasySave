using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

namespace EasySave.ViewModels;

public class RunJobsViewModel : ViewModelBase
{
    private bool _isDifferential = false;
    private string _password = string.Empty;
    public SavedJob Job { get; }

    public bool IsDifferential
    {
        get => _isDifferential;
        set => SetProperty(ref _isDifferential, value);
    }

    public LanguageViewModel LanguageViewModel { get; } // Property for the language view model, used to get translations for the UI

    public string Password   // Property for the password path, with getter and setter that raises property change notifications
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string T_launch_save => LanguageViewModel.GetTranslation("launch_save");
    public string T_what_type_save => LanguageViewModel.GetTranslation("what_type_save");
    public string T_complete => LanguageViewModel.GetTranslation("complete");
    public string T_differential => LanguageViewModel.GetTranslation("differential");
    public string T_password => LanguageViewModel.GetTranslation("password");
    public string T_cancel => LanguageViewModel.GetTranslation("cancel");
    public string T_launch => LanguageViewModel.GetTranslation("launch");





    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<bool>? OnResult; // true = Lancer, false = Annuler

    public RunJobsViewModel(SavedJob job)   //constructor
    {
        Job = job;
        ConfirmCommand = new RelayCommand(() => OnResult?.Invoke(true));
        CancelCommand = new RelayCommand(() => OnResult?.Invoke(false));
        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "dictionary.json");
        LanguageViewModel = new LanguageViewModel(dictionaryPath);
    }
}