using EasySave.ViewModels;
using EasySave.Models;

namespace EasySave.View;

public class ConsoleView
{
    // ViewModels used to communicate with the logic layer
    private readonly LanguageViewModel _languageViewModel;
    private readonly ConfigViewModel _configViewModel;
    private readonly BackupViewModel _backupViewModel;

    public ConsoleView(LanguageViewModel langVM, ConfigViewModel configVM, BackupViewModel backupVM)        // Constructor to inject ViewModels
    {
        _languageViewModel = langVM;
        _configViewModel = configVM;
        _backupViewModel = backupVM;
    }


    /// <summary>
    /// Centralized method to ask a question to the user.
    /// Uses the LanguageViewModel to handle internationalization.
    /// </summary>
    private string _ask(string questionKey) // Display a question to the user and get input, using the LanguageViewModel to translate the question
    {
        string translatedQuestion = _languageViewModel.GetTranslation(questionKey); //We use GetTranslation from the LanguageViewModel to get the translated question based on the provided key fr/en
        Console.Write($"{translatedQuestion} > ");
        string? input = Console.ReadLine();
        return input ?? string.Empty;
    }


    // <summary>
    /// Entry point for commands. Handles both interactive menu and CLI arguments.
    /// </summary>
    public void RunCommand(string? command, string[] args)  // If a command is passed as an argument, executes directly
    {
        if (string.IsNullOrEmpty(command))
        {
            ShowMainMenu(); // Display the main menu for interactive mode if no command is provided as an argument
        }
        else
        {
            ProcessDirectCommand(command, BackupType.Complete);
        }
    }

    /// <summary>
    /// Displays the main menu loop for interactive mode
    /// </summary>
    private void ShowMainMenu()
    {
        bool exit = false;  // Main menu loop, continues until the user chooses to exit
        while (!exit)
        {
            Console.Clear();


            // Logo ASCII
            string easySaveLogo = @"
 /$$$$$$$$                                     /$$$$$$                            
| $$_____/                                    /$$__  $$                           
| $$        /$$$$$$  /$$$$$$$ /$$   /$$      | $$  \__/  /$$$$$$  /$$    /$$ /$$$$$$ 
| $$$$$    |____  $$ /$$_____/| $$  | $$      |  $$$$$$  |____  $$|  $$  /$$//$$__  $$
| $$__/     /$$$$$$$|  $$$$$$ | $$  | $$       \____  $$  /$$$$$$$ \  $$/$$/| $$$$$$$$
| $$       /$$__  $$ \____  $$| $$  | $$       /$$  \ $$ /$$__  $$  \  $$$/ | $$_____/
| $$$$$$$$|  $$$$$$$ /$$$$$$$/|  $$$$$$$      |  $$$$$$/|  $$$$$$$   \  $/  |  $$$$$$$
|________/ \_______/|_______/  \____  $$       \______/  \_______/    \_/    \_______/
                               /$$  | $$                                              
                              |  $$$$$$/                                              
                               \______/                                               
";
            // Calling the method to display the logo with a rainbow gradient
            WriteRainbowGradient(easySaveLogo);

            Console.WriteLine("\n--- EasySave Version 1.0 ---");

            // Main menu options, using the LanguageViewModel to translate each option based on the user's language preference
            Console.WriteLine($"1. {_languageViewModel.GetTranslation("menu_create")}");
            Console.WriteLine($"2. Lister tous les jobs");
            Console.WriteLine($"3. Supprimer un job par le nom");
            Console.WriteLine($"4. Modifier un job par le nom");
            Console.WriteLine($"5. {_languageViewModel.GetTranslation("menu_run")}");
            Console.WriteLine($"6. {_languageViewModel.GetTranslation("settings")} / {_languageViewModel.GetTranslation("language")}");
            Console.WriteLine($"7. {_languageViewModel.GetTranslation("menu_exit")}");

            string choice = _ask("menu_choice");    //We catch the user's choice

            switch (choice)
            {
                case "1": CreateBackupJob(); break;
                case "2": ListAllJobs(); break;
                case "3": DeleteJobByName(); break;
                case "4": EditJobByName(); break;
                case "5": RunBackupSelection(); break;
                case "6": ChangeLanguageMenu(); break;
                case "7": exit = true; break;
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
    private void CreateBackupJob()  // Displays the prompts to create a new backup job and uses the ConfigViewModel to create it
    {
        Console.WriteLine($"\n--- {_languageViewModel.GetTranslation("title_new_job")} ---");
        string name = _ask("input_name");
        string source = _ask("input_source");
        string target = _ask("input_target");
        // If there is 2 saveJob with the same name
        if (_configViewModel.GetJob(name) != null)
        {
            Console.WriteLine(_languageViewModel.GetTranslation("error_job_exists"));
            Console.WriteLine(_languageViewModel.GetTranslation("press_key"));
            Console.ReadKey();
            return;

        }
        if (_configViewModel.CreateJob(name, source, target))   // We use CreateJob from the ConfigViewModel to create the new backup job
        {
            Console.WriteLine(_languageViewModel.GetTranslation("success_job_created"));
            _configViewModel.SaveConfig();

            // Récupérer le job créé
            var job = _configViewModel.GetJob(name);
            if (job != null)
            {
                Console.WriteLine($"ID: {job.Id}");
                Console.WriteLine($"Nom: {job.Name}");
                Console.WriteLine($"Source: {job.Source}");
                Console.WriteLine($"Destination: {job.Destination}");
            }
        }
        else
            Console.WriteLine(_languageViewModel.GetTranslation("error_job_limit"));
        Console.WriteLine(_languageViewModel.GetTranslation("press_key"));
        Console.ReadKey();
    }


    /// <summary>
    /// Handles the manual execution of jobs from the menu.
    /// </summary>
    private void RunBackupSelection()   // Displays the list of existing backup jobs and ask the user to select one
    {
        ListAllJobs();
        string id = _ask("input_run_id");
        Console.WriteLine($"1. {_languageViewModel.GetTranslation("complete")}\n2. {_languageViewModel.GetTranslation("differential")}");   // We ask the user to choose the type of backup (complete or differential) using the LanguageViewModel to translate the options
        string typeInput = _ask("input_type");  //Complete / DIfferential choice
        ProcessDirectCommand(id, (typeInput == "2") ? BackupType.Differential : BackupType.Complete);   // We use ProcessDirectCommand to execute the selected backup job with the chosen type, then we display a message to press a key to return to the menu
        Console.WriteLine(_languageViewModel.GetTranslation("press_key"));
        Console.ReadKey();
    }

    /// <summary>
    /// Parses the command string to identify ranges or lists of jobs to run.
    /// </summary>
    private void ProcessDirectCommand(string command, BackupType type)  // Exécute une commande de sauvegarde directement (utilisée pour les commandes passées en argument ou pour les sauvegardes sélectionnées dans le menu)
    {
        try { _backupViewModel.RunRangeBackup(command, type); }
        catch (Exception ex) { Console.WriteLine($"{_languageViewModel.GetTranslation("error_execution")}: {ex.Message}"); }    // On utilise RunRangeBackup du BackupViewModel pour exécuter la sauvegarde avec l'ID et le type spécifiés, puis on affiche un message de succès ou d'erreur selon le résultat
    }

    /// <summary>
    /// method to handle language switching
    /// </summary>
    private void ChangeLanguageMenu()   // Displays the language options and allows the user to change the application's language using the LanguageViewModel
    {
        Console.WriteLine($"\n--- {_languageViewModel.GetTranslation("settings")} ---\nen - English\nfr - Français");
        string lang = _ask("language");

        Languages newLanguage;

        if (Languages.TryParse(lang.ToUpper(), out newLanguage))
        {
            _languageViewModel.SetLanguage(newLanguage);    //We call the SetLanguage method from the LanguageViewModel
            Console.WriteLine($"{_languageViewModel.GetTranslation("language_changed")} {lang.ToUpper()}");
        }
        else
        {
            Console.WriteLine(_languageViewModel.GetTranslation("invalid_language"));
        }


        System.Threading.Thread.Sleep(1000);
    }

    /// <summary>
    /// Displays the provided text with a rainbow gradient effect in the console. PURELY ESTETHIC, IGNORE PLS Fabien :)
    /// </summary>
    private void WriteRainbowGradient(string text)
    {
        string[] lines = text.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
        int maxLen = 0;
        foreach (var l in lines) if (l.Length > maxLen) maxLen = l.Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                // Calcul of the hue based on the position of the character, creating a diagonal gradient effect across the text
                double hue = ((double)(x + y * 2) / (maxLen + lines.Length)) * 360;

                var (r, g, b) = HsvToRgb(hue % 360, 1.0, 1.0);
                Console.Write($"\u001b[38;2;{r};{g};{b}m{lines[y][x]}");
            }
            Console.WriteLine();
        }
        Console.Write("\u001b[0m");
    }

    /// <summary>
    /// Convert HSV color values to RGB. Used for the rainbow gradient effect in the console logo.
    /// </summary>
    private (int r, int g, int b) HsvToRgb(double h, double s, double v)
    {
        int hi = (int)Math.Floor(h / 60) % 6;
        double f = h / 60 - Math.Floor(h / 60);
        int p = (int)(v * (1 - s) * 255);
        int q = (int)(v * (1 - f * s) * 255);
        int t = (int)(v * (1 - (1 - f) * s) * 255);
        int vInt = (int)(v * 255);

        return hi switch
        {
            0 => (vInt, t, p),
            1 => (q, vInt, p),
            2 => (p, vInt, t),
            3 => (p, q, vInt),
            4 => (t, p, vInt),
            _ => (vInt, p, q)
        };
    }

    /// <summary>
    /// Displays all configured backup jobs, listing their details such as ID, Name, Source, and Destination.
    /// </summary>
    private void ListAllJobs()
    {
        var jobsNames = _configViewModel.GetJobsNames();
        if (jobsNames.Length == 0)
        {
            Console.WriteLine("Aucun job enregistré.");
        }
        else
        {
            Console.WriteLine("\n--- Liste des jobs ---");
            foreach (var name in jobsNames)
            {
                var job = _configViewModel.GetJob(name);
                if (job != null)
                {
                    Console.WriteLine($"ID: {job.Id}");
                    Console.WriteLine($"Nom: {job.Name}");
                    Console.WriteLine($"Source: {job.Source}");
                    Console.WriteLine($"Destination: {job.Destination}");
                    Console.WriteLine("---");
                }
            }
        }
        Console.WriteLine(_languageViewModel.GetTranslation("press_key"));
        Console.ReadKey();
    }

    /// <summary>
    /// Logic to delete a backup job by its name.
    /// </summary>
    private void DeleteJobByName()
    {
        string name = _ask("input_name");
        var job = _configViewModel.GetJob(name);
        if (job != null)
        {
            _configViewModel.DeleteJob(name);
            _configViewModel.SaveConfig();
            Console.WriteLine($"Job '{name}' supprimé.");
        }
        else
        {
            Console.WriteLine($"Aucun job trouvé avec le nom '{name}'.");
        }
        Console.WriteLine(_languageViewModel.GetTranslation("press_key"));
        Console.ReadKey();
    }

    /// <summary>
    /// Logic to edit an existing backup job.
    /// </summary>
    private void EditJobByName()
    {
        string name = _ask("input_name");
        var job = _configViewModel.GetJob(name);
        if (job != null)
        {
            string newName = _ask("input_new_name");
            string newSource = _ask("input_new_source");
            string newDestination = _ask("input_new_destination");
            _configViewModel.ChangeJobName(name, newName);
            _configViewModel.ChangeJobSource(newName, newSource);
            _configViewModel.ChangeJobDestination(newName, newDestination);
            _configViewModel.SaveConfig();
            Console.WriteLine($"Job '{name}' modifié.");
        }
        else
        {
            Console.WriteLine($"Aucun job trouvé avec le nom '{name}'.");
        }
        Console.WriteLine(_languageViewModel.GetTranslation("press_key"));
        Console.ReadKey();
    }
}