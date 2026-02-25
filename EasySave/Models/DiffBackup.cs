using EasySave.ViewModels;

namespace EasySave.Models;

/// <summary>
/// Class representing a differential backup, it inherits from the Backup 
/// class and overrides the ExecuteBackup and _getFilesList methods to perform 
/// a differential backup, creating a timestamped folder and copying only the 
/// files that have been modified since the last complete backup
/// </summary>
public class DiffBackup(SavedJob savedJob, BackupInfo backupInfo, string pw = "") : Backup(savedJob, backupInfo,pw) 
{
    /// <summary>
    /// Override of the ExecuteBackup method to perform a differential backup, 
    /// creating a timestamped folder and copying only the files that have been 
    /// modified since the last complete backup
    /// </summary>
    public override void ExecuteBackup()
    {
        var destinationPath = CreateTimestampedFolder("Differential");
        var jobManager = JobManager.GetInstance();
        
        var criticalFiles = SeparateCriticalFiles(out var notCriticalFiles);
        

        // Initialize the progress counter
        _backupInfo.TotalFiles = notCriticalFiles.Length + criticalFiles.Length;
        _backupInfo.CurrentFile = 0;
        
        foreach (var file in criticalFiles)
        {
            if (_cancel) break;
            BackupFile(file, destinationPath);
        }
        
        _isCriticalFileFinised = true;

        foreach (string file in notCriticalFiles)
        {
            if (_cancel) break;
            while (!jobManager.canRunNotCriticalJobs())
            {
                
            }
            BackupFile(file, destinationPath);
        }
    }

    /// <summary>
    /// Override of the _getFilesList method to return only the files that have 
    /// been modified since the last complete backup, it checks the creation date 
    /// of the last complete backup folder and compares it with the last write time 
    /// of each file in the source directory, if a file has been modified after the 
    /// last complete backup, it is added to the list of files to copy
    /// </summary>
    protected override string[] GetFilesList() 
    {
        var completeFolderPath = Path.Combine(_savedJob.Destination, "Complete");

        DateTime lastFullBackupDate = DateTime.MinValue;

        // Find the most recent Complete backup folder
        if (Directory.Exists(completeFolderPath))
        {
            DirectoryInfo? lastDir = new DirectoryInfo(completeFolderPath)
                .GetDirectories()
                .OrderByDescending(d => d.CreationTime)
                .FirstOrDefault();

            if (lastDir != null)
            {
                lastFullBackupDate = lastDir.CreationTime;
            }
        }
        else
        {
            throw new Exception(LanguageViewModel.GetInstance().GetTranslation("!complete_backup"));
        }


        if (lastFullBackupDate == DateTime.MinValue)
        {
            return base.GetFilesList();
        }

        var allFiles = base.GetFilesList();
        List<string> modifiedFiles = [];

        foreach (var file in allFiles)
        {
            if (File.GetLastWriteTime(file) > lastFullBackupDate)
            {
                modifiedFiles.Add(file);
            }
        }

        return [.. modifiedFiles];
    }
}
