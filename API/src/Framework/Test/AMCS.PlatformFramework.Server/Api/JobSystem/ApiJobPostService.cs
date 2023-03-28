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
  public class ApiJobPostService : EntityObjectService<ApiJobPost>
  {
    public ApiJobPostService(IEntityObjectAccess<ApiJobPost> dataAccess)
      : base(dataAccess)
    {
    }

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

      Jobs.Post(userId, job.Build());

      return null;
    }
  }
}
