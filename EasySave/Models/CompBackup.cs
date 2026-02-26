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
        var destinationPath = CreateTimestampedFolder("Complete");
        var jobManager = JobManager.GetInstance();

        var criticalFiles = SeparateCriticalFiles(out var notCriticalFiles);
        var files = GetFilesList();

        // Initialize the progress counter
        _backupInfo.TotalFiles = (uint) (notCriticalFiles.Length + criticalFiles.Length);
        _backupInfo.TotalFiles = (uint) files.Length;
        _backupInfo.CurrentFile = 0;

        
        foreach (var file in criticalFiles)
        {
            lock (_key)
            {
                if (_cancel) break;
                BackupFile(file, destinationPath);
            }
        }

        _isCriticalFileFinised = true;
        // Loop the files and execute copy
        foreach (var file in notCriticalFiles)
        {
            lock (_key)
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
}
