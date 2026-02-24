using System.Threading;

namespace EasyLog;

/// <summary>
/// Provides a threadsafe singleton logger for writing messages to log files in various formats
/// </summary>
public class Logger
{
    private static Logger? _instance;
    private readonly string _logsPath = "./logs";
    static private readonly object _lockObj = new();
    private static readonly Mutex _mutex = new(false, "logger");

    private Logger(){}

    /// <summary>
    /// Log a message to a file with the specified format (txt, json, xml)
    /// </summary>
    public void Log(string message, string format)
    {
        _mutex.WaitOne();
        try
        {
            string path = GetFileName(format);
            File.AppendAllText(path, message + Environment.NewLine);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    /// <summary>
    /// Create a Logger singleton or return the existing one
    /// </summary>
    public static Logger GetInstance()
    {
        lock (_lockObj)
        {
            _instance ??= new Logger();
            return _instance;
        }
    }
    private string GetFileName(string format)
    {
        Directory.CreateDirectory(_logsPath);
        string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "_log." + format;
        return Path.Combine(_logsPath, fileName);
    }

}