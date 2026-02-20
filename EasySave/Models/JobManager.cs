using Avalonia.Vulkan;
using EasySave.Interfaces;

namespace EasySave.Models;

class JobManager:IEventListener
{
    private static JobManager s_Instance;
    private Dictionary<string,IBackup> _runningJobs;

    private JobManager()
    {
        EventManager.GetInstance().Subscribe(this);
    }

    public static JobManager GetInstance() => s_Instance ?? new();

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
}