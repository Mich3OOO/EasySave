using EasyLog;
using EasySave.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Models;

public class LogsManager : IEventListener   // Class representing the logs manager, implementing the IEventListener interface
{
    Config _config = Config.S_GetInstance();
    public LogsMods LogsMods { get; set; } = LogsMods.Both; // Par défaut, les deux

    // Constructor that automatically subscribes to EventManager
    public LogsManager()
    {
        LogsMods = Config.S_GetInstance().LogsMods; // Toujours à jour
        EventManager.GetInstance().Subscribe(this);
    }

    // Transform and transfer BackupInfos to Logger
    public void Update(BackupInfo data)
    {
        string logText = "";
        string formatExtension = "";
        LogsFormats format = _config.LogsFormat;
        if (format == LogsFormats.Json)
        {
            formatExtension = "json";
            logText = this._toJson(data);
        }
        else if (format == LogsFormats.Xml)
        {
            formatExtension = "xml";
            logText = this._toXml(data);
        }
        else
        {
            formatExtension = "unknown";
            logText = this._toTxt(data);
        }

        var currentLogsMods = Config.S_GetInstance().LogsMods;
        
        // Gestion des modes
        if (currentLogsMods == LogsMods.Local || currentLogsMods == LogsMods.Both)
        {
            Logger.GetInstance().Log(logText, formatExtension);
        }
        if (currentLogsMods == LogsMods.Centralized || currentLogsMods == LogsMods.Both)
        {
            _ = SendLogToApiAsync(logText, formatExtension); // fire-and-forget, pas d'attente
        }
        Console.WriteLine("[LogsManager] Update: Fin");
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
           ""TimeToEncrypt"": {data.CurrentCopyInfo.TimeToEncrypt},
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
                    <JobName>{data.SavedJobInfo.GetName()}</JobName>
                    <FileSource>{data.CurrentCopyInfo.Source}</FileSource>
                    <FileTarget>{data.CurrentCopyInfo.Destination}</FileTarget>
                    <FileSize>{data.CurrentCopyInfo.Size}</FileSize>
                    <FileTransferTime>{(data.CurrentCopyInfo.EndTime - data.CurrentCopyInfo.StartTime).TotalMilliseconds}</FileTransferTime>
                    <TimeToEncrypt>{data.CurrentCopyInfo.TimeToEncrypt}</TimeToEncrypt>
                    <Time>{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</Time>
                </Log>";
    }

    // Transform BackupInfo data into a string
    private string _toTxt(BackupInfo data)
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
        // Create string with infos from data
        return $@"[{data.SavedJobInfo.GetName()}] - time:{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - source:{data.CurrentCopyInfo.Source} ; target:{data.CurrentCopyInfo.Destination} ; size:{data.CurrentCopyInfo.Size} ; transferTime:{(data.CurrentCopyInfo.EndTime - data.CurrentCopyInfo.StartTime).TotalMilliseconds} ; timeToEncrypt:{data.CurrentCopyInfo.TimeToEncrypt}";
    }

    // Centralisation of logs in Docker
    public async Task SendLogToApiAsync(string logContent, string format)
    {
        try
        {
            // Create a new HttpClient instance for sending the request
            using var httpClient = new HttpClient();
            StringContent content;
            string url = "http://localhost:8080/api/logs";

            // Set the appropriate Content-Type header based on the log format
            if (format == "json")
                content = new StringContent(logContent, Encoding.UTF8, "application/json");
            else if (format == "xml")
                content = new StringContent(logContent, Encoding.UTF8, "application/xml");
            else
                content = new StringContent(logContent, Encoding.UTF8, "text/plain");

            // Send the POST request to the API endpoint
            var response = await httpClient.PostAsync(url, content);
            // Throw an exception if the response indicates failure
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Log any error that occurs during the HTTP request
            Console.WriteLine($"Erreur lors de l'envoi du log à l'API : {ex.Message}");
        }
    }
}