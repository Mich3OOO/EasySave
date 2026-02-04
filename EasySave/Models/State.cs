namespace EasySave.Models;

public abstract class State
{
    public string JobName;
    public DateTime LastCompleteSave;
    public bool IsActive;
    public int TotalBackupSize;
    public int RemainingFiles;
    public int RemainingSize;
    public string CurrentFileSource;
    public string CurrentFileDestination;
    
}