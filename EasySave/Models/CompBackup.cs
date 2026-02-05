using EasySave.Interfaces;
using EasySave.ViewModels;

namespace EasySave.Models;

public class CompBackup : Backup
{
    public CompBackup(SavedJob savedJob, BackupInfo backupInfo, LanguageViewModel languageViewModel) : base(savedJob, backupInfo, languageViewModel) { }


    public override void ExecuteBackup()
    {
        // Get the list of files (Strategy specific logic)
        string[] files = _getFilesList();

        // Loop through the files
        foreach (string file in files)
        {
            // Call the parent class
            _backupFile(file);
        }
    }
}