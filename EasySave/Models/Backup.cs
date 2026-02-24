using System.Diagnostics;
using EasySave.Interfaces;
using EasySave.ViewModels;

namespace EasySave.Models;

/// <summary>
/// Abstract class representing a backup operation, implementing the IBackup interface
/// </summary>
/// <remarks>
/// Constructor to initialize the backup with a saved job and backup info
/// </remarks>
public abstract class Backup(SavedJob savedJob, BackupInfo backupInfo, string pw = "") : IBackup
{
    protected SavedJob _savedJob = savedJob;
    protected BackupInfo _backupInfo = backupInfo;
    protected string _sevenZipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CryptoSoft", "7za.exe");
    private string _password = pw;
    
    private bool _continue = true;
    protected bool _cancel = false;
    protected bool _isCriticalFileFinised = false;

    protected static readonly SemaphoreSlim LargeFileSemaphore = new(1, 1);

    public void SetPassword(string password)
    {
        _password = password;
    }

    /// <summary>
    /// Abstract method to execute the backup, to be implemented by derived classes
    /// </summary>
    public abstract void ExecuteBackup();
    public bool isCriticalCopyFinished() => _isCriticalFileFinised;

    public void Pause()
    {
        _continue = false;
    }

    public void Continue()
    {
        _continue = true;
    }

    public void Cancel()
    {
        _cancel = true;
    }

    protected string CreateTimestampedFolder(string subFolderType)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fullPath = Path.Combine(_savedJob.Destination, subFolderType, timestamp);
        Directory.CreateDirectory(fullPath);
        return fullPath;
    }

    /// <summary>
    /// Method to backup a single file, handling encryption if needed, and updating the backup status
    /// </summary>
    protected void BackupFile(string sourceFilePath, string destinationPath)   
    {
        long fileSize = 0;
        bool isLargeFile = false;
        Config config = Config.GetInstance();

        try
        {
            CopyInfo copyInfo = new()
            {
                Source = sourceFilePath
            };
            fileSize = new FileInfo(sourceFilePath).Length;
            copyInfo.Size = fileSize;

            isLargeFile = (fileSize > config.MaxParallelLargeFileSizeKo * 1024);
            if (isLargeFile)
            {
                Console.WriteLine($"[DEBUG] Début transfert gros fichier : {sourceFilePath} ({fileSize / 1024} Ko)");
                LargeFileSemaphore.Wait();
            }

            string relativePath = Path.GetRelativePath(_savedJob.Source, sourceFilePath);

            string targetFilePath = Path.Combine(destinationPath, relativePath);

            string? targetDirectory = Path.GetDirectoryName(targetFilePath);
            if (targetDirectory != null && !Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            copyInfo.StartTime = DateTime.Now;

            if (config.ExtensionsToEncrypt.Contains(Path.GetExtension(targetFilePath)))
            {
                // Add .7z to the file path end Check Encrypt time
                targetFilePath += ".7z";
                DateTime temp = DateTime.Now;
                EncryptFile(sourceFilePath, targetFilePath);
                copyInfo.TimeToEncrypt = (int)(DateTime.Now - temp).TotalMicroseconds;
            }
            else
            {
                File.Copy(sourceFilePath, targetFilePath, true);
            }
            copyInfo.EndTime = DateTime.Now;

            copyInfo.Destination = targetFilePath;

            // Notify observer
            UpdateStatus(copyInfo);

            while (!_continue)
            {
                // Pause
            }
        }
        // Handle any exceptions that occur during the file backup process
        catch (Exception ex)
        {
            Console.WriteLine($"Error copying file {sourceFilePath}: {ex.Message}");
        }
        finally
        {
            if (isLargeFile)
            {
                Console.WriteLine($"[DEBUG] Fin transfert gros fichier : {sourceFilePath} ({fileSize / 1024} Ko)");
                LargeFileSemaphore.Release();
            }
        }
    }

    /// <summary>
    /// Method to encrypt a file using 7-Zip, constructing the appropriate 
    /// command-line arguments and handling the process execution (it work 
    /// only if the 7za.exe is placed at the same emplacement that EasySave.exe)
    /// </summary>
    private void EncryptFile(string sourceFilePath, string targetFilePath)
    {
        // a = add (to add fie)
        // -t7z = 7z format
        // -p = password
        // -mhe=on = to encrypt file name
        // -mx=1 = fast compression
        // -y = to remplace existing file

        ProcessStartInfo p = new()
        {
            FileName = _sevenZipPath,

            Arguments = $"a -t7z -p\"{_password}\" -mhe=on -mx=1 -y \"{targetFilePath}\" \"{sourceFilePath}\"",

            WindowStyle = ProcessWindowStyle.Hidden, // To hide black screen
            CreateNoWindow = true,
            UseShellExecute = false
        };

        using Process process = Process.Start(p);
        if (process != null)
        {
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"7-Zip failed with exit code {process.ExitCode}");
            }
        }
    }

    /// <summary>
    /// Return all files to backup
    /// </summary>
    protected virtual string[] GetFilesList()
    {
        return Directory.GetFiles(_savedJob.Source, "*", SearchOption.AllDirectories);
    }
    protected string[] _separateCriticalFiles(out string[] notCriticalFiles)
    {
        
        string[] criticalExtensions = Config.S_GetInstance().CriticalExtensions;
        List<string> allFiles = new List<string>(_getFilesList()); 
        List<string> criticalfiles = new List<string>(_getFilesList());

        for (int i = allFiles.Count - 1; i >= 0; i--)
        {
            foreach (string extension in criticalExtensions)
            {
                if (allFiles[i].EndsWith(extension))
                {
                    criticalfiles.Add(allFiles[i]);
                    allFiles.RemoveAt(i);
                }
            }
        }

        notCriticalFiles = allFiles.ToArray();
        return criticalfiles.ToArray();
        
    }

    /// <summary>
    /// Update the backup information and notify the EventManager
    /// </summary>
    protected void UpdateStatus(CopyInfo newCopyInfo)
    {
        _backupInfo.SavedJobInfo = _savedJob;
        _backupInfo.LastCopyInfo = _backupInfo.CurrentCopyInfo;
        _backupInfo.CurrentCopyInfo = newCopyInfo;
        _backupInfo.CurrentFile++;

        EventManager.GetInstance().Update(_backupInfo);
    }
    
}