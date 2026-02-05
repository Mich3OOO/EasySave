using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.ViewModels;

public class BackupViewModel
{
    private StatesManager _statesManager;
    private Config _config;

    private void _runBackup(int jobId, BackupType backupType)   //private method to run single backup
    {
        Config config = Config.S_GetInstance();
        SavedJob savedJob = config.GetJob(jobId);
        BackupInfo backupInfo = new BackupInfo();
        backupInfo.TotalFiles = 0;   //initialize total files to 0, will be updated in the backup process
        IBackup backup;

        if (backupType == BackupType.Differential)     //if backup type is differential, create a DiffBackup object and call its ExecuteBackup method
        {
            //IBackup backup = new DiffBackup();
        }
        else if (backupType == BackupType.Complete)
        {
            backup = new CompBackup(savedJob, backupInfo);
            backup.ExecuteBackup();
            Console.WriteLine($"Backup for job ID {jobId} from the source '{savedJob.Source}' to the destination '{savedJob.Destination}' has been completed.");

        }
    }

    public void RunRangeBackup(string range, BackupType backupType)     //Public method, will call _getJobIdsToBackup to get back the job IDs to backup, then call _runBackup for each job ID
    {
        try 
        {
            int[] jobIdsToBackup = _getJobIdsToBackup(range);
            foreach (int jobId in jobIdsToBackup)
            {
                _runBackup(jobId, backupType);
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log the error, show a message to the user, etc.)
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private int[] _getJobIdsToBackup(string range) //private method to parse the range string and return an array of job IDs to backup (e.g., "1-3" returns [1,2,3], "1;3" returns [1,3])
    {
        if (range.Contains("-"))   //if the range contains a "-", we split it and create a range of job IDs from the first to the second number
        {
            string[] parts = range.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end))
            {
                if (start > end) throw new ArgumentException("Start of range must be less than or equal to end.");
                return Enumerable.Range(start, end - start + 1).ToArray();
            }
            else
            {
                throw new ArgumentException("Invalid range format. Expected format: 'start-end'.");
            }
        }
        else if (range.Contains(";"))    //if the range contains a ";", we split it and parse each part as a separate job ID
        {
            string[] parts = range.Split(';');
            List<int> jobIds = new List<int>();
            foreach (string part in parts)
            {
                if (int.TryParse(part, out int jobId))
                {
                    jobIds.Add(jobId);
                }
                else
                {
                    throw new ArgumentException($"Invalid job ID: '{part}'. Expected an integer.");
                }
            }
            return jobIds.ToArray();
        }
        else    //if the range is just a single number, we parse it as a single job ID
        {
            if (int.TryParse(range, out int jobId))
            {
                return new int[] { jobId };
            }
            else
            {
                throw new ArgumentException($"Invalid job ID: '{range}'. Expected an integer.");
            }
        }
    }
}