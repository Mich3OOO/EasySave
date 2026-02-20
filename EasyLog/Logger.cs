namespace EasyLog;

public class Logger
{
    private static Logger? _instance;
    private readonly string _logsPath = "./logs";

    private Logger()
    {
        //throw new NotImplementedException();
    }

    // Allows to create logs as a text file with "[level] - message"
    // Is not used by EasySave because logs are requested in JSON
    public void Log(LogLevel level, string message, string format)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Log the data into the file according to the parameters
    /// </summary>
    /// <param name="message"></param>
    /// <param name="format"></param>
    public void Log(string message, string format)
    {
        string path = _getFileName(format);
        File.AppendAllText(path, message + Environment.NewLine);
    }

    // Create a Logger singleton or return the existing one
    public static Logger GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Logger();
        }

        return _instance;
    }

    // Return the path of the current log file (based on the current date)
    private string _getFileName(string format)
    {
        Directory.CreateDirectory(_logsPath);
        string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "_log." + format;
        return Path.Combine(_logsPath, fileName);
    }

}