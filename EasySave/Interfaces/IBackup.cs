
namespace EasySave.Interfaces;

/// <summary>
/// Interface for backup operations
/// </summary>
public interface IBackup    
{
    public void ExecuteBackup();
    public void Pause();
    public void Continue();
    public void Cancel();
}