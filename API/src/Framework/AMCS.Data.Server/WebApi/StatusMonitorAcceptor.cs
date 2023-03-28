using System;
using System.Collections.Generic;
using System.Linq;
using AMCS.Data.Entity.JobSystem;
using AMCS.Data.Server.Services;
using AMCS.Data.Server.SQL.Querying;
using AMCS.JobSystem;
using AMCS.JobSystem.Scheduler.Api;
using AMCS.JobSystem.StatusMonitor.WebApi;

namespace AMCS.Data.Server.WebApi
{
#if NETFRAMEWORK
  internal class StatusMonitorAcceptor : AMCS.JobSystem.StatusMonitor.WebApi.Owin.StatusMonitorAcceptor
#else
  internal class StatusMonitorAcceptor : AMCS.JobSystem.StatusMonitor.WebApi.AspNetCore.StatusMonitorAcceptor
#endif
  {
    public StatusMonitorAcceptor()
      : base(DataServices.Resolve<SchedulerClient>())
    {
    }

    protected override bool TryAuthenticate(string authToken, out string userId)
    {
#if !NETFRAMEWORK
      // This is a strange one. For some reason, the auth token query string parameter
      // is double escaped in .NET Core.
      authToken = Uri.UnescapeDataString(authToken);
#endif

      if (DataServices.Resolve<IUserService>().TryDeserializeSessionToken(authToken, out var sessionToken) == SessionTokenStatus.Valid)
      {
        userId = JobHandler.GetJobUserId(sessionToken);
        return true;
      }

      userId = null;
      return false;
    }

    protected override List<RunningJobDto> GetRunningAndQueuedJobs()
    {
      var runningAndQueuedJobs = new List<RunningJobDto>();

      var jobCriteria = Criteria.For(typeof(ApiJob));
      jobCriteria
        .Add(Expression.In(nameof(ApiJob.Status), new[] { 0, 10 }));

      var sessionToken = DataServices.Resolve<IUserService>().SystemUserSessionKey;

      using (var dataSession = BslDataSessionFactory.GetDataSession(sessionToken, false))
      using (var transaction = dataSession.CreateTransaction())
      {
        runningAndQueuedJobs = dataSession.GetApiCollection<ApiJob>(sessionToken, jobCriteria, true)
          .Entities.Where(apiJob => (DateTime.Now - apiJob.Updated).TotalSeconds < 60)
          .Select(job =>
            new RunningJobDto
            {
              Id = (Guid)job.GUID,
              UserId = job.UserId,
              Posted = job.Created,
              Updated = job.Updated,
              Status = (JobStatus)job.Status,
              FriendlyName = job.FriendlyName,
              Handler = job.Handler,
              Parameters = job.Parameters,
              Queue = job.Queue,
              Deadline = job.Deadline,
              PersistenceMode = PersistenceMode.Persist,
              DuplicateMode = (DuplicateMode)job.DuplicateMode
            }).ToList();
        transaction.Commit();
      }

      return runningAndQueuedJobs;
    }
  }
}
