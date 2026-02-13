using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.ViewModels;

public class ConfirmDeleteDialogViewModel : ViewModelBase
{
    private string _jobName = string.Empty;

    public string JobName
    {
        get => _jobName;
        set => SetProperty(ref _jobName, value);
    }

    public event Action<bool>? OnResult;

    public ICommand CancelCommand { get; }
    public ICommand ConfirmCommand { get; }

    public ConfirmDeleteDialogViewModel()
    {
        CancelCommand = new RelayCommand(Cancel);
        ConfirmCommand = new RelayCommand(Confirm);
    }

    private void Cancel()
    {
        OnResult?.Invoke(false);
    }

    private void Confirm()
    {
        OnResult?.Invoke(true);
    }
}
