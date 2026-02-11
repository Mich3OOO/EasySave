using EasySave.Interfaces;
using EasyLog;

namespace EasySave.Models;

public class LogsManager : IEventListener   // Class representing the logs manager, implementing the IEventListener interface
{

    Config _config = Config.S_GetInstance();

    // Constructor that automatically subscribes to EventManager
    public LogsManager()
    {   
        EventManager.GetInstance().Subscribe(this);
    }
    // Transform and transfer BackupInfos to Logger
    public void Update(BackupInfo data)
    {
        string logText = ""; // Initializing the log text variable
        LogsFormats formats = _config.GetLogsFormat(); // Retrieving the logs format from config
        if (formats == LogsFormats.Json) // If the format is JSON, transform the data into a JSON string and log it
        {
            logText = this._toJson(data);
        }
        if (formats == LogsFormats.Xml) // If the format is XML, transform the data into an XML string and log it
        {
            logText = this._toXml(data);
        }
        else // If the format isn't reconized or sent, to Json (default)
        {
            logText = this._toJson(data);
        }
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

    // Transform BackupInfo data into a XML string
    private string _toXml(BackupInfo data)
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
        // Create 'XML' string with infos from data
        return $@"<Log>
                    <Name>{data.SavedJobInfo.GetName()}</Name>
                    <FileSource>{data.CurrentCopyInfo.Source}</FileSource>
                    <FileTarget>{data.CurrentCopyInfo.Destination}</FileTarget>
                    <FileSize>{data.CurrentCopyInfo.Size}</FileSize>
                    <FileTransferTime>{(data.CurrentCopyInfo.EndTime - data.CurrentCopyInfo.StartTime).TotalMilliseconds}</FileTransferTime>
                    <Time>{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</Time>
                </Log>";
    }
}