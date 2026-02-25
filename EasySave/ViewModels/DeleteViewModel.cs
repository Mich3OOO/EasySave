using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EasySave.ViewModels;

/// <summary>
/// ViewModel for the confirmation dialog when deleting a backup job. It contains 
/// the job name to display in the dialog and commands for confirming or canceling the deletion.
/// </summary>
public class DeleteViewModel : ObservableObject
{
    public LanguageViewModel LanguageViewModel { get; } = LanguageViewModel.GetInstance();
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
