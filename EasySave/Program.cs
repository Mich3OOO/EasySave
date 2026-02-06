using EasySave.View;
using EasySave.ViewModels;
using EasySave.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        LogsManager logsManager = new LogsManager(); // Initialization of the LogsManager, needed for backup logging (called by EventManager)
        // Setup Encoding for accents
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Define path to dictionary.json
        string dictionaryPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, 
            "..", "..", "..", 
            "Resources", 
            "dictionary.json"
        );

        // Initialize ViewModels (Dependency Injection)
        LanguageViewModel langVM = new LanguageViewModel(dictionaryPath);
        ConfigViewModel configVM = new ConfigViewModel();
        BackupViewModel backupVM = new BackupViewModel();

        // Create the View with the ViewModels
        ConsoleView view = new ConsoleView(langVM, configVM, backupVM);

        // Run the application
        string command = args.Length > 0 ? args[0] : null;

        view.RunCommand(command, args);
    }
}