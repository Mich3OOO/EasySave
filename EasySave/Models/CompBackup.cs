namespace EasySave.Models;

/// <summary>
/// Class representing a complete backup operation, inheriting from the abstract Backup class
/// </summary>
public class CompBackup : Backup    
{
    public CompBackup(SavedJob savedJob, BackupInfo backupInfo,string pw = "") : base(savedJob, backupInfo,pw) { }   

    /// <summary>
    /// Override of the ExecuteBackup method to perform a complete backup, creating a timestamped folder and copying all files from the source to the destination
    /// </summary>
    public override void ExecuteBackup()    
    {
        string destinationPath = CreateTimestampedFolder("Complete");

        string[] files = GetFilesList();

        _backupInfo.TotalFiles = files.Length;
        _backupInfo.CurrentFile = 0;

        foreach (string file in files)
        {
            BackupFile(file, destinationPath);
        }
    }
}