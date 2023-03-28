using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using AMCS.Data.Configuration;
using AMCS.Data.Server.Configuration;
using AMCS.Data.Server.DataSets.Support;
using AMCS.Data.Server.Services;
using AMCS.JobSystem.Scheduler.Api;
using log4net;

namespace AMCS.Data.Server.DataSets
{
  internal class DataSetImportManager : IDataSetImportManager, IDisposable
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(DataSetImportManager));

    private readonly IJobReader jobReader;
    private readonly SchedulerClient schedulerClient;
    private readonly ConcurrentDictionary<Guid, Job> jobs = new ConcurrentDictionary<Guid, Job>();
    private readonly TempFileService tempFileService;
    private Timer timer; 
    private bool disposed;

    public const int DefaultMaxConcurrency = 2;

    public int Concurrency { get; }

    public DataSetImportManager(IDataSetsConfiguration configuration, IJobReader jobReader, IConnectionString blobStorageConnectionString, SchedulerClient schedulerClient)
    {
      Concurrency = configuration.Concurrency.GetValueOrDefault(DefaultMaxConcurrency);

      this.jobReader = jobReader;
      this.schedulerClient = schedulerClient;
      if (schedulerClient != null)
        SetupJobSystem();

      tempFileService = new TempFileService(
        configuration.Storage?.Azure?.Container,
        configuration.Storage?.FileSystem?.Store,
        configuration.Storage?.Ttl ?? TimeSpan.FromDays(7),
        blobStorageConnectionString);
    }

    private void SetupJobSystem()
    {
      var jobs = schedulerClient.GetRunningJobs();

      foreach (var job in jobs)
      {
        if (job.Handler == typeof(DataSetImportJob).FullName)
          this.jobs[job.Id] = Job.From(job);
      }

      schedulerClient.UpdatesAvailable += SchedulerClient_UpdatesAvailable;
    }

    private void SchedulerClient_UpdatesAvailable(object sender, JobUpdatesEventArgs e)
    {
      foreach (var update in e.Updates)
      {
        switch (update)
        {
          case IJobProgress jobProgress:
            if (jobs.TryGetValue(jobProgress.Id, out var job))
              jobs[jobProgress.Id] = job.WithProgress(jobProgress);
            break;

          case IJobStatus jobStatus:
            if (jobs.TryGetValue(jobStatus.Id, out job))
              jobs[jobStatus.Id] = job.WithStatus(jobStatus);
            break;

          case IRunningJob runningJob:
            if (runningJob.Handler == typeof(DataSetImportJob).FullName)
              jobs[runningJob.Id] = Job.From(runningJob);
            break;
        }
      }
    }

    public string WriteFile(Stream stream)
    {
      return tempFileService.WriteFile(stream);
    }

    public Stream ReadFile(string key)
    {
      return tempFileService.ReadFile(key);
    }

    public void DeleteFile(string key)
    {
      tempFileService.DeleteFile(key);
    }

    public IDataSetImportJob GetJob(Guid id)
    {
      if (!jobs.TryGetValue(id, out var job))
        job = GetJobFromReader(id);

      return job;
    }

    private Job GetJobFromReader(Guid id)
    {
      var job = jobReader?.GetJobStatus(id);
      if (job != null)
        return Job.From(job);

      return null;
    }

    public void DismissJob(Guid id)
    {
      jobs.TryRemove(id, out _);
    }

    public void CancelJob(Guid id)
    {
      if (jobs.ContainsKey(id))
        schedulerClient.CancelJob(id);
    }

    public void Dispose()
    {
      if (!disposed)
      {
        if (timer != null)
        {
          timer.Dispose();
          timer = null;
        }

        if (tempFileService != null)
        {
          tempFileService.Dispose();
          timer = null;
        }

        disposed = true;
      }
    }

    private class Job : IDataSetImportJob
    {
      public JobStatus Status { get; }
      public TimeSpan Runtime { get; }
      public double? LastProgress { get; }
      public string LastStatus { get; }
      public string Result { get; }

      private Job(JobStatus status, TimeSpan runtime, double? lastProgress, string lastStatus, string result)
      {
        Status = status;
        Runtime = runtime;
        LastProgress = lastProgress;
        LastStatus = lastStatus;
        Result = result;
      }

      public static Job From(IRunningJob job)
      {
        return new Job(
          job.Status,
          job.Runtime,
          job.LastProgress,
          job.LastStatus,
          null);
      }

      public static Job From(Support.Job job)
      {
        return new Job(
          job.Status,
          job.Runtime,
          null,
          null,
          job.Result);
      }

      public Job WithProgress(IJobProgress progress)
      {
        return new Job(
          Status,
          progress.Runtime,
          progress.Progress,
          progress.Status,
          null);
      }

      public Job WithStatus(IJobStatus status)
      {
        return new Job(
          status.Status,
          status.Runtime,
          LastProgress,
          LastStatus,
          status.Result);
      }
    }
  }
}