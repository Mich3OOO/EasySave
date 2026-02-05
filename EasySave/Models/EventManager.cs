using EasySave.Interfaces;

namespace EasySave.Models;

public class EventManager
{
    private EventManager _instance;
    private List<IEventListener> _subscribers;

    public EventManager GetInstance()
    {
        throw new NotImplementedException();
    }

    private EventManager()
    {
        throw new NotImplementedException();
    }

    public void Subscribe(IEventListener listener)
    {
        throw new NotImplementedException();
    }

    public void Update(BackupInfo data)
    {
        throw new NotImplementedException();
    }
}