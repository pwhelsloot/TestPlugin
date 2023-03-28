using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.JobSystem.Scheduler.Api;

namespace AMCS.Data.Server
{
  public interface IJobSystemService
  {

    SchedulerClient Client { get; }

    Guid PostJob(ISessionToken userId, IJobRequest request, Action<IJobUpdate> update = null);

    Guid ExecuteScheduledJob(ISessionToken userId, Guid scheduledJobId, Action<IJobUpdate> update = null);
  }
}