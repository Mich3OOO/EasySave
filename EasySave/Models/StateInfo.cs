using System.Text.Json.Serialization;

namespace EasySave.Models;

public class StateInfo
{
    [JsonInclude]
    public string Name = "";
    [JsonInclude]
    public string SourceFilePath = "";
    [JsonInclude]
    public string TargetFilePath= "";
    [JsonInclude]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StateLevel State = StateLevel.Inactive;
    [JsonInclude]
    public int TotalFilesToCopy = 0;
    [JsonInclude]
    public long TotalFilesSize = 0;
    [JsonInclude]
    public int NbFilesLeftToDo = 0;
    [JsonInclude]
    public float Progression = 0;
    
}