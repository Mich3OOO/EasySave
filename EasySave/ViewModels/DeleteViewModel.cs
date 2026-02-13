using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.ViewModels;

public class DeleteJobViewModel : ViewModelBase   // ViewModel for the confirmation dialog when deleting a backup job. It contains the job name to display in the dialog and commands for confirming or canceling the deletion.
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

    public event Action<bool>? OnResult;    // Event to notify the result of the confirmation, it takes a boolean parameter indicating whether the user confirmed (true) or canceled (false) the deletion

    public ICommand CancelCommand { get; }
    public ICommand ConfirmCommand { get; }

    public DeleteJobViewModel()   // Constructor initializes the commands for canceling and confirming the deletion
    {
        CancelCommand = new RelayCommand(Cancel);
        ConfirmCommand = new RelayCommand(Confirm);

        string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "dictionary.json");
        LanguageViewModel = new LanguageViewModel(dictionaryPath);
    }

    private void Cancel()   // Method to handle the cancel action, it invokes the OnResult event with false to indicate that the user canceled the deletion
    {
        OnResult?.Invoke(false);
    }

    private void Confirm()  // Method to handle the confirm action, it invokes the OnResult event with true to indicate that the user confirmed the deletion
    {
        OnResult?.Invoke(true);
    }
}
