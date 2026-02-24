using EasySave.Interfaces;

namespace EasySave.Models;

/// <summary>
/// Class representing the event manager, implementing the IEventManager interface, 
/// it is a singleton class that manages the subscribers and notifies them with BackupInfo updates
/// </summary>
public class EventManager   
{
    private static EventManager? _instance;
    private readonly List<IEventListener> _subscribers;

    /// <summary>
    /// Create an EventManager singleton or return the existing one
    /// </summary>
    /// <returns></returns>
    public static EventManager GetInstance()
    {
        _instance ??= new EventManager();

        return _instance;
    }

    /// <summary>
    /// Private constructor (singleton) that initializes the subscribers list
    /// </summary>
    private EventManager()
    {
        _subscribers = [];
    }

    /// <summary>
    /// Add the object in parameter to the subscribers list
    /// </summary>
    public void Subscribe(IEventListener listener)
    {
        this._subscribers.Add(listener);
    }

    /// <summary>
    /// Remove the object from the subscribers list
    /// </summary>
    public void Unsubscribe(IEventListener listener)
    {
        if (this._subscribers.Contains(listener))
        {
            this._subscribers.Remove(listener);
        }
    }

    /// <summary>
    /// Notify all subscribers with BackupInfo
    /// </summary>
    public void Update(BackupInfo data)
    {
        foreach (IEventListener listener in _subscribers)
        {
            listener.Update(data);
        }
    }
}