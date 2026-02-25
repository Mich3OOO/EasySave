namespace EasySave.Models;

/// <summary>
/// Class representing information about a backup operation, used for event updates
/// </summary>
public class BackupInfo
{
    public required SavedJob SavedJobInfo;
    public CopyInfo? CurrentCopyInfo;
    public CopyInfo? LastCopyInfo;
    public uint CurrentFile
    {
        get => _currentFile;
        set
        {
            if (value > _totalFiles) throw new ArgumentOutOfRangeException("_currentFile current File cannot be greater than TotalFiles");
            _currentFile =  value ;
        }
    }

    public uint TotalFiles{
        get => _totalFiles;
        set
        {
            if (value < _currentFile) throw new ArgumentOutOfRangeException("TotalFiles cannot be lower than _currentFile");
            _totalFiles =  value ;
        }
        
    }
    private uint _currentFile;
    private uint _totalFiles;
}
