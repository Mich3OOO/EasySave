namespace EasySave.Models;

public class BackupInfo // Class representing information about a backup operation, used for event updates
{
    public SavedJob SavedJobInfo;
    public CopyInfo LastCopyInfo;
    public CopyInfo CurrentCopyInfo;
    public int TotalFiles;
    public int CurrentFile;
}