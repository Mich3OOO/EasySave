namespace EasySave.Models;

public class CopyInfo   // Class representing information about a file copy operation, used for event updates
{
    public string Source;
    public string Destination;
    public DateTime StartTime;
    public long Size;
    public DateTime EndTime;

    public CopyInfo()
    {
        Source = string.Empty;
        Destination = string.Empty;
        StartTime = DateTime.MinValue;
        Size = 0;
        EndTime = DateTime.MinValue;
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
}
