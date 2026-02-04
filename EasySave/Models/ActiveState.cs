namespace EasySave.Models;

public class ActiveState:State
{
  public int TotalBackupSize;
  public int RemainingFiles;
  public int RemainingSize;
  public string CurrentFileSource;
  public string CurrentFileDestination;
}