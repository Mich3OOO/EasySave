using EasySave.Interfaces;

namespace EasySave.Models;

public class LogsManager:IEventListener
{
    public void Update(BackupInfo data)
    {
        throw new NotImplementedException();
    }

    private string _toJson(BackupInfo data)
    {
        return $@"{{
           ""Name"": ""{data.SavedJobInfo.getName()}"",
           ""FileSource"": ""{data.CurrentCopyInfo.Source.Replace(@"\", @"\\")}"",
           ""FileTarget"": ""{data.CurrentCopyInfo.Destination.Replace(@"\", @"\\")}"",
           ""FileSize"": {data.CurrentCopyInfo.Size},
           ""FileTransferTime"": {je vais devoir faire end time moins start time},
           ""Time"": ""{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}""
         }}";
    }
}