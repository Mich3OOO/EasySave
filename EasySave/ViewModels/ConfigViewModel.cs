using EasySave.Models;

namespace EasySave.View;

public class ConfigViewModel
{
    private Config _config;

    public void SaveConfig()
    {
        throw new NotImplementedException();
    }
    public void SetLanguage(string lang)
    {
        throw new NotImplementedException();
    }
    public void ChangeJobSource(string jobSource, string newJobSource)
    {
        throw new NotImplementedException();
    }
    public void ChangeJobName(string oldJobName, string newJobName)
    {
        throw new NotImplementedException();
    }
    public void ChangeJobDestination(string jobName, string newDestination)
    {
        throw new NotImplementedException();
    }
    public void CreateJob(string name, string source, string destination)
    {
        throw new NotImplementedException();
    }
    public string[] GetJobsNames()
    {
        throw new NotImplementedException();
    }
    public SavedJob GetJob(string name)
    {
        throw new NotImplementedException();
    }
}