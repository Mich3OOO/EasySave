using System.Text.Json.Serialization;

namespace EasySave.Models;

/// <summary>
/// Class representing the real-time state of a backup job, including progress 
/// and file information, used for persistence and UI updates
/// </summary>
public class StateInfo  
{
    [JsonInclude]
    public string Name = "";
    [JsonInclude]
    public string SourceFilePath = "";
    [JsonInclude]
    public string TargetFilePath = "";
    [JsonInclude]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StateLevel State = StateLevel.Inactive;
    [JsonInclude]
    public int TotalFilesToCopy;
    [JsonInclude]
    public long TotalFilesSize;
    [JsonInclude]
    public int NbFilesLeftToDo;
    [JsonInclude]
    public float Progression;

}