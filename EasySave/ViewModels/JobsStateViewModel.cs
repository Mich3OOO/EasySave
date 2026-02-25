using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Threading;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.ViewModels;

public partial class JobsStateViewModel : ObservableObject
{
    // List of the jobs who are running right now
    public ObservableCollection<JobProgressViewModel> ActiveJobs { get; } = [];

    public IRelayCommand ClearHistoryCommand { get; }

    public LanguageViewModel LanguageViewModel { get; } = LanguageViewModel.GetInstance();

    public string T_saves_progression => LanguageViewModel.GetTranslation("saves_progression");

    public string T_delete => LanguageViewModel.GetTranslation("clear_history");

    // Event delegate to tell the main window to close this
    public event Action? OnCloseRequested;

    [RelayCommand]
    private void Close()
    {
        OnCloseRequested?.Invoke();
    }

    public JobProgressViewModel AddNewJob(string jobName)
    {
        // Create new job in the list
        var newJob = new JobProgressViewModel(jobName);
        ActiveJobs.Add(newJob);

        return newJob;
    }

    public JobsStateViewModel()
    {
        ClearHistoryCommand = new RelayCommand(ClearHistory);
    }

    private void ClearHistory()
    {
        for (int i = ActiveJobs.Count - 1; i >= 0; i--)
        {
            if (ActiveJobs[i].IsFinished)
                ActiveJobs.RemoveAt(i);
        }
    }
}

public partial class JobProgressViewModel : ObservableObject, IEventListener
{
    [ObservableProperty] private string _jobName;
    [ObservableProperty] private double _progress;
    [ObservableProperty] private bool _isPaused;
    [ObservableProperty] private bool _isFinished;

    public LanguageViewModel LanguageViewModel { get; } = LanguageViewModel.GetInstance();

    public string PauseIcon => IsPaused ? "▷" : "⏸";
    public string T_saves_progression => LanguageViewModel.GetTranslation("saves_progression");
    public string T_finished => LanguageViewModel.GetTranslation("finished");

    public JobProgressViewModel(string jobName)
    {
        JobName = jobName;
        Progress = 0;
    }

    // called by observer when a file is copied
    public void Update(BackupInfo data)
    {
        // Only update this progress bar if the event is for this specific job
        if (data.SavedJobInfo.Name != JobName)
            return;

        if (data.TotalFiles <= 0) return;

        var percentage = ((double)data.CurrentFile / data.TotalFiles) * 100;

        // asynchrous send to the UI
        Dispatcher.UIThread.Post(() =>
        {
            UpdateProgress(percentage);
        });
    }

    public void UpdateProgress(double percentage)
    {
        Progress = percentage;
        if (Progress >= 100 && !IsFinished)
        {
            IsFinished = true;
        }
    }

    [RelayCommand]
    private void TogglePause()
    {
        IsPaused = !IsPaused;
        OnPropertyChanged(nameof(PauseIcon));

        // Get SavedJobs object by his name
        var jobToPause = Config.GetInstance().SavedJobs.FirstOrDefault(j => j.Name == JobName);

        if (jobToPause != null)
        {
            if (IsPaused)
            {
                JobManager.GetInstance().PauseJob(jobToPause);
            }
            else
            {
                JobManager.GetInstance().ContinueJob(jobToPause);
            }
        }
    }

    [RelayCommand]
    private void Stop()
    {
        IsFinished = true;

        var jobToStop = Config.GetInstance().SavedJobs.FirstOrDefault(j => j.Name == JobName);
        if (jobToStop != null)
        {
            JobManager.GetInstance().CancelJob(jobToStop);
        }

        JobName += " (Canceled)";
    }
}
