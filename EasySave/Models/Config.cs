using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySave.Models;

struct ConfigStructure
{
    [JsonInclude]
    public Languages SelectedLanguage;
    [JsonInclude]
    public List<SavedJob> SavedJobs;
    
}

public class Config
{
    private Languages _language;
    private static Config? s_instance;
    private List<SavedJob> _savedJobs;
    private readonly string _confPath = "./config.json";
    public Languages Language { get => _language; set => _language = value; }
    public List<SavedJob> SavedJobs { get => new List<SavedJob>(_savedJobs);}
    

    private Config()
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

    public static Config S_GetInstance()
    {
        s_instance ??= new Config();
        return s_instance;
    }

    public void SaveConfig()
    {
        if (File.Exists(_confPath)) File.Delete(_confPath);
        using(FileStream fs = File.Open(_confPath, FileMode.CreateNew, FileAccess.Write))
        {
            ConfigStructure config = new ConfigStructure(){SelectedLanguage = _language, SavedJobs = _savedJobs};
            fs.Write(new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(config)));
           
        }
    }

    private void _loadConfig()
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
            
            Console.WriteLine(json);
            ConfigStructure? config = JsonSerializer.Deserialize<ConfigStructure>(json.Trim('\0'));

            if (config is null)
            {
                _setDefaultConfig();
                SaveConfig();
            }
            else
            {
                _language = config.Value.SelectedLanguage;
                _savedJobs = config.Value.SavedJobs ?? new List<SavedJob>();
            }
            
        }
    }

    private void _setDefaultConfig()
    {
        _language = Languages.EN;
        _savedJobs = new List<SavedJob>();
    }

    public void AddJob(SavedJob job)
    {
        if (_savedJobs.FirstOrDefault(j => j.Name == job.Name && j.Id == job.Id) == null)
        {
            _savedJobs.Add(job);
        }
        
    }
    
    public void UpdateJob(string jobName,SavedJob job)
    {
        
        SavedJob? jobToUpdate = GetJob(jobName);

        if (jobToUpdate != null)
        {
            jobToUpdate.Name = job.Name;
            jobToUpdate.Destination = job.Destination;
            jobToUpdate.Source= job.Source;
            
        }
    }
    public SavedJob? GetJob(string jobName)
    {
        SavedJob? job = _savedJobs.FirstOrDefault(j => j.Name == jobName);
        return  job is null ? null : new(job);
    }
    public void DeleteJob(SavedJob job)
    {
        _savedJobs.Remove(job);
    }

    

}