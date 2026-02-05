namespace EasySave.Models;

public abstract class State // Abstract class representing the state of a backup job
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