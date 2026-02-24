using EasySave.Interfaces;

namespace EasySave.Models;

/// <summary>
/// Class representing the event manager, implementing the IEventManager interface, 
/// it is a singleton class that manages the subscribers and notifies them with BackupInfo updates
/// </summary>
public class EventManager   
{
    private static EventManager? _instance;
    private List<IEventListener> _subscribers;

    /// <summary>
    /// Create an EventManager singleton or return the existing one
    /// </summary>
    /// <returns></returns>
    public static EventManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new EventManager();
        }

        return _instance;
    }

    /// <summary>
    /// Private constructor (singleton) that initializes the subscribers list
    /// </summary>
    private EventManager()
    {
        _subscribers = new List<IEventListener>();
    }

    /// <summary>
    /// Add the object in parameter to the subscribers list
    /// </summary>
    /// <param name="listener"></param>
    public void Subscribe(IEventListener listener)
    {
        this._subscribers.Add(listener);
    }

    /// <summary>
    /// Notify all subscribers with BackupInfo
    /// </summary>
    /// <param name="data"></param>
    public void Update(BackupInfo data)
    {
        foreach (IEventListener listener in _subscribers)
        {
            listener.Update(data);
        }
    }
}