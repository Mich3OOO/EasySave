using EasySave.Interfaces;

namespace EasySave.Models;

public abstract class Backup : IBackup  // Abstract class representing a backup operation, implementing the IBackup interface
{
    protected SavedJob _savedJob;
    protected BackupInfo _backupInfo;

    public Backup(SavedJob savedJob, BackupInfo backupInfo) // Constructor 
    {
        _savedJob = savedJob;
        _backupInfo = backupInfo;
    }

    public abstract void ExecuteBackup();

    //Create a timestamp folder for backup
    protected string _createTimestampedFolder(string subFolderType)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fullPath = Path.Combine(_savedJob.Destination, subFolderType, timestamp);
        Directory.CreateDirectory(fullPath);
        return fullPath;
    }

    protected void _backupFile(string sourceFilePath, string destinationPath)
    {
        try
        {
            // Prepare copy information
            CopyInfo copyInfo = new CopyInfo();
            copyInfo.Source = sourceFilePath;
            copyInfo.Size = new FileInfo(sourceFilePath).Length;

            // Calculate relative path to maintain structure
            string relativePath = Path.GetRelativePath(_savedJob.Source, sourceFilePath);

            // Construct full target path using specific destination folder
            string targetFilePath = Path.Combine(destinationPath, relativePath);
            copyInfo.Destination = targetFilePath;

            // Create destination directory if it doesn't exist
            string? targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (targetDirectory != null && !Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // Make copy with time
            copyInfo.StartTime = DateTime.Now;
            File.Copy(sourceFilePath, targetFilePath, true);
            copyInfo.EndTime = DateTime.Now;

            // Notify observer
            _updateStatus(copyInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error copying file {sourceFilePath}: {ex.Message}");
        }
    }

    protected virtual string[] _getFilesList()
    {
        // Return all files to backup
        return Directory.GetFiles(_savedJob.Source, "*", SearchOption.AllDirectories);
    }

    protected void _updateStatus(CopyInfo newCopyInfo)  // Update the backup information and notify the EventManager
    {
        // Update general info
        _backupInfo.SavedJobInfo = _savedJob;
        _backupInfo.LastCopyInfo = _backupInfo.CurrentCopyInfo;
        _backupInfo.CurrentCopyInfo = newCopyInfo;
        _backupInfo.CurrentFile++;

        // Notify Observer
        EventManager.GetInstance().Update(_backupInfo);
    }
}