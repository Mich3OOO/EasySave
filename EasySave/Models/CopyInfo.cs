namespace EasySave.Models;

/// <summary>
/// Class representing information about a file copy operation, used for event updates
/// </summary>
public class CopyInfo
{
    public string Source;
    public string Destination;
    public DateTime StartTime;
    public long Size;
    public DateTime EndTime;
    public int TimeToEncrypt;

    public CopyInfo()
    {
        Source = string.Empty;
        Destination = string.Empty;
        StartTime = DateTime.MinValue;
        Size = 0;
        EndTime = DateTime.MinValue;
        TimeToEncrypt = 0;
    }

    /// <summary>
    /// Constructor that include the time to encrypt the file
    /// </summary>
    public CopyInfo(string source, string destination, DateTime startTime, int size, DateTime endTime, int timeToEncrypt)
    {
        this.Source = source;
        this.Destination = destination;
        this.StartTime = startTime;
        this.Size = size;
        this.EndTime = endTime;
        this.TimeToEncrypt = timeToEncrypt;
    }
}
