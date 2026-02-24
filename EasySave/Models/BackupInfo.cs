namespace EasySave.Models;

/// <summary>
/// Class representing information about a backup operation, used for event updates
/// </summary>
public class BackupInfo
{
    public required SavedJob SavedJobInfo;
    public CopyInfo? CurrentCopyInfo;
    public CopyInfo? LastCopyInfo;
    public int CurrentFile;
    public int TotalFiles;
}