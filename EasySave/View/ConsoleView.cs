using EasySave.ViewModels;
using EasySave.Models;

namespace EasySave.View;

public class ConsoleView
{
    // ViewModels used to communicate with the logic layer
    private readonly LanguageViewModel _languageViewModel;
    private readonly ConfigViewModel _configViewModel;
    private readonly BackupViewModel _backupViewModel;

    // Constructor to inject ViewModels
    public ConsoleView(LanguageViewModel langVM, ConfigViewModel configVM, BackupViewModel backupVM)
    {
        _languageViewModel = langVM;
        _configViewModel = configVM;
        _backupViewModel = backupVM;
    }

    /// <summary>
    /// Centralized method to ask a question to the user.
    /// Uses the LanguageViewModel to handle internationalization.
    /// </summary>
    private string _ask(string questionKey)
    {
        // Fetch the translated text from the dictionary
        string translatedQuestion = _languageViewModel.GetTranslation(questionKey);

        Console.Write($"{translatedQuestion} > ");
        string? input = Console.ReadLine();

        return input ?? string.Empty;
    }

    /// <summary>
    /// Entry point for commands. Handles both interactive menu and CLI arguments.
    /// </summary>
    public void RunCommand(string? command, string[] args)
    {
        // If no command is provided, launch the interactive menu
        if (string.IsNullOrEmpty(command))
        {
            ShowMainMenu();
        }
        else
        {
            // Process CLI arguments (Requirement: EasySave.exe 1-3)
            // Default to Complete backup for CLI to avoid blocking
            ProcessDirectCommand(command, BackupType.Complete);
        }
    }

    /// <summary>
    /// Displays the main menu loop for interactive mode
    /// </summary>
    private void ShowMainMenu()
    {
        bool exit = false;
        while (!exit)
        {
            // Clear console for a cleaner look
            Console.Clear();
            Console.WriteLine("\n--- EasySave Version 1.0 ---");

            // Display menu options using translations

            Console.WriteLine($"1. {_languageViewModel.GetTranslation("menu_create")}");
            Console.WriteLine($"2. {_languageViewModel.GetTranslation("menu_run")}");
            Console.WriteLine($"3. {_languageViewModel.GetTranslation("settings")} / {_languageViewModel.GetTranslation("language")}");
            Console.WriteLine($"4. {_languageViewModel.GetTranslation("menu_exit")}");

            string choice = _ask("menu_choice");

            switch (choice)
            {
                case "1":
                    CreateBackupJob();
                    break;
                case "2":
                    RunBackupSelection();
                    break;
                case "3":
                    ChangeLanguageMenu();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine(_languageViewModel.GetTranslation("error_invalid_choice"));
                    Console.WriteLine(_languageViewModel.GetTranslation("press_key"));
                    Console.ReadKey();
                    break;
            }
        }
    }

    /// <summary>
    /// Logic to create a new backup job.
    /// </summary>
    private void CreateBackupJob()
    {
        Console.WriteLine($"\n--- {_languageViewModel.GetTranslation("title_new_job")} ---");

        string name = _ask("input_name");
        string source = _ask("input_source");
        string target = _ask("input_target");

        // Create the job via the ConfigViewModel
        bool success = _configViewModel.CreateJob(name, source, target);

        if (success)
            Console.WriteLine(_languageViewModel.GetTranslation("success_job_created"));
        else
            Console.WriteLine(_languageViewModel.GetTranslation("error_job_limit"));

        // Pause to let user read the message
        Console.WriteLine(_languageViewModel.GetTranslation("press_key"));
        Console.ReadKey();
    }

    /// <summary>
    /// Handles the manual execution of jobs from the menu.
    /// </summary>
    private void RunBackupSelection()
    {
        string id = _ask("input_run_id");

        // Ask for the type using translations
        Console.WriteLine($"1. {_languageViewModel.GetTranslation("complete")}");
        Console.WriteLine($"2. {_languageViewModel.GetTranslation("differential")}");

        string typeInput = _ask("input_type");

        BackupType selectedType = (typeInput == "2") ? BackupType.Differential : BackupType.Complete;

        // Call the execution logic
        ProcessDirectCommand(id, selectedType);

        Console.WriteLine(_languageViewModel.GetTranslation("press_key"));
        Console.ReadKey();
    }

    /// <summary>
    /// Parses the command string to identify ranges or lists of jobs to run.
    /// </summary>
    private void ProcessDirectCommand(string command, BackupType type)
    {
        try
        {
            _backupViewModel.RunRangeBackup(command, type);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{_languageViewModel.GetTranslation("error_execution")}: {ex.Message}");
        }
    }

    /// <summary>
    /// method to handle language switching
    /// </summary>
    private void ChangeLanguageMenu()
    {
        Console.WriteLine($"\n--- {_languageViewModel.GetTranslation("settings")} ---");
        Console.WriteLine("en - English");
        Console.WriteLine("fr - Français");

        string lang = _ask("language");

        Languages newLanguage;
        
        if (Languages.TryParse(lang.ToUpper(), out newLanguage))
        {
            _languageViewModel.SetLanguage(newLanguage);
            Console.WriteLine($"{_languageViewModel.GetTranslation("language_changed")} {lang.ToUpper()}");
        }
        else
        {
            Console.WriteLine(_languageViewModel.GetTranslation("invalid_language"));
        }

        System.Threading.Thread.Sleep(1000); // Small pause
    }
}