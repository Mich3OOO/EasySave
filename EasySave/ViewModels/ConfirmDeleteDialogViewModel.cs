using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.ViewModels;

public class ConfirmDeleteDialogViewModel : ViewModelBase
{
    public LanguageViewModel LanguageViewModel { get; }

    public string T_confirm_deletion => LanguageViewModel.GetTranslation("confirm_deletion");
    public string T_are_you_sure_deletion => LanguageViewModel.GetTranslation("are_you_sure_deletion");
    public string T_irreversible_action => LanguageViewModel.GetTranslation("irreversible_action");
    public string T_cancel => LanguageViewModel.GetTranslation("cancel");
    public string T_delete => LanguageViewModel.GetTranslation("delete");

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

        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "dictionary.json");
        LanguageViewModel = new LanguageViewModel(dictionaryPath);
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
