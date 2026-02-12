using System.Diagnostics;
using EasySave.Interfaces;

namespace EasySave.Models;

public abstract class Backup : IBackup  // Abstract class representing a backup operation, implementing the IBackup interface
{
    protected SavedJob _savedJob;
    protected BackupInfo _backupInfo;
    protected string _sevenZipPath;

    public Backup(SavedJob savedJob, BackupInfo backupInfo)
    {
        _savedJob = savedJob;
        _backupInfo = backupInfo;
        _sevenZipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7za.exe");
    }

    public abstract void ExecuteBackup();

    //Create a timestamp folder for backup
    protected string _createTimestampedFolder(string subFolderType)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fullPath = Path.Combine(_savedJob.Destination, subFolderType, timestamp);
        Directory.CreateDirectory(fullPath);
        return fullPath;
    }

    protected void _backupFile(string sourceFilePath, string destinationPath)
    {
        try
        {
            // Prepare copy information
            CopyInfo copyInfo = new CopyInfo();
            copyInfo.Source = sourceFilePath;
            copyInfo.Size = new FileInfo(sourceFilePath).Length;

            // Calculate relative path to maintain structure
            string relativePath = Path.GetRelativePath(_savedJob.Source, sourceFilePath);

            // Construct full target path using specific destination folder
            string targetFilePath = Path.Combine(destinationPath, relativePath);
            

            // Create destination directory if it doesn't exist
            string? targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (targetDirectory != null && !Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // Make copy with time
            copyInfo.StartTime = DateTime.Now;

            // Check if the file need to be encrypt
            Config config = Config.S_GetInstance();
            if (config.ExtensionsToEncrypt.Contains(Path.GetExtension(targetFilePath)))
            {
                // Add .7z to the file path end Check Encrypt time
                targetFilePath += ".7z";
                DateTime temp = DateTime.Now;
                _encryptFile(sourceFilePath, targetFilePath);
                copyInfo.TimeToEncrypt =  (int)(DateTime.Now - temp).TotalMicroseconds;
            }
            else
            {
                File.Copy(sourceFilePath, targetFilePath, true);
            }
            copyInfo.EndTime = DateTime.Now;

            // Add File path after change it if the file is encrypt
            copyInfo.Destination = targetFilePath;

            // Notify observer
            _updateStatus(copyInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error copying file {sourceFilePath}: {ex.Message}");
        }
    }

    // To encrypt file with 7z (it work only if the 7za.exe is placed at the same emplacement that EasySave.exe
    private void _encryptFile(string sourceFilePath, string targetFilePath)
    {
        // PassWord use to encrypt
        string password = "azerty";

        // a = add (to add fie)
        // -t7z = 7z format
        // -p = password
        // -mhe=on = to encrypt file name
        // -mx=1 = fast compression
        // -y = to remplace existing file

        ProcessStartInfo p = new ProcessStartInfo();
        p.FileName = _sevenZipPath;

        p.Arguments = $"a -t7z -p\"{password}\" -mhe=on -mx=1 -y \"{targetFilePath}\" \"{sourceFilePath}\"";

        p.WindowStyle = ProcessWindowStyle.Hidden; // To hide black screen
        p.CreateNoWindow = true;
        p.UseShellExecute = false;

        // 3. Start the processus
        using (Process process = Process.Start(p))
        {
            if (process != null)
            {
                process.WaitForExit(); // Wait the end of 7z encryptation

                // Check if encryptation Wrk
                if (process.ExitCode != 0)
                {
                    throw new Exception($"7-Zip failed with exit code {process.ExitCode}");
                }
            }
        }
    }
    
    protected virtual string[] _getFilesList()
    {
        // Return all files to backup
        return Directory.GetFiles(_savedJob.Source, "*", SearchOption.AllDirectories);
    }

    protected void _updateStatus(CopyInfo newCopyInfo)  // Update the backup information and notify the EventManager
    {
        // Update general info
        _backupInfo.SavedJobInfo = _savedJob;
        _backupInfo.LastCopyInfo = _backupInfo.CurrentCopyInfo;
        _backupInfo.CurrentCopyInfo = newCopyInfo;
        _backupInfo.CurrentFile++;

        // Notify Observer
        EventManager.GetInstance().Update(_backupInfo);
    }
}