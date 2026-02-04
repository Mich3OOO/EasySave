using EasySave.Models;
using System.Security.Cryptography.X509Certificates;

namespace EasySave;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        // ============ Mock data to test the Logger ============
        SavedJob saveJobTest = new SavedJob(1, "saveJobTest", "C:\\sourceTest", "C:\\destinationTest"); // Double backslash avoid interpretation by C#
        CopyInfo copyInfoTest = new CopyInfo("\"C:\\\\sourceTest", "C:\\destinationTest", DateTime.Now, 50, DateTime.Now.AddMinutes(2));

        BackupInfo backupInfoTest = new BackupInfo();
        backupInfoTest.SavedJobInfo = saveJobTest;
        backupInfoTest.LastCopyInfo = copyInfoTest;
        backupInfoTest.CurrentCopyInfo = copyInfoTest;
        backupInfoTest.TotalFiles = 100;
        backupInfoTest.CurrentFile = 50;

        LogsManager logsManagerTest = new LogsManager();

        EventManager.GetInstance().Update(backupInfoTest);

        // When launched in VS, logs folder appears in EasySave/bin/Debug/net8.0/logs
    }
}