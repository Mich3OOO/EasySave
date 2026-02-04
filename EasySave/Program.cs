using EasySave.View;

namespace EasySave;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        // Créer une instance du LanguageViewModel avec le bon chemin
        string dictionaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "dictionary.json");
        var langViewModel = new LanguageViewModel(dictionaryPath);

        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== EasySave - Language Test ===\n");
            Console.WriteLine($"{langViewModel.GetTranslation("language")}: {langViewModel.GetCurrentLanguage().ToUpper()}\n");
            Console.WriteLine($"1. {langViewModel.GetTranslation("welcome")}");
            Console.WriteLine($"2. {langViewModel.GetTranslation("backup")}");
            Console.WriteLine($"3. {langViewModel.GetTranslation("settings")}");
            Console.WriteLine($"4. {langViewModel.GetTranslation("exit")}");
            Console.WriteLine($"\n--- {langViewModel.GetTranslation("commands")} ---");
            Console.WriteLine("lang [en/fr] - Change language");
            Console.WriteLine("quit - Exit program");
            Console.Write("\nEnter command: ");

            string? input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input == "quit")
            {
                running = false;
            }
            else if (input.StartsWith("lang "))
            {
                string lang = input.Substring(5).Trim();
                if (lang == "en" || lang == "fr")
                {
                    langViewModel.SetLanguage(lang);
                    // Retourne directement au menu avec la nouvelle langue
                }
                else
                {
                    Console.WriteLine($"\n{langViewModel.GetTranslation("error")}: {langViewModel.GetTranslation("invalid_language")}");
                    Console.WriteLine($"\n{langViewModel.GetTranslation("press_key")}");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine($"\n{langViewModel.GetTranslation("error")}: {langViewModel.GetTranslation("unknown_command")}");
                Console.WriteLine($"\n{langViewModel.GetTranslation("press_key")}");
                Console.ReadKey();
            }
        }

        Console.WriteLine($"\n{langViewModel.GetTranslation("exit")}...");
    }
}