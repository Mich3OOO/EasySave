using EasySave.Interfaces;
using System.Text.Json;
using System.Text;

namespace EasySave.Models;

public class StateManager: IEventListener
{
    private List<StateInfo> _states;
    
    private readonly string _stateFilePath = "./states.json";

    public StateManager()
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
    
    public void Update(BackupInfo data)
    {
        Console.WriteLine("update state");
        StateInfo? editedJobState = _states.FirstOrDefault(s => s.Name == data.SavedJobInfo.Name);

        if (editedJobState == null)
        {
            editedJobState = new()
            {
                Name = data.SavedJobInfo.Name,
            };
            _states.Add(editedJobState);
        }

        if (data.CurrentCopyInfo != null)
        {
            editedJobState.SourceFilePath = data.CurrentCopyInfo.Source;
            editedJobState.TargetFilePath = data.CurrentCopyInfo.Destination;
            editedJobState.State = StateLevel.Active;
            editedJobState.TotalFilesToCopy = data.TotalFiles;
            editedJobState.TotalFilesSize = editedJobState.TotalFilesSize + data.CurrentCopyInfo.Size;
            editedJobState.NbFilesLeftToDo = data.TotalFiles - data.CurrentFile;
            editedJobState.Progression = ((float)data.CurrentFile/data.TotalFiles) * 100f;
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

    private void _save()
    {
        if (File.Exists(_stateFilePath)) File.Delete(_stateFilePath);
        using(FileStream fs = File.Open(_stateFilePath, FileMode.CreateNew, FileAccess.Write))
        {
            fs.Write(new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(_states)));
        }
    }

    public StateInfo? GetStateFrom(string savedJobName)
    {
        return _states.FirstOrDefault(s => s.Name == savedJobName);
    }

}