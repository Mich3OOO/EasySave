using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave.Models;

struct ConfigStructure  // This struct is used to serialize and deserialize the config file, it is not used in the program itself
{
    // The JsonInclude attribute is used to include the fields in the serialization and deserialization process, even if they are not public properties
    [JsonInclude]
    public Languages SelectedLanguage;
    [JsonInclude]  
    public LogsFormats SelectedLogsFormat;
    [JsonInclude]
    public String[] SelectedExtensionsToEncrypt;
    [JsonInclude]
    public List<SavedJob> SavedJobs;
    
}

public class Config // Class representing the configuration of the application, it is a singleton class that holds the selected language and the list of saved jobs
{
    // The singleton pattern is used to ensure that there is only one instance of the Config class throughout the application, and it can be accessed globally via the S_GetInstance method
    private Languages _language;
    private LogsFormats _logsFormat;
    private String[] _ExtensionsToEncrypt;    // The default extensions to encrypt, it is set to a list of common document formats
    private static Config? s_instance;
    private List<SavedJob> _savedJobs;
    private readonly string _confPath = "./config.json";    // The path to the config file, it is set to the current directory with the name "config.json"
    public Languages Language { get => _language; set => _language = value; }
    public LogsFormats LogsFormat { get => _logsFormat; set => _logsFormat = value; }
    public String[] ExtensionsToEncrypt { get => _ExtensionsToEncrypt; set => _ExtensionsToEncrypt = value; }
    public List<SavedJob> SavedJobs { get => new List<SavedJob>(_savedJobs);}
    

    private Config()    // The constructor is private to prevent instantiation from outside the class
    {
        _savedJobs = null!;

        if (File.Exists(_confPath))
        {
            _loadConfig();
        }
        else
        {
            _setDefaultConfig();
            SaveConfig();
        }
    }

    public static Config S_GetInstance()    // Static method to get the single instance of the Config class
    {
        s_instance ??= new Config();
        return s_instance;
    }

    public void SaveConfig()    // Method to save the current configuration to the config file, it serializes the ConfigStructure struct and writes it to the file
    {
        if (File.Exists(_confPath)) File.Delete(_confPath);
        using(FileStream fs = File.Open(_confPath, FileMode.CreateNew, FileAccess.Write))
        {
            ConfigStructure config = new ConfigStructure(){SelectedLanguage = _language, SelectedLogsFormat = _logsFormat, SelectedExtensionsToEncrypt = _ExtensionsToEncrypt, SavedJobs = _savedJobs};
            fs.Write(new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(config)));
           
        }
    }

    private void _loadConfig()  //  Method to load the configuration from the config file, it reads the file, deserializes it into a ConfigStructure struct and updates the current configuration accordingly
    {
        using(FileStream fs = File.Open(_confPath, FileMode.OpenOrCreate, FileAccess.Read))
        {
            string json = "";
            byte[] b = new byte[1024];
            UTF8Encoding temp = new UTF8Encoding(true);

            while (fs.Read(b,0,b.Length) > 0)
            {
                json += temp.GetString(b);
            }
            
            ConfigStructure? config = JsonSerializer.Deserialize<ConfigStructure>(json.Trim('\0'));

            if (config is null)
            {
                _setDefaultConfig();
                SaveConfig();
            }
            else
            {
                _language = config.Value.SelectedLanguage;
                _logsFormat = config.Value.SelectedLogsFormat;
                _ExtensionsToEncrypt = config.Value.SelectedExtensionsToEncrypt;
                _savedJobs = config.Value.SavedJobs ?? new List<SavedJob>();
            }
            
        }
    }

    private void _setDefaultConfig()    // Method to set the default configuration, it is called when there is no config file or when the config file is invalid, it sets the default language to English and initializes an empty list of saved jobs
    {
        _language = Languages.En;
        _logsFormat = LogsFormats.Json;
        _savedJobs = new List<SavedJob>();
        _ExtensionsToEncrypt = new String[] { ".txt", ".docx", ".xlsx", ".pdf"};
    }

    public bool AddJob(SavedJob job)
    {
        if (_savedJobs.FirstOrDefault(j => j.Name == job.Name && j.Id == job.Id) == null)   // Check if a job with the same name and ID already exists in the list of saved jobs, if not, add the new job to the list and return true, otherwise return false
        {
            _savedJobs.Add(job);
            return true;
        }
        
        return false;
    }
    
    public void UpdateJob(string jobName,SavedJob job)  // Method to update a saved job, it takes the name of the job to update and the new job data, it searches for the job with the given name and updates its properties if found
    {
        
        SavedJob? jobToUpdate = _savedJobs.FirstOrDefault(j => j.Name == jobName);

        if (jobToUpdate != null)
        {
            jobToUpdate.Name = job.Name;
            jobToUpdate.Destination = job.Destination;
            jobToUpdate.Source= job.Source;
            
        }
    }
    public SavedJob? GetJob(string jobName) // Method to get a saved job by its name, it searches for the job with the given name and returns a copy of it if found, otherwise it returns null
    {
        SavedJob? job = _savedJobs.FirstOrDefault(j => j.Name == jobName);
        return  job is null ? null : new(job);
    }
    
    public SavedJob? GetJob(int id) // Method to get a saved job by its id, it searches for the job with the given id and returns a copy of it if found, otherwise it returns null
    {
        SavedJob? job = _savedJobs.FirstOrDefault(j => j.Id == id);
        return  job is null ? null : new(job);
    }
    public void DeleteJob(SavedJob job) // Method to delete a saved job, it takes the job to delete and removes it from the list of saved jobs if it exists
    {
        _savedJobs.RemoveAll(j => j.Id == job.Id);
    }

    

}