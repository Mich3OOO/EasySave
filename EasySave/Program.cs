using EasySave.Models;

namespace EasySave;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== Test de StateManager ===");
        
        // Créer une instance de StateInfo
        StateInfo stateinfo = new StateInfo
        {
            JobName = "MonBackup1",
            LastCompleteSave = DateTime.Now,
            Status = StateLevel.Active,
            TotalBackupSize = 1024000,
            RemainingFiles = 10,
            RemainingSize = 512000,
            CurrentFileSource = @"C:\Source\fichier.txt",
            CurrentFileDestination = @"D:\Backup\fichier.txt"
        };
        
        Console.WriteLine($"Job Name: {stateinfo.JobName}");
        Console.WriteLine($"Last Complete Save: {stateinfo.LastCompleteSave}");
        Console.WriteLine($"Status: {stateinfo.Status}");
        Console.WriteLine($"Total Backup Size: {stateinfo.TotalBackupSize} bytes");
        Console.WriteLine($"Remaining Files: {stateinfo.RemainingFiles}");
        Console.WriteLine($"Remaining Size: {stateinfo.RemainingSize} bytes");
        Console.WriteLine($"Current File Source: {stateinfo.CurrentFileSource}");
        Console.WriteLine($"Current File Destination: {stateinfo.CurrentFileDestination}");
        
        Console.WriteLine("\n=== Test de StateLevel ===");
        Console.WriteLine($"Active: {StateLevel.Active}");
        Console.WriteLine($"Inactive: {StateLevel.Inactive}");
        Console.WriteLine($"End: {StateLevel.End}");
        
        // Tester les changements d'état
        Console.WriteLine("\n=== Changement d'état ===");
        stateinfo.Status = StateLevel.Inactive;
        Console.WriteLine($"État changé à: {stateinfo.Status}");

        stateinfo.Status = StateLevel.End;
        Console.WriteLine($"État changé à: {stateinfo.Status}");
        
        Console.WriteLine("\n=== Test terminé avec succès ! ===");
        Console.WriteLine("Appuyez sur une touche pour quitter...");
        Console.ReadKey();
    }
}