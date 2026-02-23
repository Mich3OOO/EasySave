
namespace EasySave.Interfaces;

public interface IBackup    // Interface for backup operations
{
    public void ExecuteBackup();
    public bool isCriticalCopyFinished();
    public void Pause();
    public void Continue();
    public void Cancel();
}