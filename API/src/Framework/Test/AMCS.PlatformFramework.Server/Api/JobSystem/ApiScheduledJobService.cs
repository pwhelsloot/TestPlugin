using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.JobSystem;
using AMCS.JobSystem.Scheduler.Api;
using AMCS.PlatformFramework.Entity.Api.JobSystem;

namespace AMCS.PlatformFramework.Server.Api.JobSystem
{
  public class ApiScheduledJobService : EntityObjectService<ApiScheduledJob>
  {
    public ApiScheduledJobService(IEntityObjectAccess<ApiScheduledJob> dataAccess)
      : base(dataAccess)
    {
    }

    public override void Delete(ISessionToken userId, ApiScheduledJob entity, IDataSession existingDataSession = null)
    {
      Jobs.Client.DeleteScheduledJob(entity.GUID.Value);
    }

    public override int? Save(ISessionToken userId, ApiScheduledJob entity, IDataSession existingDataSession = null)
    {
      var scheduledJob = DeserializeScheduledJob(entity);

      if (entity.GUID.HasValue)
        Jobs.Client.UpdateScheduledJob(scheduledJob);
      else
        Jobs.Client.CreateScheduledJob(scheduledJob);

      return null;
    }

    private static IScheduledJob DeserializeScheduledJob(ApiScheduledJob entity)
    {
      var jobBuilder = new JobBuilder()
        .FriendlyName(entity.FriendlyName)
        .Handler(entity.Handler)
        .ParametersJson(entity.Parameters)
        .Queue(entity.Queue)
        .DuplicateMode((DuplicateMode)entity.DuplicateMode);

      if (entity.PersistenceMode.HasValue)
        jobBuilder.PersistenceMode((PersistenceMode)entity.PersistenceMode.Value);
      if (entity.Deadline.HasValue)
        jobBuilder.Deadline(TimeSpan.FromSeconds(entity.Deadline.Value));

      var builder = new ScheduledJobBuilder()
        .TriggerPlan(entity.Trigger)
        .Job(jobBuilder.Build());

      if (entity.GUID.HasValue)
        builder.Id(entity.GUID.Value);

      return builder.Build();
    }
  }
}
