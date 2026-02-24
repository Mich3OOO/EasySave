namespace EasySave.Models;

/// <summary>
/// Class representing information about a backup operation, used for event updates
/// </summary>
public class BackupInfo
{
    public required SavedJob SavedJobInfo;
    public CopyInfo? LastCopyInfo;
    public CopyInfo? CurrentCopyInfo;
    public int TotalFiles;
    public int CurrentFile;
}