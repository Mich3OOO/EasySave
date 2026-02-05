using EasySave.Interfaces;
using EasySave.ViewModels;

namespace EasySave.Models;

public abstract class Backup : IBackup
{

    protected SavedJob _savedJob;
    protected BackupInfo _backupInfo;
    protected LanguageViewModel _languageViewModel;

    public Backup(SavedJob savedJob, BackupInfo backupInfo, LanguageViewModel languageViewModel)
    {
        _savedJob = savedJob;
        _backupInfo = backupInfo;
        _languageViewModel = languageViewModel;
    }

    public abstract void ExecuteBackup();
    protected void _backupFile(string sourceFilePath)
    {
        try
        {   
            // Create CopyInfo to update EventManager
            CopyInfo copyInfo = new CopyInfo();
            copyInfo.Source = sourceFilePath;
            copyInfo.Size = new FileInfo(sourceFilePath).Length;

            // Calculate the relative path (e.g., "SubFolder/image.png")
            string relativePath = Path.GetRelativePath(_savedJob.Source, sourceFilePath);

            // Construct the full destination path (e.g., "D:/Backup/SubFolder/image.png")
            string targetFilePath = Path.Combine(_savedJob.Destination, relativePath);
            copyInfo.Destination = targetFilePath;

            // Create the destination directory if it doesn't exist
            string? targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (targetDirectory != null && !Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }


            // Perform the actual copy and get time
            copyInfo.StartTime = DateTime.Now;
            File.Copy(sourceFilePath, targetFilePath, true);
            copyInfo.EndTime = DateTime.Now;

            // Inform the observer
            _updateStatus(copyInfo);
        }
        catch (Exception ex)
        {
            // If ther is an error
            Console.WriteLine(string.Format(_languageViewModel.GetTranslation("error_copying_file"), sourceFilePath, ex.Message));
        }
    }

    protected virtual string[] _getFilesList() {
        return Directory.GetFiles(_savedJob.Source, "*", SearchOption.AllDirectories);
    }

    protected void _updateStatus(CopyInfo newCopyInfo)
    {
        // Set backupInfo information
        _backupInfo.SavedJobInfo = _savedJob;
        _backupInfo.LastCopyInfo = _backupInfo.CurrentCopyInfo;
        _backupInfo.CurrentCopyInfo = newCopyInfo;
        _backupInfo.TotalFiles = _getFilesList().Length;
        _backupInfo.CurrentFile = _backupInfo.CurrentFile++;

        EventManager.GetInstance().Update(_backupInfo);
    }
    
}