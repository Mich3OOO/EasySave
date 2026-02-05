using EasySave.Interfaces;

namespace EasySave.Models;

public class StatesManager: IEventListener  // Class representing the states manager, implementing the IEventListener interface, it manages the states of backup jobs and saves them in a JSON file
{
    private List<State> _states;
    private readonly string _statePath;
    
    
    public void Update(BackupInfo data)
    {
        throw new NotImplementedException();
    }
}