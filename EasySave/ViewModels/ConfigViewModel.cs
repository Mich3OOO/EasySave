using EasySave.Models;

namespace EasySave.ViewModels;

/// <summary>
/// Class representing the config view model, it is responsible for managing
/// the configuration of the application, such as the language and the saved jobs
/// </summary>
public class ConfigViewModel    
{
    private Config _config;

    public ConfigViewModel()    // Constructor
    {
        _config = Config.GetInstance();
    }
    
    public void SaveConfig()    // Method to save the current configuration, it calls the SaveConfig method of the Config class
    {
        _config.SaveConfig();
    }
    public void SetLanguage(Languages lang) // Method to change the selected language, it updates the Language property of the Config class
    {
        _config.Language = lang;
    }
    public void SetLogsFormat(LogsFormats format)    // Method to change the selected logs format, it updates the LogsFormat property of the Config class
    {
        _config.LogsFormat = format;
    }

    public void SetLogsMods(LogsMods mods)    // Method to change the selected logs format, it updates the LogsFormat property of the Config class
    {
        _config.LogsMods = mods;
    }

    public void SetApiUrl(string url)
    {
        _config.API_URL = url;
    }

    public void ChangeJobName(string oldJobName, string newJobName) // Method to change the name of a saved job, it takes the old name of the job to update and the new name
    {
        SavedJob? job = _config.GetJob(oldJobName);
        if (job != null)
        { 
            job.Name = newJobName;
            _config.UpdateJob(oldJobName,job); 
        }
    }
    public void ChangeJobSource(string jobName, string newJobSource)    // Method to change the source of a saved job, it takes the name of the job to update and the new source
    {
        SavedJob? job = _config.GetJob(jobName);
        if (job != null)
        { 
            job.Source = newJobSource;
            _config.UpdateJob(jobName,job); 
        }
       
    }
    public void ChangeJobDestination(string jobName, string newDestination) // Method to change the destination of a saved job, it takes the name of the job to update and the new destination
    {
        SavedJob? job = _config.GetJob(jobName);
        if (job != null)
        { 
            job.Destination = newDestination;
            _config.UpdateJob(jobName,job); 
        }
    }
    public bool CreateJob(string name, string source, string destination) //    Method to create a new saved job, it takes the name, source and destination of the new job, it creates a new SavedJob object with the given values and adds it to the configuration using the AddJob method of the Config class, it returns true if the job was added successfully, false otherwise (e.g., if a job with the same name already exists)
    {
        Console.WriteLine($"2./ Source = {source}");    //debug
        List<SavedJob> savedJobs = _config.SavedJobs;
        return _config.AddJob(new SavedJob(savedJobs.Count > 0 ?savedJobs.Max(j=> j.Id) + 1: 1 ,name, source, destination));    // The ID of the new job is set to the maximum ID of the existing jobs plus one, or 1 if there are no existing jobs

    }
    public string[] GetJobsNames()  // Method to get the names of all saved jobs, it returns an array of strings containing the names of the saved jobs
    {
        
        return _config.SavedJobs.Select(s => s.Name).ToArray();
    }
    public SavedJob? GetJob(string name)    // Method to get a saved job by its name, it takes the name of the job to get and returns a copy of it if found, otherwise it returns null
    {
        return _config.GetJob(name);
    }
    
    public void DeleteJob(string name)  // Method to delete a saved job by its name, it takes the name of the job to delete, it searches for the job with the given name and deletes it if found
    {
        SavedJob? job = _config.GetJob(name);
        if (job != null)
        { 
            _config.DeleteJob(job);
        }
    }
}