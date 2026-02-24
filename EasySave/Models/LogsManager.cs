using EasyLog;
using EasySave.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Models;

/// <summary>
/// Class representing the logs manager, implementing the IEventListener interface
/// </summary>
public class LogsManager : IEventListener   
{
    private readonly Config _config = Config.GetInstance();
    public LogsMods LogsMods { get; set; } = LogsMods.Both;

    /// <summary>
    /// Constructor that automatically subscribes to EventManager
    /// </summary>
    public LogsManager()
    {
        LogsMods = Config.GetInstance().LogsMods;
        EventManager.GetInstance().Subscribe(this);
    }

    /// <summary>
    /// Transform and transfer BackupInfos to Logger
    /// </summary>
    public void Update(BackupInfo data)
    {
        LogsFormats format = _config.LogsFormat;
        string logText;
        string formatExtension;
        if (format == LogsFormats.Json)
        {
            formatExtension = "json";
            logText = ToJson(data);
        }
        else if (format == LogsFormats.Xml)
        {
            formatExtension = "xml";
            logText = ToXml(data);
        }
        else
        {
            formatExtension = "unknown";
            logText = ToTxt(data);
        }

        LogsMods currentLogsMods = Config.GetInstance().LogsMods;
        
        // Gestion des modes
        if (currentLogsMods == LogsMods.Local || currentLogsMods == LogsMods.Both)
        {
            Logger.GetInstance().Log(logText, formatExtension);
        }
        if (currentLogsMods == LogsMods.Centralized || currentLogsMods == LogsMods.Both)
        {
            _ = SendLogToApiAsync(logText, formatExtension, _config.API_URL); // fire-and-forget, pas d'attente
        }
        Console.WriteLine("[LogsManager] Update: Fin");
    }

    /// <summary>
    /// Transform BackupInfo data into a JSON string
    /// </summary>
    private static string ToJson(BackupInfo data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "Backup data cannot be null.");
        }
        if (data.SavedJobInfo == null || data.CurrentCopyInfo == null)
        {
            throw new ArgumentException("Invalid backup data structure.");
        }
        return $@"{{
           ""Name"": ""{data.SavedJobInfo.GetName()}"",
           ""FileSource"": ""{data.CurrentCopyInfo.Source}"",
           ""FileTarget"": ""{data.CurrentCopyInfo.Destination}"",
           ""FileSize"": {data.CurrentCopyInfo.Size},
           ""FileTransferTime"": {(data.CurrentCopyInfo.EndTime - data.CurrentCopyInfo.StartTime).TotalMilliseconds},
           ""TimeToEncrypt"": {data.CurrentCopyInfo.TimeToEncrypt},
           ""Time"": ""{DateTime.Now:dd/MM/yyyy HH:mm:ss}""
         }}";
    }

    /// <summary>
    /// Transform BackupInfo data into a XML string
    /// </summary>
    private static string ToXml(BackupInfo data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "Backup data cannot be null.");
        }
        if (data.SavedJobInfo == null || data.CurrentCopyInfo == null)
        {
            throw new ArgumentException("Invalid backup data structure.");
        }
        return $@"<Log>
                    <JobName>{data.SavedJobInfo.GetName()}</JobName>
                    <FileSource>{data.CurrentCopyInfo.Source}</FileSource>
                    <FileTarget>{data.CurrentCopyInfo.Destination}</FileTarget>
                    <FileSize>{data.CurrentCopyInfo.Size}</FileSize>
                    <FileTransferTime>{(data.CurrentCopyInfo.EndTime - data.CurrentCopyInfo.StartTime).TotalMilliseconds}</FileTransferTime>
                    <TimeToEncrypt>{data.CurrentCopyInfo.TimeToEncrypt}</TimeToEncrypt>
                    <Time>{DateTime.Now:dd/MM/yyyy HH:mm:ss}</Time>
                </Log>";
    }

    /// <summary>
    /// Transform BackupInfo data into a string
    /// </summary>
    private static string ToTxt(BackupInfo data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "Backup data cannot be null.");
        }
        if (data.SavedJobInfo == null || data.CurrentCopyInfo == null)
        {
            throw new ArgumentException("Invalid backup data structure.");
        }
        return $@"[{data.SavedJobInfo.GetName()}] - time:{DateTime.Now:dd/MM/yyyy HH:mm:ss} - source:{data.CurrentCopyInfo.Source} ; target:{data.CurrentCopyInfo.Destination} ; size:{data.CurrentCopyInfo.Size} ; transferTime:{(data.CurrentCopyInfo.EndTime - data.CurrentCopyInfo.StartTime).TotalMilliseconds} ; timeToEncrypt:{data.CurrentCopyInfo.TimeToEncrypt}";
    }

    /// <summary>
    /// Centralization of logs in Docker
    /// </summary>
    public static async Task SendLogToApiAsync(string logContent, string format, string url)
    {
        try
        {
            using var httpClient = new HttpClient();
            StringContent content;

            if (format == "json")
                content = new StringContent(logContent, Encoding.UTF8, "application/json");
            else if (format == "xml")
                content = new StringContent(logContent, Encoding.UTF8, "application/xml");
            else
                content = new StringContent(logContent, Encoding.UTF8, "text/plain");

            HttpResponseMessage response = await httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'envoi du log à l'API : {ex.Message}");
        }
    }
}
