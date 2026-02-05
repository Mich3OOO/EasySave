namespace EasySave.Models;

public class DiffBackup : Backup
{
    public DiffBackup(SavedJob savedJob, BackupInfo backupInfo) : base(savedJob, backupInfo) { }

    public override void ExecuteBackup()
    {
        // Create the timestamped folder
        string destinationPath = _createTimestampedFolder("Differential");

        string[] filesToCopy = _getFilesList();

        // Initialize backup info data
        _backupInfo.TotalFiles = filesToCopy.Length;
        _backupInfo.CurrentFile = 0;

        foreach (string file in filesToCopy)
        {
            _backupFile(file, destinationPath);
        }
    }

    protected override string[] _getFilesList()
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