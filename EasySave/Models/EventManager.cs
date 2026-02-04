using EasyLog;
using EasySave.Interfaces;

namespace EasySave.Models;

public class EventManager
{
    private static EventManager _instance;
    private List<IEventListener> _subscribers;

    // Create an EventManager singleton or return the existing one
    public static EventManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new EventManager();
        }

        return _instance;
    }

    private EventManager()
    {
        _subscribers = new List<IEventListener>();
    }

    public void Subscribe(IEventListener listener)
    {
        throw new NotImplementedException();
    }

    // Notify all subscribers with BackupInfo
    public void Update(BackupInfo data)
    {
        foreach(IEventListener listener in _subscribers)
        {
            listener.Update(data);
        }
    }
}