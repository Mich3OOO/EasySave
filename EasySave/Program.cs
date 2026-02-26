using EasySave.Views;
using EasySave.ViewModels;
using Avalonia;
using EasySave.Models;

namespace EasySave;
internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (args[0].ToLower() == "list")
            {
                Utils.ConsoleRunner.ListJobs();
                return;
            }
            if (IsRange(args[0]) || IsList(args[0]))
            {
                RunJobsFromArgs(args[0]);
                return;
            }
            if (int.TryParse(args[0], out int singleId))
            {
                Utils.ConsoleRunner.RunJobInteractive(singleId);
                return;
            }
        }
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static bool IsRange(string arg) => arg.Contains('-');
    private static bool IsList(string arg) => arg.Contains(';');

    private static void RunJobsFromArgs(string arg)
    {
        var config = Config.GetInstance();
        var jobs = config.SavedJobs;
        var jobIds = new System.Collections.Generic.List<int>();

        if (IsRange(arg))
        {
            var parts = arg.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end))
            {
                for (int i = start; i <= end; i++)
                    jobIds.Add(i);
            }
        }
        else if (IsList(arg))
        {
            var parts = arg.Split(';');
            foreach (var p in parts)
                if (int.TryParse(p.Trim(), out int id))
                    jobIds.Add(id);
        }
        else if (int.TryParse(arg, out int singleId))
        {
            jobIds.Add(singleId);
        }

        foreach (var id in jobIds.Distinct())
        {
            var job = jobs.FirstOrDefault(j => j.Id == id);
            if (job == null)
            {
                
                continue;
            }

            var backupInfo = new BackupInfo { SavedJobInfo = job };
            Backup? backup = job is { } && job is SavedJob sj && sj is not null
                ? job.Name.ToLower().Contains("diff")
                    ? new DiffBackup(job, backupInfo)
                    : new CompBackup(job, backupInfo)
                : null;

            if (backup == null)
            {
                Console.WriteLine($"Could not create backup for job {id}.");
                continue;
            }

            Console.WriteLine($"Running job {id}: {job.Name}");
            backup.ExecuteBackup();
            Console.WriteLine($"Job {id} finished.");
        }
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace();
}
