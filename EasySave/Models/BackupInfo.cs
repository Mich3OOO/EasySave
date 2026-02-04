namespace EasySave.Models;

public class BackupInfo
{
    public Job JobInfo;
    public FinishedStep LastStep;
    public Step CurrentStep;
    public int TotalFiles;
    public int CurrentFile;
}