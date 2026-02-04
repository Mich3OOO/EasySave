using EasySave.Interfaces;
using EasyLog;

namespace EasySave.Models;

public class LogsManager:IEventListener
{
    // Transform and transfer BackupInfos to Logger
    public void Update(BackupInfo data)
    {
        string logText = this._toJson(data);
        Logger.GetInstance().Log(logText);
    }

    // Transform BackupInfo data into a JSON string
    private string _toJson(BackupInfo data)
    {
        return $@"{{
           ""Name"": ""{data.SavedJobInfo.getName()}"",
           ""FileSource"": ""{data.CurrentCopyInfo.Source.Replace(@"\", @"\\")}"",
           ""FileTarget"": ""{data.CurrentCopyInfo.Destination.Replace(@"\", @"\\")}"",
           ""FileSize"": {data.CurrentCopyInfo.Size},
           ""FileTransferTime"": {(data.CurrentCopyInfo.EndTime - data.CurrentCopyInfo.StartTime).TotalMilliseconds},
           ""Time"": ""{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}""
         }}";
    }
}