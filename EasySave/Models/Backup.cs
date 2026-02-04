namespace EasySave.Models;

public abstract class Backup
{

    protected SavedJob _savedJob;
    protected BackupInfo _backupInfo;


    protected  void _backupFile(string fileName)
    {
        throw new NotImplementedException();
    }

    protected abstract string[] _getFilesList();

    protected void _updateStatus(CopyInfo newCopyInfo)
    {
        throw new NotImplementedException();
    }
    
}