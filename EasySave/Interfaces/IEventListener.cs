using EasySave.Models;

namespace EasySave.Interfaces;

/// <summary>
/// Interface for event listeners that respond to backup events
/// </summary>
public interface IEventListener
{
    public void Update(BackupInfo data);
}