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
    private object _locker = new object();

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

    /// <summary>
    /// Add a job to the running jobs dictionary with the job name as key and the backup as value
    /// </summary>
    public void AddJob(SavedJob job,IBackup backup) 
    {
        _runningJobs.Add(job.Name,backup);
    }

    /// <summary>
    /// Cancel the job by removing it from the running jobs and calling the cancel method of the backup
    /// </summary>
    /// <param name="job"></param>
    public void CancelJob(SavedJob job) 
    {
        _runningJobs.Remove(job.Name,out IBackup backup);
        backup.Cancel();
    }

    /// <summary>
    /// Pause the job by calling the pause method of the backup
    /// </summary>
    public void PauseJob(SavedJob job) 
    {
        _runningJobs[job.Name].Pause();
    }

    /// <summary>
    /// Continue the job by calling the continue method of the backup
    /// </summary>
    public void ContinueJob(SavedJob job) 
    {
        _runningJobs[job.Name].Continue();
    }

    /// <summary>
    /// This method is called when the BackupInfo is updated, it 
    /// checks if the job is completed and if it is, it removes it 
    /// from the running jobs
    /// </summary>
    public void Update(BackupInfo data) 
    {
        if (data.CurrentFile == data.TotalFiles)
        {
            _runningJobs.Remove(data.SavedJobInfo.Name);
        }
    }
    
    public bool canRunNotCriticalJobs()
    {
        bool canRun = true;
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