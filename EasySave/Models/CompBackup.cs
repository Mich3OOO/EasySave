using EasySave.Interfaces;
using System.IO;

namespace EasySave.Models;

public class CompBackup : Backup, IBackup
{
    public CompBackup(SavedJob savedJob, BackupInfo backupInfo) : base(savedJob, backupInfo) { }

    public void ExecuteBackup()
    {
        // Access public properties 'Source' and 'Destination' from SavedJob
        DirectoryInfo sourceDir = new DirectoryInfo(SavedJob.Source);
        DirectoryInfo destinationDir = new DirectoryInfo(SavedJob.Destination);

        // Check if the source directory exists
        if (!sourceDir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDir.FullName}");
        }

        // Create destination directory if it doesn't exist
        if (!destinationDir.Exists)
        {
            destinationDir.Create();
        }

        // Get all files from source
        foreach (FileInfo file in sourceDir.GetFiles("*", SearchOption.AllDirectories))
        {
            // Maintain directory structure by calculating the relative path
            string relativePath = Path.GetRelativePath(sourceDir.FullName, file.FullName);
            string targetFilePath = Path.Combine(destinationDir.FullName, relativePath);

            // Create necessary subdirectories in the destination
            string? targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (targetDirectory != null)
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // Copy file and overwrite if it already exists
            file.CopyTo(targetFilePath, true);
        }
    }

    protected override string[] _getFilesList()
    {
        // Return the list of all files to be backed up
        return Directory.GetFiles(SavedJob.Source, "*", SearchOption.AllDirectories);
    }
}