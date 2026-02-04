using EasySave.Interfaces;
namespace EasySave.Models;

public class CompBackup:Backup,IBackup
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