using System.Threading;

namespace EasyLog;

public class Logger
{
    private static Logger? _instance;
    private readonly string _logsPath = "./logs";
    private string _format;
    static private readonly object _lockObj = new object();

    private static Mutex _mutex = new Mutex(false, "logger");

    /// <summary>
    /// Empty constructor
    /// </summary>
    private Logger()
    {
    }

    /// <summary>
    /// Log the data into the file according to the parameters
    /// </summary>
    /// <param name="message"> Text that will be writen in the file</param>
    /// <param name="format"> Format of the file (currently json, xml or txt</param>
    public void Log(string message, string format)
    {
        _mutex.WaitOne(); // Wait until the mutex is available
        try
        {
            string path = _getFileName(format);
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
    /// <returns></returns>
    public static Logger GetInstance()
    {
        lock (_lockObj)
        {
            if (_instance == null)
            {
                _instance = new Logger();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Return the path of the current log file (based on the current date)
    /// </summary>
    /// <param name="format"> Format used, it gets applied to the filename </param>
    /// <returns></returns>
    private string _getFileName(string format)
    {
        Directory.CreateDirectory(_logsPath);
        string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "_log." + format;
        return Path.Combine(_logsPath, fileName);
    }

}