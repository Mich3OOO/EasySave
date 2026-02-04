using EasySave.Interfaces;

namespace EasySave.Models;

public class StatesManager: IEventListener
{
    private List<State> _states;
    private readonly string _statePath;
    
    
    public void Update(BackupInfo data)
    {
        throw new NotImplementedException();
    }
}