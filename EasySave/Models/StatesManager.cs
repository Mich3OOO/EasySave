using EasySave.Interfaces;

namespace EasySave.Models;

/// <summary>
/// Manages the collection of backup job states and handles state updates from backup operations.
/// Implements IEventListener to receive and process backup progress events.
/// </summary>
public class StatesManager: IEventListener
{
    private List<State> _states;
    private readonly string _statePath;
    
    /// <summary>
    /// Updates the state information based on backup progress data.
    /// This method is called when a backup operation reports its progress.
    /// </summary>
    /// <param name="data">The backup information containing current progress details.</param>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void Update(BackupInfo data)
    {
        throw new NotImplementedException();
    }
}