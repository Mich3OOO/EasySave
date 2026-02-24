namespace EasySave.Models;

/// <summary>
/// Class representing a complete backup operation, inheriting from the abstract Backup class
/// </summary>
public class CompBackup : Backup
{
    public CompBackup(SavedJob savedJob, BackupInfo backupInfo,string pw = "") : base(savedJob, backupInfo,pw) { }

    /// <summary>
    /// Override of the ExecuteBackup method to perform a complete backup, creating a 
    /// timestamped folder and copying all files from the source to the destination
    /// </summary>
    public override void ExecuteBackup()
    {
        string destinationPath = CreateTimestampedFolder("Complete");
        JobManager jobManager = JobManager.GetInstance();
        
        string[] notCriticalFiles;

        string[] criticalFiles = _separateCriticalFiles(out notCriticalFiles);

        string[] files = GetFilesList();

        // Initialize the progress counter
        _backupInfo.TotalFiles = notCriticalFiles.Length + criticalFiles.Length;
        _backupInfo.TotalFiles = files.Length;
        _backupInfo.CurrentFile = 0;

        
        foreach (string file in criticalFiles)
        {
            if (_cancel) break;
            BackupFile(file, destinationPath);
            }

        _isCriticalFileFinised = true;
        // Loop the files and execute copy
        foreach (string file in notCriticalFiles)
        {
            if (_cancel) break;
            while (!jobManager.canRunNotCriticalJobs())
            {

            }
                // backup file
                BackupFile(file, destinationPath);
            }
    }
}
