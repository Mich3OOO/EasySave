using Avalonia.Vulkan;
using EasySave.Interfaces;

namespace EasySave.Models;

/// <summary>
/// This class is responsible for managing the running jobs, it is a 
/// singleton class that keeps track of the running jobs and allows to 
/// cancel, pause and continue them, it also listens to the BackupInfo 
/// updates to remove the job from the running jobs when it is completed
/// </summary>
public class JobManager : IEventListener 
{
    private static JobManager? s_Instance;
    private readonly Dictionary<string,IBackup> _runningJobs;
    private readonly object _locker = new();

    /// <summary>
    /// Trivate constructor (singleton) that initializes the running 
    /// jobs dictionary and subscribes to the EventManager
    /// </summary>
    private JobManager()
    {
        _runningJobs = [];
        EventManager.GetInstance().Subscribe(this);
    }

    /// <summary>
    /// Singleton pattern to get the instance of the JobManager
    /// </summary>
    /// <returns></returns>
    public static JobManager GetInstance() 
    {
        s_Instance ??= new JobManager();
        return s_Instance;
    }

    public void AddJob(SavedJob job,IBackup backup) 
    {
        _runningJobs.Add(job.Name,backup);
    }

    public void CancelJob(SavedJob job) 
    {
        _runningJobs.Remove(job.Name,out IBackup backup);
        backup.Cancel();
    }

    public void PauseJob(SavedJob job) 
    {
        _runningJobs[job.Name].Pause();
    }

    public void ContinueJob(SavedJob job) 
    {
        _runningJobs[job.Name].Continue();
    }

    public void Update(BackupInfo data) 
    {
        if (data.CurrentFile == data.TotalFiles)
        {
            _runningJobs.Remove(data.SavedJobInfo.Name);
        }
    }
    
    public bool canRunNotCriticalJobs()
    {
        var canRun = true;
        foreach (IBackup backup in _runningJobs.Values)
        {
            lock (_locker)
            {
                canRun &= backup.isCriticalCopyFinished();
            }
        }
        return canRun;
    }
}
