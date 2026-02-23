namespace EasySave.Models;

public class CompBackup : Backup    // Class representing a complete backup operation, inheriting from the abstract Backup class
{
    public CompBackup(SavedJob savedJob, BackupInfo backupInfo,string pw = "") : base(savedJob, backupInfo,pw) { }    //Constructor

    public override void ExecuteBackup()    // Override of the ExecuteBackup method to perform a complete backup, creating a timestamped folder and copying all files from the source to the destination
    {
        string destinationPath = _createTimestampedFolder("Complete");

        // Get the list of all files
        string[] notCriticalFiles;
        
        string[] criticalFiles = _separateCriticalFiles(out notCriticalFiles);
        
        

        // Initialize the progress counter
        _backupInfo.TotalFiles = notCriticalFiles.Length;
        _backupInfo.CurrentFile = 0;
        
        foreach (string file in criticalFiles)
        {
            _backupFile(file, destinationPath);
        }
        
        _isCriticalFileFinised = true;
        // Loop the files and execute copy
        foreach (string file in notCriticalFiles)
        {
            // backup file
            _backupFile(file, destinationPath);
        }
    }
}