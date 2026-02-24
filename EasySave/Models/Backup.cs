using System.Diagnostics;
using EasySave.Interfaces;
using EasySave.ViewModels;

namespace EasySave.Models;

/// <summary>
/// Abstract class representing a backup operation, implementing the IBackup interface
/// </summary>
public abstract class Backup : IBackup
{
    protected SavedJob _savedJob;
    protected BackupInfo _backupInfo;
    protected string _sevenZipPath;
    private string _password;
    
    private bool _continue = true;
    private bool _cancel = false;

    protected static readonly SemaphoreSlim LargeFileSemaphore = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Constructor to initialize the backup with a saved job and backup info
    /// </summary>
    /// <param name="savedJob"> SavedJob that will be executed </param>
    /// <param name="backupInfo"> BackupInfo object that will be used </param>
    /// <param name="pw"> Password </param>
    public Backup(SavedJob savedJob, BackupInfo backupInfo, string pw = "")
    {
        _password = pw;
        _savedJob = savedJob;
        _backupInfo = backupInfo;
        _sevenZipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CryptoSoft", "7za.exe");
    }

    /// <summary>
    /// Setter for _password
    /// </summary>
    /// <param name="password"></param>
    public void SetPassword(string password)
    {
        _password = password;
    }

    /// <summary>
    /// Abstract method to execute the backup, to be implemented by derived classes
    /// </summary>
    public abstract void ExecuteBackup();

    /// <summary>
    /// Pause the backup by setting _continue to false
    /// </summary>
    public void Pause()
    {
        _continue = false;
    }

    /// <summary>
    /// Resume the backup by setting _continue to true
    /// </summary>
    public void Continue()
    {
        _continue = true;
    }

    /// <summary>
    /// Cancel the bakcup by setting _cancel to true
    /// </summary>
    public void Cancel()
    {
        _cancel = true;
    }

    /// <summary>
    /// Create a timestamp folder for backup
    /// </summary>
    /// <param name="subFolderType"></param>
    /// <returns></returns>
    protected string _createTimestampedFolder(string subFolderType)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fullPath = Path.Combine(_savedJob.Destination, subFolderType, timestamp);
        Directory.CreateDirectory(fullPath);
        return fullPath;
    }

    /// <summary>
    /// Method to backup a single file, handling encryption if needed, and updating the backup status
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="destinationPath"></param>
    protected void _backupFile(string sourceFilePath, string destinationPath)   
    {
        long fileSize = 0;
        bool isLargeFile = false;
        Config config = Config.S_GetInstance();

        try
        {
            // Prepare copy information
            CopyInfo copyInfo = new CopyInfo();
            copyInfo.Source = sourceFilePath;
            fileSize = new FileInfo(sourceFilePath).Length;
            copyInfo.Size = fileSize;

            // Déterminer si le fichier est > n Ko
            isLargeFile = (fileSize > config.MaxParallelLargeFileSizeKo * 1024);
            if (isLargeFile)
            {
                Console.WriteLine($"[DEBUG] Début transfert gros fichier : {sourceFilePath} ({fileSize / 1024} Ko)");
                LargeFileSemaphore.Wait();
            }

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
            if (config.ExtensionsToEncrypt.Contains(Path.GetExtension(targetFilePath)))
            {
                // Add .7z to the file path end Check Encrypt time
                targetFilePath += ".7z";
                DateTime temp = DateTime.Now;
                _encryptFile(sourceFilePath, targetFilePath);
                copyInfo.TimeToEncrypt = (int)(DateTime.Now - temp).TotalMicroseconds;
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

            while (!_continue)
            {
                // Pause
            }
        }
        catch (Exception ex)    // Handle any exceptions that occur during the file backup process
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
    /// Method to encrypt a file using 7-Zip, constructing the appropriate command-line arguments and handling the process execution
    /// (it work only if the 7za.exe is placed at the same emplacement that EasySave.exe)
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="targetFilePath"></param>
    /// <exception cref="Exception"></exception>
    private void _encryptFile(string sourceFilePath, string targetFilePath) // Method to encrypt a file using 7-Zip,
                                                                            // constructing the appropriate command-line
                                                                            // arguments and handling the process execution
    {
        // a = add (to add fie)
        // -t7z = 7z format
        // -p = password
        // -mhe=on = to encrypt file name
        // -mx=1 = fast compression
        // -y = to remplace existing file

        ProcessStartInfo p = new ProcessStartInfo();
        p.FileName = _sevenZipPath;

        p.Arguments = $"a -t7z -p\"{_password}\" -mhe=on -mx=1 -y \"{targetFilePath}\" \"{sourceFilePath}\"";

        p.WindowStyle = ProcessWindowStyle.Hidden; // To hide black screen
        p.CreateNoWindow = true;
        p.UseShellExecute = false;

        using (Process process = Process.Start(p))  // Start the 7z process with the specified arguments
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

    /// <summary>
    /// Update the backup information and notify the EventManager
    /// </summary>
    /// <param name="newCopyInfo"></param>
    protected void _updateStatus(CopyInfo newCopyInfo)
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