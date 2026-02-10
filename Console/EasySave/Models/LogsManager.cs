using EasySave.Interfaces;
using EasyLog;

namespace EasySave.Models;

public class LogsManager : IEventListener   // Class representing the logs manager, implementing the IEventListener interface
{
    // Constructor that automatically subscribes to EventManager
    public LogsManager()
    {   
        EventManager.GetInstance().Subscribe(this);
    }
    // Transform and transfer BackupInfos to Logger
    public void Update(BackupInfo data)
    {
        string logText = this._toJson(data);
        Logger.GetInstance().Log(logText);
    }

    // Transform BackupInfo data into a JSON string
    private string _toJson(BackupInfo data)
    {
        // Check if the data sent
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "Backup data cannot be null.");
        }
        if (data.SavedJobInfo == null || data.CurrentCopyInfo == null)
        {
            throw new ArgumentException("Invalid backup data structure.");
        }
        // Create 'JSON' string with infos from data
        return $@"{{
           ""Name"": ""{data.SavedJobInfo.GetName()}"",
           ""FileSource"": ""{data.CurrentCopyInfo.Source}"",
           ""FileTarget"": ""{data.CurrentCopyInfo.Destination}"",
           ""FileSize"": {data.CurrentCopyInfo.Size},
           ""FileTransferTime"": {(data.CurrentCopyInfo.EndTime - data.CurrentCopyInfo.StartTime).TotalMilliseconds},
           ""Time"": ""{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}""
         }}";
    }
}