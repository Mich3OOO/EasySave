namespace EasyLog;

public class Logger
{
    private Logger _instance;
    private readonly string _logsPath;
    
    private Logger()
    {
        throw new NotImplementedException();
    }
    public void Log (LogLevel level,string message)
    {
        throw new NotImplementedException();
    }
    public Logger GetInstance()
    {
        throw new NotImplementedException();
    }
    private string _getFileName()
    {
        throw new NotImplementedException();
    }
   
}