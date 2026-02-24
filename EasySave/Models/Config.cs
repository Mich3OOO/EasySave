using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave.Models;

class ConfigData // This class is used to serialize and deserialize the config file, it is not used in the program itself
/// <summary>
/// This class is used to serialize and deserialize the config file, it is not 
/// used in the program itself
/// </summary>
{
    [JsonInclude]
    public Languages Language = Languages.En;
    [JsonInclude]  
    public LogsFormats LogsFormat  = LogsFormats.Json;
    [JsonInclude]
    public LogsMods LogsMods = LogsMods.Local;
    [JsonInclude]
    public string[] ExtensionsToEncrypt = [];
    [JsonInclude]
    public string[] CriticalExtensions = [];
    [JsonInclude]
    public List<SavedJob> SavedJobs = [];
    [JsonInclude]
    public string[] Softwares = [];
    [JsonInclude]
    public string API_URL = "http://localhost:8080/api/logs";
    [JsonInclude]
    public int MaxParallelLargeFileSizeKo = 10240;


    public ConfigData(Config _config)
    { 
        
        Language = _config.Language;
        LogsFormat  = _config.LogsFormat;
        LogsMods = _config.LogsMods;
        ExtensionsToEncrypt = _config.ExtensionsToEncrypt;
        SavedJobs = _config.SavedJobs;
        Softwares = _config.Softwares;
        API_URL = _config.API_URL;

        MaxParallelLargeFileSizeKo = _config.MaxParallelLargeFileSizeKo;
        CriticalExtensions = _config.CriticalExtensions;

    }
    public ConfigData()
    {}

}

/// <summary>
/// Class representing the configuration of the application, it is a singleton 
/// class that holds the selected language and the list of saved jobs
/// </summary>
public class Config
{
    private Languages _language;
    private LogsFormats _logsFormat;
    private LogsMods _logsMods;
    private string[] _extensionsToEncrypt;

    private string[] _softwares;
    public string[] _criticalExtensions = {};
    private string _API_URL;
    private static Config? s_instance;
    private List<SavedJob> _savedJobs;
    private readonly string _confPath = "./config.json";

    private int _maxParallelLargeFileSizeKo = 10240; 
    public Languages Language { get => _language; set => _language = value; }
    public LogsFormats LogsFormat { get => _logsFormat; set => _logsFormat = value; }
    public LogsMods LogsMods { get => _logsMods; set => _logsMods = value; }
    public string[] ExtensionsToEncrypt { get => _extensionsToEncrypt; set => _extensionsToEncrypt = value; }
    public string[] Softwares { get => _softwares; set => _softwares = value; }
    public string API_URL { get => _API_URL; set => _API_URL = value; }
    public List<SavedJob> SavedJobs { get => new List<SavedJob>(_savedJobs);}
    public string[] CriticalExtensions { get => _criticalExtensions; set => _criticalExtensions = value; }
    
    
    public int MaxParallelLargeFileSizeKo { get => _maxParallelLargeFileSizeKo; set => _maxParallelLargeFileSizeKo = value; }

    /// <summary>
    /// The constructor is private to prevent instantiation from outside the class
    /// </summary>
    private Config()
    {
        _savedJobs = null!;

        if (File.Exists(_confPath))
        {
            LoadConfig();
        }
        else
        {
            SetDefaultConfig();
            SaveConfig();
        }
    }

    /// <summary>
    /// Static method to get the single instance of the Config class
    /// </summary>
    /// <returns></returns>
    public static Config GetInstance()
    {
        s_instance ??= new Config();
        return s_instance;
    }

    /// <summary>
    /// Method to save the current configuration to the config file, it serializes 
    /// the ConfigStructure struct and writes it to the file
    /// </summary>
    public void SaveConfig()
    {
        if (File.Exists(_confPath)) File.Delete(_confPath);
        using FileStream fs = File.Open(_confPath, FileMode.CreateNew, FileAccess.Write);
        ConfigData config = new ConfigData(this);
        fs.Write(new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(config)));
    }

    /// <summary>
    /// Method to load the configuration from the config file, it reads the file, 
    /// deserializes it into a ConfigStructure struct and updates the current configuration accordingly
    /// </summary>
    private void LoadConfig()  
    {
        using FileStream fs = File.Open(_confPath, FileMode.OpenOrCreate, FileAccess.Read);
        string json = "";
        byte[] b = new byte[1024];
        UTF8Encoding temp = new UTF8Encoding(true);

        while (fs.Read(b, 0, b.Length) > 0)
        {
            json += temp.GetString(b);
        }

        ConfigData? config = JsonSerializer.Deserialize<ConfigData>(json.Trim('\0'));

            if (config is null)
            {
                SetDefaultConfig();
                SaveConfig();
            }
            else
            {
                SetConfig(config);
            }

        }
    }

    /// <summary>
    /// Method to set the default configuration, it is called when there is no 
    /// config file or when the config file is invalid
    /// </summary>
    private void SetDefaultConfig()
    {
        _language = Languages.En;
        _logsFormat = LogsFormats.Json;
        _savedJobs = [];
        _extensionsToEncrypt = [".txt", ".docx", ".xlsx", ".pdf"];
        _softwares = [];
        _criticalExtensions = [];
        _API_URL = "http://localhost:8080/api/logs";
        _maxParallelLargeFileSizeKo = 10240;
    }

    private void _setConfig(ConfigData config)
    {
        _language = config.Language;
        _logsFormat = config.LogsFormat;
        _logsMods = config.LogsMods;
        _extensionsToEncrypt = config.ExtensionsToEncrypt;
        _savedJobs = config.SavedJobs;
        _softwares = config.Softwares;
        _criticalExtensions = config.CriticalExtensions;
        _API_URL = config.API_URL ?? "http://localhost:8080/api/logs";
        _maxParallelLargeFileSizeKo = config.MaxParallelLargeFileSizeKo;
    }

    public bool AddJob(SavedJob job)    // Method to add a new job to the list of saved jobs, it takes a SavedJob object as a parameter and checks if a job with the same name and ID already exists in the list, if not, it adds the new job to the list and returns true, otherwise it returns false
    /// <summary>
    /// Method to add a new job to the list of saved jobs, it takes a SavedJob 
    /// object as a parameter and checks if a job with the same name and ID already 
    /// exists in the list, if not, it adds the new job to the list and returns 
    /// true, otherwise it returns false
    /// </summary>
    public bool AddJob(SavedJob job)
    {
        if (_savedJobs.FirstOrDefault(j => j.Name == job.Name && j.Id == job.Id) == null)
        {
            _savedJobs.Add(job);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Method to update a saved job, it takes the name of the job to update and 
    /// the new job data, it searches for the job with the given name and updates 
    /// its properties if found
    /// </summary>
    public void UpdateJob(string jobName, SavedJob job)
    {
        SavedJob? jobToUpdate = _savedJobs.FirstOrDefault(j => j.Name == jobName);

        if (jobToUpdate != null)
        {
            jobToUpdate.Name = job.Name;
            jobToUpdate.Destination = job.Destination;
            jobToUpdate.Source = job.Source;

        }
    }

    /// <summary>
    /// Method to get a saved job by its name, it searches for the job with the 
    /// given name and returns a copy of it if found, otherwise it returns null 
    /// </summary>
    public SavedJob? GetJob(string jobName) 
    {
        SavedJob? job = _savedJobs.FirstOrDefault(j => j.Name == jobName);
        return job is null ? null : new(job);
    }

    /// <summary>
    /// Method to get a saved job by its id, it searches for the job with the given 
    /// id and returns a copy of it if found, otherwise it returns null
    /// </summary>
    public SavedJob? GetJob(int id)
    {
        SavedJob? job = _savedJobs.FirstOrDefault(j => j.Id == id);
        return job is null ? null : new(job);
    }

    /// <summary>
    /// Method to delete a saved job, it takes the job to delete and removes it from 
    /// the list of saved jobs if it exists
    /// </summary>
    public void DeleteJob(SavedJob job)
    {
        _savedJobs.RemoveAll(j => j.Id == job.Id);
    }
}