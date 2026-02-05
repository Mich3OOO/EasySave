using EasySave.Interfaces;

namespace EasySave.Models;

public class StateManager: IEventListener
{
    private List<StateInfo> _states;

    private readonly string _stateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "state");
    private readonly string _stateFilePath;

    public StateManager()
    {
        EventManager.GetInstance().Subscribe(this);

        _stateFilePath = Path.Combine(_stateDirectory, "state.json");

        _states = new List<StateInfo>()

        if (File.Exists(_statePath))
        {
            try
            {
                string json = File.ReadAllText(_statePath);
                _states = JsonSerializer.Deserialize<List<StateInfo>>(json) ?? new List<StateInfo>();
            }
            catch
            {
                _states = new List<StateInfo>();
            }
        }
    }
    
    public void Update(StateInfo data)
    {
        string json = _toJson(data);
        this.Save(json);
    }

    private void Save(string json)
    {
        string _path = _getFileName();
        System.IO.File.Replace(_path, json + System.Environment.NewLine);
    }

    // Serializes the StateInfo object to a 'JSON' string format.
    private string _toJson(StateInfo data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "Backup job data cannot be null.");
        }

        int progression = 0;
        if (data.TotalBackupSize > 0)
        {
            // how much has been copied so far
        }

        return $@"{{
        ""Name"": ""{data.JobName}"",
        ""SourceFilePath"": ""{data.CurrentFileSource}"",
        ""TargetFilePath"": ""{data.CurrentFileDestination}"",
        ""State"": ""{data.Status.ToString().ToUpper()}"", 
        ""TotalFilesToCopy"": {data.RemainingFiles}, 
        ""TotalFilesSize"": {data.TotalBackupSize},
        ""NbFilesLeftToDo"": {data.RemainingFiles},
        ""Progression"": {progression}
    }}";
    }

    // Return the path of the current log file (based on the current date)
    private string _getFileName()
    {
        System.IO.Directory.CreateDirectory(_statePath); // Create the logs directory if it doesn't exist
        string _fileName = DateTime.Now.ToString("state.json"; // Assemble the file name based on the current date
        return System.IO.Path.Combine(_logsPath, _fileName); // Assemble the path with the file name (better than a concatenation because it handles / and )
    }
}