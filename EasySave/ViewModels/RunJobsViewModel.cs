using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySave.Models;

namespace EasySave.ViewModels;

public class RunJobsViewModel : ViewModelBase
{
    private bool _isDifferential = false;
    public SavedJob Job { get; }

    public bool IsDifferential
    {
        get => _isDifferential;
        set => SetProperty(ref _isDifferential, value);
    }

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<bool>? OnResult; // true = Lancer, false = Annuler

    public RunJobsViewModel(SavedJob job)
    {
        Job = job;
        ConfirmCommand = new RelayCommand(() => OnResult?.Invoke(true));
        CancelCommand = new RelayCommand(() => OnResult?.Invoke(false));
    }
}