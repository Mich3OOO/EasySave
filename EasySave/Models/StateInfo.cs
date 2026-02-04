namespace EasySave.Models;

public class StateInfo
{
    public string JobName;
    public DateTime LastCompleteSave;
    public StateLevel Status;
    public int TotalBackupSize;
    public int RemainingFiles;
    public int RemainingSize;
    public string CurrentFileSource;
    public string CurrentFileDestination;
}
