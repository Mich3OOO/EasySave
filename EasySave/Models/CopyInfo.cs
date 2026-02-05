namespace EasySave.Models;

public class CopyInfo
{
    public string Source;
    public string Destination;
    public DateTime StartTime;
    public long Size;
    public DateTime EndTime;

    public CopyInfo()
    {
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