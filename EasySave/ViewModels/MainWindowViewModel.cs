namespace EasySave.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to EasySave!";

    public string CustomCursorPath { get; set; } = "avares://EasySave/Assets/cursor.cur";
}