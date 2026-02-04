using EasySave.Interfaces;

namespace EasySave.Models;

public class StateManager: IEventListener
{
    private List<StateInfo> _states;
    private readonly string _statePath;
    
    
    public void Update(BackupInfo data)
    {
        throw new NotImplementedException();
    }

    private void Save()
    {
        throw new NotImplementedException();
    }
}