namespace EasySave.Models;

public class CopyInfo   // Class representing information about a file copy operation, used for event updates
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
    public CopyInfo(string source, string destination, DateTime startTime, int size, DateTime endTime)
    {
        this.Source = source;
        this.Destination = destination;
        this.StartTime = startTime;
        this.Size = size;
        this.EndTime = endTime;
    }

    // Overload of the constructor to include the time to encrypt the file, previous one note deleted because not every files are encrypted
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
