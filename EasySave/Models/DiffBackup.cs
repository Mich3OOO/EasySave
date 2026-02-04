using EasySave.Interfaces;

namespace EasySave.Models;

public class DiffBackup:Backup,IBackup
{ 
    public void Run()
    {
        throw new NotImplementedException();
    }

    protected override string[] _getFilesList()
    {
        throw new NotImplementedException();
    }

   
}