using EasySave.ViewModels;
using EasySave.Models;

namespace EasySave.View;

public class ConsoleView
{
    // ViewModels are used to communicate with the logic layer
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
    /// <param name="questionKey">The translation key for the question</param>
    /// <returns>The user's input string</returns>
    private string _ask(string questionKey)
    {
        // Fetch the translated text from the dictionary with GetTranslation method
        string translatedQuestion = _languageViewModel.GetTranslation(questionKey);

        //Write the translated user input 
        Console.Write($"{translatedQuestion} > ");

        // Read user input
        string? input = Console.ReadLine();

        // Returns empty string if input is null to avoid crashes
        return input ?? string.Empty;
    }

    /// <summary>
    /// Entry point for commands. Handles both interactive menu and CLI arguments.
    /// </summary>
    /// <param name="command">The first argument (e.g., "1-3" or "1;3")</param>
    /// <param name="args">Additional arguments if necessary</param>
    public void RunCommand(string? command, string[] args)
    {
        // If no command is provided, launch the interactive menu
        if (string.IsNullOrEmpty(command))
        {
            ShowMainMenu();
        }
        else
        {
            // Process CLI arguments (Requirement: EasySave.exe 1-3 or 1;3)
            ProcessDirectCommand(command, BackupType.Complete); // Default to Complete backup for CLI
        }
    }

    /// <summary>
    /// Displays the main menu loop for interactive mode.
    /// </summary>
    private void ShowMainMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n--- EasySave Version 1.0 ---");
            Console.WriteLine($"1. {_languageViewModel.GetTranslation("menu_create")}");    //Choice 1: Create Backup Job (translated in selected language)
            Console.WriteLine($"2. {_languageViewModel.GetTranslation("menu_run")}");       //Choice 2: Run Backup Job (translated in selected language) 
            Console.WriteLine($"3. {_languageViewModel.GetTranslation("menu_exit")}");      //Choice 3: Exit (translated in selected language)

            string choice = _ask("menu_choice");    // Ask user for choice with _ask method

            switch (choice)
            {
                case "1":
                    CreateBackupJob();  //call CreateBackupJob method
                    break;
                case "2":
                    RunBackupSelection();   //call RunBackupSelection method
                    break;
                case "3":
                    exit = true;    // Set exit flag to true to break the loop
                    break;
                default:
                    Console.WriteLine(_languageViewModel.GetTranslation("error_invalid_choice")); // Handle invalid input
                    break;
            }
        }
    }

    /// <summary>
    /// Logic to create a new backup job (Max 5 jobs).
    /// </summary>
    private void CreateBackupJob()
    {
        Console.WriteLine($"\n--- {_languageViewModel.GetTranslation("title_new_job")} ---");

        string name = _ask("input_name");   // Ask for job name
        string source = _ask("input_source"); // Ask for source directory
        string target = _ask("input_target");   // Ask for target directory

        // create the job via the ConfigViewModel
        bool success = _configViewModel.CreateJob(name, source, target);

        if (success)
        {
            Console.WriteLine(_languageViewModel.GetTranslation("success_job_created"));
        }
        else
        {
            Console.WriteLine(_languageViewModel.GetTranslation("error_job_limit"));
        }
    }

    /// <summary>
    /// Handles the manual execution of jobs from the menu.
    /// </summary>
        private void RunBackupSelection()
        {
            string id = _ask("input_run_id");

            // Ask for the type here
            Console.WriteLine("1. Full");
            Console.WriteLine("2. Differential");
            string typeInput = _ask("input_type");

            BackupType selectedType = (typeInput == "2") ? BackupType.Differential : BackupType.Complete;

            // Now call the execution with the type
            ProcessDirectCommand(id, selectedType);
        }

    /// <summary>
    /// Parses the command string to identify ranges or lists of jobs to run.
    /// </summary>
    /// <param name="command">The command string (e.g., "1-3")</param>
    private void ProcessDirectCommand(string command, BackupType type)
    {
        try
        {
            // Call the BackupViewModel to run the specified range or list of jobs
            _backupViewModel.RunRangeBackup(command, type);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{_languageViewModel.GetTranslation("error_execution")}: {ex.Message}");
        }
    }
}