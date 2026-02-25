using System;
using System.Linq;
using System.Threading;
using EasySave.Models;
using EasySave.ViewModels;

namespace EasySave.Utils;

/// <summary>
/// Utility class for running backup jobs interactively from the console, allowing
/// </summary>
public static class ConsoleRunner
{
    public static void ListJobs()
    {
        var config = Config.GetInstance();
        var jobs = config.SavedJobs;

        int idWidth = 4;
        int nameWidth = 18;
        int srcWidth = 45;
        int dstWidth = 45;
        string h = "-";
        string v = "|";
        string hd = "+";
        string tl = "+";
        string tr = "+";
        string bl = "+";
        string br = "+";
        string th = "+";
        string bh = "+";
        string vl = "+";
        string vr = "+";
        string cross = "+";

        string line = tl + new string('-', idWidth + 2) + th + new string('-', nameWidth + 2) + th + new string('-', srcWidth + 2) + th + new string('-', dstWidth + 2) + tr;
        string header = v + Pad("ID", idWidth) + v + Pad("Name", nameWidth) + v + Pad("Source", srcWidth) + v + Pad("Destination", dstWidth) + v;
        string sep = vl + new string('-', idWidth + 2) + hd + new string('-', nameWidth + 2) + hd + new string('-', srcWidth + 2) + hd + new string('-', dstWidth + 2) + vr;
        string bottom = bl + new string('-', idWidth + 2) + bh + new string('-', nameWidth + 2) + bh + new string('-', srcWidth + 2) + bh + new string('-', dstWidth + 2) + br;

        Console.WriteLine(line);
        Console.WriteLine(header);
        Console.WriteLine(sep);
        foreach (var job in jobs)
        {
            Console.WriteLine(v +
                Center(job.Id.ToString(), idWidth) + v +
                Pad(job.Name, nameWidth) + v +
                Pad(Truncate(job.Source, srcWidth), srcWidth) + v +
                Pad(Truncate(job.Destination, dstWidth), dstWidth) + v);
        }
        Console.WriteLine(bottom);
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }

    private static string Pad(string value, int width)
    {
        if (value == null) value = "";
        if (value.Length > width) value = value.Substring(0, width);
        return value.PadRight(width);
    }

    private static string Center(string value, int width)
    {
        if (value == null) value = "";
        int left = (width - value.Length) / 2;
        int right = width - value.Length - left;
        return new string(' ', left) + value + new string(' ', right);
    }

    public static void RunJobInteractive(int jobId)
    {
        var config = Config.GetInstance();
        var job = config.SavedJobs.FirstOrDefault(j => j.Id == jobId);
        if (job == null)
        {
            Console.WriteLine($"Job {jobId} not found.");
            return;
        }

        // 1. Log format
        Console.WriteLine("Log format? (Json/Xml) [Json]: ");
        var logFormat = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(logFormat)) logFormat = "Json";

        // 2. Log mode
        Console.WriteLine("Log mode? (Local/Centralized/Both) [Local]: ");
        var logMode = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(logMode)) logMode = "Local";
        string apiUrl = string.Empty;
        if (logMode.Equals("Centralized", StringComparison.OrdinalIgnoreCase) || logMode.Equals("Both", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("API URL for centralized logs: ");
            apiUrl = Console.ReadLine() ?? string.Empty;
        }

        // 3. Business software
        Console.WriteLine("Business software to monitor (leave empty if none): ");
        var businessSoftware = Console.ReadLine();

        // 4. Critical files?
        Console.WriteLine("Are there critical files? (y/n) [n]: ");
        var critical = Console.ReadLine();
        string[] criticalExtensions = Array.Empty<string>();
        if (critical != null && critical.Trim().ToLower() == "y")
        {
            Console.WriteLine("Critical extensions separated by comma (e.g.: .exe,.dll): ");
            var ext = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ext))
                criticalExtensions = ext.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        // 5. Large file threshold
        Console.WriteLine("Large file size threshold (KB) [10240]: ");
        var thresholdStr = Console.ReadLine();
        int threshold = 10240;
        if (!string.IsNullOrWhiteSpace(thresholdStr) && int.TryParse(thresholdStr, out int parsedThreshold))
            threshold = parsedThreshold;

        // 6. Encrypt?
        Console.WriteLine("Encrypt files? (y/n) [n]: ");
        var encrypt = Console.ReadLine();
        string[] encryptedExtensions = Array.Empty<string>();
        string? password = null;
        if (encrypt != null && encrypt.Trim().ToLower() == "y")
        {
            Console.WriteLine("Extensions to encrypt separated by comma (e.g.: .txt,.docx): ");
            var ext = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ext))
                encryptedExtensions = ext.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            password = ReadPasswordWithValidation();
        }

        // Config summary
        Console.WriteLine("\n-----------------------------");
        Console.WriteLine("Configuration summary:");
        Console.WriteLine($"Log format: {logFormat}");
        Console.WriteLine($"Log mode: {logMode}");
        if (!string.IsNullOrWhiteSpace(apiUrl)) Console.WriteLine($"API URL: {apiUrl}");
        if (!string.IsNullOrWhiteSpace(businessSoftware)) Console.WriteLine($"Business software: {businessSoftware}");
        if (criticalExtensions.Length > 0) Console.WriteLine($"Critical extensions: {string.Join(", ", criticalExtensions)}");
        Console.WriteLine($"Large file threshold: {threshold} KB");
        if (encryptedExtensions.Length > 0) Console.WriteLine($"Extensions to encrypt: {string.Join(", ", encryptedExtensions)}");

        // Temporary config for this run
        var backupInfo = new BackupInfo { SavedJobInfo = job };
        Backup backup = job.Name.ToLower().Contains("diff")
            ? new DiffBackup(job, backupInfo, password ?? string.Empty)
            : new CompBackup(job, backupInfo, password ?? string.Empty);

        // Apply parameters to config instance (temporary)
        var configInstance = Config.GetInstance();
        configInstance.LogsFormat = logFormat.ToLower() == "xml" ? LogsFormats.Xml : LogsFormats.Json;
        configInstance.LogsMods = logMode.ToLower() switch
        {
            "centralized" => LogsMods.Centralized,
            "both" => LogsMods.Both,
            _ => LogsMods.Local
        };
        if (!string.IsNullOrWhiteSpace(apiUrl))
            configInstance.API_URL = apiUrl;
        if (criticalExtensions.Length > 0)
            configInstance.CriticalExtensions = criticalExtensions;
        if (!string.IsNullOrWhiteSpace(businessSoftware))
            configInstance.Softwares = new[] { businessSoftware };
        configInstance.MaxParallelLargeFileSizeKo = threshold;
        if (encryptedExtensions.Length > 0)
            configInstance.ExtensionsToEncrypt = encryptedExtensions;

        // Run backup with progress bar
        Console.WriteLine($"\nStarting backup for job {job.Id}: {job.Name}");
        RunWithProgress(backup, backupInfo);
        Console.WriteLine("Backup completed.");
    }

    private static void RunWithProgress(Backup backup, BackupInfo backupInfo)
    {
        var progressBarLength = 30;
        var lastProgress = -1;
        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        var progressThread = new Thread(() =>
        {
            while (backupInfo.TotalFiles == 0 || backupInfo.CurrentFile < backupInfo.TotalFiles)
            {
                if (backupInfo.TotalFiles > 0)
                {
                    int percent = (int)(100.0 * backupInfo.CurrentFile / backupInfo.TotalFiles);
                    if (percent != lastProgress)
                    {
                        lastProgress = percent;
                        int filled = progressBarLength * percent / 100;
                        string bar = $"[{new string('#', filled)}{new string(' ', progressBarLength - filled)}] {percent}% ({backupInfo.CurrentFile}/{backupInfo.TotalFiles})";
                        Console.Write($"\r{bar}");
                    }
                }
                Thread.Sleep(100);
            }
            if (backupInfo.TotalFiles > 0)
            {
                int percent = 100;
                int filled = progressBarLength;
                string bar = $"[{new string('#', filled)}] {percent}% ({backupInfo.TotalFiles}/{backupInfo.TotalFiles})";
                Console.Write($"\r{bar}");
            }
        });
        progressThread.Start();
        backup.ExecuteBackup();
        timer.Stop();
        progressThread.Join();
        Console.WriteLine($"\nDuration: {timer.Elapsed:mm\\:ss}");
    }

    private static string ReadPasswordWithValidation()
    {
        while (true)
        {
            Console.Write("Password for encryption: ");
            string password = string.Empty;
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();

            if (!RunJobsViewModel.IsPasswordValid(password))
            {
                Console.WriteLine("Error: Password must be at least 12 characters, with lowercase, uppercase, digit, and special character.");
                continue;
            }
            return password;
        }
    }
}
