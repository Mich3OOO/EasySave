using EasySave.Models;

namespace EasySave.ViewModels;

public class ConfigViewModel
{
    private Config _config;


    public ConfigViewModel()
    {
        _config = Config.S_GetInstance();
    }
    
    public void SaveConfig()
    {
        _config.SaveConfig();
    }
    public void SetLanguage(Languages lang)
    {
        _config.Language = lang;
    }
    public void ChangeJobSource(string jobName, string newJobSource)
    {
        SavedJob? job = _config.GetJob(jobName);
        if (job != null)
        { 
            job.Source = newJobSource;
            _config.UpdateJob(jobName,job); 
        }
       
    }
    public void ChangeJobName(string oldJobName, string newJobName)
    {
        SavedJob? job = _config.GetJob(oldJobName);
        if (job != null)
        { 
            job.Name = newJobName;
            _config.UpdateJob(oldJobName,job); 
        }
    }
    public void ChangeJobDestination(string jobName, string newDestination)
    {
        SavedJob? job = _config.GetJob(jobName);
        if (job != null)
        { 
            job.Destination = newDestination;
            _config.UpdateJob(jobName,job); 
        }
    }
    public bool CreateJob(string name, string source, string destination) 
    {
        List<SavedJob> savedJobs = _config.SavedJobs;
        return _config.AddJob(new SavedJob(savedJobs.Count > 0 ?savedJobs.Max(j=> j.Id) + 1: 1 ,name, source, destination));
    }
    public string[] GetJobsNames()
    {
        
        return _config.SavedJobs.Select(s => s.Name).ToArray();
    }
    public SavedJob? GetJob(string name)
    {
        return _config.GetJob(name);
    }
    
    public void DeleteJob(string name)
    {
        SavedJob? job = _config.GetJob(name);
        if (job != null)
        { 
            _config.DeleteJob(job);
        }
    }
}