namespace EasySave.Models;

/// <summary>
/// Class representing information about a file copy operation, used for event updates
/// this class is used to pass informations
/// </summary>
public class CopyInfo   
{
    public string Source;
    public string Destination;
    public DateTime StartTime;
    public long Size;
    public DateTime EndTime;
    public int TimeToEncrypt; // Time to encrypt the file (in milliseconds)

    public CopyInfo()
    {
        Source = string.Empty;
        Destination = string.Empty;
        StartTime = DateTime.MinValue;
        Size = 0;
        EndTime = DateTime.MinValue;
        TimeToEncrypt = 0;
        // Empty constructor
    }
    // TO DELETE !!! I left it here to not break everything while charlie's part (encryption) isn't done yet
    public CopyInfo(string source, string destination, DateTime startTime, int size, DateTime endTime) // TO DELETE
    {
        this.Source = source; // TO DELETE
        this.Destination = destination; // TO DELETE
        this.StartTime = startTime; // TO DELETE
        this.Size = size; // TO DELETE
        this.EndTime = endTime; // TO DELETE
    }

    /// <summary>
    /// Constructor that include the time to encrypt the file
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="startTime"></param>
    /// <param name="size"></param>
    /// <param name="endTime"></param>
    /// <param name="timeToEncrypt"></param>
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
