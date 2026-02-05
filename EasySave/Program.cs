using EasySave.View;
using EasySave.ViewModels;
using EasySave.Models; // Si besoin

internal class Program
{
    private static void Main(string[] args)
    {
        LogsManager logsManager = new LogsManager(); // Initialization of the LogsManager, needed for backup logging (called by EventManager)
        // 1. Setup Encoding for accents
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // 2. Define path to dictionary.json
        string dictionaryPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, 
            "..", "..", "..", 
            "Resources", 
            "dictionary.json"
        );

        // 3. Initialize ViewModels (Dependency Injection)
        // Note: Make sure LanguageViewModel constructor accepts the path!
        LanguageViewModel langVM = new LanguageViewModel(dictionaryPath);
        ConfigViewModel configVM = new ConfigViewModel();
        BackupViewModel backupVM = new BackupViewModel(); // Assure-toi que le constructeur est vide ou géré

        // 4. Create the View with the ViewModels
        ConsoleView view = new ConsoleView(langVM, configVM, backupVM);

        // 5. Run the application
        // Check if there are arguments (Command Line Mode) or not (Menu Mode)
        string command = args.Length > 0 ? args[0] : null;

        view.RunCommand(command, args);
    }
}