namespace EasyLog;

public class Logger
{
    private static Logger _instance;
    private readonly string _logsPath = "./logs";
    
    private Logger()
    {
        throw new NotImplementedException();
    }

    // Allows to create logs as a text file with "[level] - message"
    // Is not used by EasySave because logs are requested in JSON
    public void Log (LogLevel level,string message)
    {
        throw new NotImplementedException();
    }

    // Allows to create logs as a JSON file
    // The creation of the JSON document is managed in EasySave and transferred as a string
    public void Log(string message)
    {
        string _path = _getFileName();
        System.IO.File.AppendAllText(_path, message + System.Environment.NewLine);
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
    private string _getFileName()
    {
        System.IO.Directory.CreateDirectory(_logsPath); // Create the logs directory if it doesn't exist
        string _nomFichier = DateTime.Now.ToString("yyyy-MM-dd") + "_log.json"; // Assemble the file name based on the current date
        return System.IO.Path.Combine(_logsPath, _nomFichier); // Assemble the path with the file name (better than a concatenation because it handles / and \)
    }
   
}