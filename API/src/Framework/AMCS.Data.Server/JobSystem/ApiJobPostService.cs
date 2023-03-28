using System;
using AMCS.Data.Entity.JobSystem;
using AMCS.JobSystem;
using AMCS.JobSystem.Scheduler.Api;

namespace AMCS.Data.Server.JobSystem
{
  [Obsolete("You need to use ApiExecuteScheduledJobService instead of ApiJobPostService.", true)]
  public class ApiJobPostService : EntityObjectService<ApiJobPost>
  {
    public override int? Save(ISessionToken userId, ApiJobPost entity, IDataSession existingDataSession = null)
    {
      var job = new JobBuilder()
        .Queue(entity.Queue)
        .Handler(entity.Handler)
        .ParametersJson(entity.Parameters)
        .PersistenceMode((PersistenceMode)entity.PersistenceMode)
        .DuplicateMode((DuplicateMode)entity.DuplicateMode);

      if (entity.FriendlyName != null)
        job.FriendlyName(entity.FriendlyName);
      if (entity.Deadline.HasValue)
        job.Deadline(TimeSpan.FromSeconds(entity.Deadline.Value));

      var jobSystemService = DataServices.Resolve<IJobSystemService>();

      var jobBuild = job.Build();

      jobSystemService.PostJob(userId, jobBuild);

      return null;
    }
  }
}