using EasySave.Models;

namespace EasySave.ViewModels;

public class BackupViewModel
{
    private StatesManager _statesManager;
    private Config _config;

    private void _runBackup(int jobId, BackupType backupType)   //private method to run single backup
    {
        throw new NotImplementedException();
    }

    public void RunRangeBackup(string range, BackupType backupType)     //Public method, will call _getJobIdsToBackup to get back the job IDs to backup, then call _runBackup for each job ID
    {
        throw new NotImplementedException();
    }

    private int[] _getJobIdsToBackup(string range) //private method to parse the range string and return an array of job IDs to backup (e.g., "1-3" returns [1,2,3], "1;3" returns [1,3])
    {
        throw new NotImplementedException();
    }
}