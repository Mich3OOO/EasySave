namespace EasySave.Models;

public class CompBackup : Backup
{
    public CompBackup(SavedJob savedJob, BackupInfo backupInfo) : base(savedJob, backupInfo) { }

    public override void ExecuteBackup()
    {
        string destinationPath = _createTimestampedFolder("Complete");

        // Get the list of all files
        string[] files = _getFilesList();

        // Initialize the progress counter
        _backupInfo.TotalFiles = files.Length;
        _backupInfo.CurrentFile = 0;

        // Loop the files and execute copy
        foreach (string file in files)
        {
            // backup file
            _backupFile(file, destinationPath);
        }
    }
}