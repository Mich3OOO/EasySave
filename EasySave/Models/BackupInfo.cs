namespace EasySave.Models;

public class BackupInfo
{
    public SavedJob SavedJobInfo;
    public CopyInfo LastCopyInfo;
    public CopyInfo CurrentCopyInfo;
    public int TotalFiles;
    public int CurrentFile;
}