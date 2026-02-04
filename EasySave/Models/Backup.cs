namespace EasySave.Models;

public abstract class Backup
{

    protected Job _job;
    protected BackupInfo _backupInfo;


    protected  void _backupFile()
    {
        throw new NotImplementedException();
    }

    protected abstract string[] _getFilesList();

    protected void _updateStatus(Step newStep)
    {
        throw new NotImplementedException();
    }
    
}