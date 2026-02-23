using EasySave.ViewModels;

namespace EasySave.Models;

public class DiffBackup : Backup
{
    string dictionaryPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "dictionary.json");

    public DiffBackup(SavedJob savedJob, BackupInfo backupInfo,string pw = "") : base(savedJob, backupInfo,pw) { }

    public override void ExecuteBackup()    // Override of the ExecuteBackup method to perform a differential backup, creating a timestamped folder and copying only the files that have been modified since the last complete backup
    {
        // Create the timestamped folder
        string destinationPath = _createTimestampedFolder("Differential");

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

        foreach (string file in notCriticalFiles)
        {
            _backupFile(file, destinationPath);
        }
    }

    protected override string[] _getFilesList() // Override of the _getFilesList method to return only the files that have been modified since the last complete backup, it checks the creation date of the last complete backup folder and compares it with the last write time of each file in the source directory, if a file has been modified after the last complete backup, it is added to the list of files to copy
    {
        // Define the path where Complete backups are stored
        string completeFolderPath = Path.Combine(_savedJob.Destination, "Complete");

        DateTime lastFullBackupDate = DateTime.MinValue; // Default value

        // Find the most recent Complete backup folder
        if (Directory.Exists(completeFolderPath))
        {
            // Get all directories, order by creation date desc
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
            throw new Exception(LanguageViewModel.GetInstance(dictionaryPath).GetTranslation("!complete_backup"));
        }


        if (lastFullBackupDate == DateTime.MinValue)
        {
            return base._getFilesList();
        }

        // Get file list
        string[] allFiles = base._getFilesList();
        List<string> modifiedFiles = new List<string>();

        foreach (string file in allFiles)
        {
            // Check if the source file has been modified AFTER the last full backup
            if (File.GetLastWriteTime(file) > lastFullBackupDate)
            {
                modifiedFiles.Add(file);
            }
        }

        return modifiedFiles.ToArray();
    }
}