using EasySave.View;
using EasySave.ViewModels;

namespace EasySave;
internal class Program
{
    private static void Main(string[] args)
    {
        
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Define path to dictionary.json
        string dictionaryPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, 
            "..", "..", "..", 
            "Resources", 
            "dictionary.json"
        );

        // Initialize ViewModels (Dependency Injection)
        LanguageViewModel langVm = new LanguageViewModel(dictionaryPath);
        ConfigViewModel configVm = new ConfigViewModel();
        BackupViewModel backupVm = new BackupViewModel();

        // Create the View with the ViewModels
        ConsoleView view = new ConsoleView(langVm, configVm, backupVm);

        // Run the application
        string command = args.Length > 0 ? args[0] : string.Empty;

        view.RunCommand(command, args);
    }
}