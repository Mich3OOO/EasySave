namespace EasySave.Models;

public class Config
{
    private string _language;
    private static Config _instance;
    private List<Job> _savedJobs;
    private readonly string _confPath;

    private Config()
    {
        throw new NotImplementedException();
    }

    public static Config S_GetInstance()
    {
        throw new NotImplementedException();
    }

    public void SaveConfig()
    {
        throw new NotImplementedException();
    }

    private void _loadConfig()
    {
        throw new NotImplementedException();
    }
}