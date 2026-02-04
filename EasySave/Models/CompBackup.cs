using EasySave.Interfaces;
namespace EasySave.Models;

public class CompBackup:Backup,IBackup
{ 
    public void ExecuteBackup()
    {
        throw new NotImplementedException();
    }

    protected override string[] _getFilesList()
    {
        throw new NotImplementedException();
    }

   
}