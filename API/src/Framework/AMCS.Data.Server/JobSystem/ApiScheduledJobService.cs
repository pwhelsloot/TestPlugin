using System;
using System.Collections.Generic;
using AMCS.Data.Entity.JobSystem;
using AMCS.Data.Server.SQL.Querying;
using AMCS.JobSystem;
using AMCS.JobSystem.Scheduler.Api;

namespace AMCS.Data.Server.JobSystem
{
  public class ApiScheduledJobService : EntityObjectService<ApiScheduledJob>
  {
    public override void Delete(ISessionToken userId, ApiScheduledJob entity, IDataSession existingDataSession = null)
    {
      if (entity == null)
        throw new ArgumentNullException(nameof(entity));

      var jobSystemService = DataServices.Resolve<IJobSystemService>();

      jobSystemService.Client.DeleteScheduledJob(entity.GUID.Value);

      var dataSession = BslDataSessionFactory.GetDataSession(userId, false);
      var builder = dataSession.GetHistoryBuilder(entity);

      builder.SetKind(DataUpdateKind.Delete);
      dataSession.CreateEntityHistory(userId, builder);
    }

    public override int? Save(ISessionToken userId, ApiScheduledJob entity, IDataSession existingDataSession = null)
    {
      if (entity == null)
        throw new ArgumentNullException(nameof(entity));

      var scheduledJob = DeserializeScheduledJob(entity);
      var jobSystemService = DataServices.Resolve<IJobSystemService>();

      var dataSession = BslDataSessionFactory.GetDataSession(userId, false);
      var builder = dataSession.GetHistoryBuilder(entity);

      var scheduledJobCriteria = Criteria.For(typeof(ApiScheduledJob))
        .Add(Expression.Eq(nameof(ApiScheduledJob.GUID), entity.GUID.Value));

      if (entity.GUID.HasValue && dataSession.GetExistsByCriteria<ApiScheduledJob>(userId, scheduledJobCriteria))
      {
        jobSystemService.Client.UpdateScheduledJob(scheduledJob);
        builder.SetKind(DataUpdateKind.Update);
        dataSession.CreateEntityHistory(userId, builder);
      }
      else
      {
        jobSystemService.Client.CreateScheduledJob(scheduledJob);

        builder.SetKind(DataUpdateKind.Insert);
        dataSession.CreateEntityHistory(userId, builder);
      }

      return null;
    }

    private IScheduledJob DeserializeScheduledJob(ApiScheduledJob entity)
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
