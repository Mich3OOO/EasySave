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
    public int TotalFilesToCopy ;
    [JsonInclude]
    public long TotalFilesSize;
    [JsonInclude]
    public int NbFilesLeftToDo;
    [JsonInclude]
    public float Progression ;
    
}