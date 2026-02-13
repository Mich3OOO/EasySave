using EasySave.Interfaces;
using System.Text.Json;
using System.Text;

namespace EasySave.Models;

/// <summary>
/// Manages the real-time state of backup jobs, handling persistence to a JSON file 
/// and responding to backup events via the Observer pattern.
/// </summary>
public class StateManager : IEventListener   // Class representing the state manager, implementing the IEventListener interface, it manages the state of backup jobs and saves it in a local JSON file
{
    private List<StateInfo> _states;

    private readonly string _stateFilePath = "./states.json";

    /// <summary>
    /// Initializes a new instance of the StateManager.
    /// Subscribes to the EventManager and loads existing states from the local JSON file.
    /// </summary>
    public StateManager()   // Constructor that initializes the state manager, subscribes to events, and loads existing states from the JSON file
    {
        EventManager.GetInstance().Subscribe(this);

        _states = new List<StateInfo>();

        if (File.Exists(_stateFilePath))
        {
            try
            {
                string json = File.ReadAllText(_stateFilePath);
                _states = JsonSerializer.Deserialize<List<StateInfo>>(json) ?? new List<StateInfo>();
            }
            catch
            {
                _states = new List<StateInfo>();
            }
        }
    }

    /// <summary>
    /// Updates the state of a specific backup job based on provided BackupInfo.
    /// Triggered whenever a file is copied or a job status changes.
    /// </summary>
    /// <param name="data">The current data context of the backup job.</param>
    public void Update(BackupInfo data) // Method that updates the state of a backup job based on the provided BackupInfo, it is triggered whenever a file is copied or a job status changes
    {
        StateInfo? editedJobState = _states.FirstOrDefault(s => s.Name == data.SavedJobInfo.Name);

        if (editedJobState == null)
        {
            editedJobState = new()
            {
                Name = data.SavedJobInfo.Name,
            };
            _states.Add(editedJobState);
        }

        if (data.CurrentFile != data.TotalFiles)    // If the current file being copied is not the last one, update the state to active and fill in the relevant information
        {
            editedJobState.SourceFilePath = data.CurrentCopyInfo.Source;
            editedJobState.TargetFilePath = data.CurrentCopyInfo.Destination;
            editedJobState.State = StateLevel.Active;
            editedJobState.TotalFilesToCopy = data.TotalFiles;
            editedJobState.TotalFilesSize = editedJobState.TotalFilesSize + data.CurrentCopyInfo.Size;
            editedJobState.NbFilesLeftToDo = data.TotalFiles - data.CurrentFile;
            editedJobState.Progression = ((float)data.CurrentFile / data.TotalFiles) * 100f;
        }
        else
        {
            editedJobState.SourceFilePath = "";
            editedJobState.TargetFilePath = "";
            editedJobState.State = StateLevel.End;
            editedJobState.TotalFilesToCopy = 0;
            editedJobState.TotalFilesSize = 0;
            editedJobState.NbFilesLeftToDo = 0;
            editedJobState.Progression = 0;
        }

        _save();
    }

    /// <summary>
    /// Serializes the current list of states to the JSON file.
    /// Uses UTF8 encoding for the file stream.
    /// </summary>
    private void _save()    // Method that saves the current list of states to the JSON file, it serializes the list of states and writes it to the file using UTF8 encoding
    {
        using (FileStream fs = File.Open(_stateFilePath, FileMode.Create, FileAccess.Write))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(_states));
            fs.Write(info, 0, info.Length);
        }
    }

    /// <summary>
    /// Retrieves the state information for a specific backup job by name.
    /// </summary>
    /// <param name="savedJobName">The name of the job to search for.</param>
    /// <returns>The StateInfo if found; otherwise, null.</returns>
    public StateInfo? GetStateFrom(string savedJobName) // Method that retrieves the state information for a specific backup job by name, it searches the list of states for a state with the given name and returns it, or null if not found
    {
        return _states.FirstOrDefault(s => s.Name == savedJobName);
    }

}