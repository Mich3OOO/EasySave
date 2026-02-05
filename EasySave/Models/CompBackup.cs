using EasySave.Interfaces;

namespace EasySave.Models;

public class CompBackup : Backup    // Class representing a complete backup operation, inheriting from the abstract Backup class
{
    public CompBackup(SavedJob savedJob, BackupInfo backupInfo) : base(savedJob, backupInfo) { }    //Constructor


    public override void ExecuteBackup()    // Override the ExecuteBackup method to perform a complete backup
    {
        // Get the list of files (Strategy specific logic)
        string[] files = _getFilesList();

        // Loop through the files

        foreach (string file in files)
        {
            // Call the parent class
            _backupFile(file);
        }
    }
}