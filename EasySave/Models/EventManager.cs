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

    // Private constructor (singleton) that initializes the subscribers list
    private EventManager()
    {
        _subscribers = new List<IEventListener>();
    }

    // Add the object in parameter to the subscribers list
    public void Subscribe(IEventListener listener)
    {
        this._subscribers.Add(listener);
    }

    // Notify all subscribers with BackupInfo
    public void Update(BackupInfo data)
    {
        foreach (IEventListener listener in _subscribers)
        {
            listener.Update(data);
        }
    }
}