using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EasySave.Models;

/// <summary>
/// Class representing the real-time state of a backup job, including progress 
/// and file information, used for persistence and UI updates
/// </summary>
public class StateInfo  
{
    
    [JsonInclude]
    public string Name
    {
        get 
        {
            if (string.Empty == _name) throw new ValidationException("Name can't be empty");
            return _name;
        } 
            
        set
        {
            if (string.Empty == value) throw new ValidationException("Name can't be empty");
            _name = value;
        }
    }
    [JsonInclude]
    public string SourceFilePath { get; set; } = "";
    [JsonInclude]
    public string TargetFilePath { get; set; } = "";
    [JsonInclude]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StateLevel State = StateLevel.Inactive;
    [JsonInclude]
    public uint TotalFilesToCopy
    {
        get => _totalFilesToCopy;  set
    {
        if (value < NbFilesLeftToDo) throw new ValidationException("NbFilesLeftToDo can't be greater than TotalFilesToCopy");
        _totalFilesToCopy = value;
            
    } }
    [JsonInclude]
    public uint TotalFilesSize { get; set; }
    [JsonInclude]
    public uint NbFilesLeftToDo { get => _nbFilesLeftToDo;
        set
        {
            if (value > TotalFilesToCopy) throw new ValidationException("NbFilesLeftToDo can't be greater than TotalFilesToCopy");
            _nbFilesLeftToDo = value;
            
        } }
    [JsonInclude]
    public float Progression
    {
        get => _progression; set
    {
        if (0 > value ) throw new ValidationException("Progression can't be less than 0");
        if (100 < value) throw new ValidationException("Progression can't be greater than 100");
        _progression = value;
    } }
    
    private string _name = "";
    private float _progression;
    private uint _nbFilesLeftToDo;
    private uint _totalFilesToCopy;


}