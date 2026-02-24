using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.ViewModels;

/// <summary>
/// ViewModel for the confirmation dialog when deleting a backup job. It contains 
/// the job name to display in the dialog and commands for confirming or canceling the deletion.
/// </summary>
public class DeleteViewModel : ViewModelBase
{
    public LanguageViewModel _languageViewModel { get; }

    public string T_confirm_deletion => _languageViewModel.GetTranslation("confirm_deletion");
    public string T_are_you_sure_deletion => _languageViewModel.GetTranslation("are_you_sure_deletion");
    public string T_irreversible_action => _languageViewModel.GetTranslation("irreversible_action");
    public string T_cancel => _languageViewModel.GetTranslation("cancel");
    public string T_delete => _languageViewModel.GetTranslation("delete");

    private string _jobName = string.Empty;

    public string JobName
    {
        get => _jobName;
        set => SetProperty(ref _jobName, value);
    }

    /// <summary>
    /// Event to notify the result of the confirmation, it takes a boolean 
    /// parameter indicating whether the user confirmed (true) or canceled (false) the deletion
    /// </summary>
    public event Action<bool>? OnResult;

    public ICommand CancelCommand { get; }
    public ICommand ConfirmCommand { get; }

    /// <summary>
    /// Constructor initializes the commands for canceling and confirming the deletion
    /// </summary>
    public DeleteViewModel()
    {
        CancelCommand = new RelayCommand(Cancel);
        ConfirmCommand = new RelayCommand(Confirm);

        var dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "Dictionary.json");
        _languageViewModel = LanguageViewModel.GetInstance(dictionaryPath);
    }

    /// <summary>
    /// Method to handle the cancel action, it invokes the OnResult event with false 
    /// to indicate that the user canceled the deletion
    /// </summary>
    private void Cancel()
    {
        OnResult?.Invoke(false);
    }

    /// <summary>
    /// Method to handle the confirm action, it invokes the OnResult event with true
    /// to indicate that the user confirmed the deletion
    /// </summary>
    private void Confirm()
    {
        OnResult?.Invoke(true);
    }
}
