using System.ComponentModel.DataAnnotations;

namespace EasySave.Models;

/// <summary>
/// Class representing information about a file copy operation, used for event updates
/// this class is used to pass informations
/// </summary>
public class CopyInfo
{
    private string _source;
    private string _destination;
    private DateTime _startTime;
    private uint _size;
    private DateTime _endTime;
    private uint _timeToEncrypt;

    public string Source
    {
        get => _source;
        set
        {
            if (!Path.Exists(value)) throw new ValidationException("Source path not found");
            _source = value;
        }
    }

    public string Destination
    {
        get => _destination;
        set
        {
            if (!Path.Exists(value)) throw new ValidationException("Destination path not found");
            _destination = value;
        }
    }

    public DateTime StartTime
    {
        get => _startTime;
        set => _startTime = value;
    }

    public uint Size
    {
        get => _size;
        set
        {
            if (0 > value) throw new ValidationException("File size cannot be negative");
            _size = value;
        }
    }

    public DateTime EndTime
    {
        get => _endTime;
        set => _endTime = value;
    }

    public uint TimeToEncrypt
    {
        get => _timeToEncrypt;
        set
        {
            if (0 > value) throw new ValidationException("TimeToEncrypt cannot be negative");
            _size = value;
        }
    }

    public CopyInfo()
    {
        _source = string.Empty;
        _destination = string.Empty;
        _startTime = DateTime.MinValue;
        _size = 0;
        _endTime = DateTime.MinValue;
        _timeToEncrypt = 0;
    }

    /// <summary>
    /// Constructor that include the time to encrypt the file
    /// </summary>
    public CopyInfo(string source, string destination, DateTime startTime, uint size, DateTime endTime,
        uint timeToEncrypt)
    {
        this.Source = source;
        this.Destination = destination;
        this.StartTime = startTime;
        this.Size = size;
        this.EndTime = endTime;
        this.TimeToEncrypt = timeToEncrypt;
    }
}