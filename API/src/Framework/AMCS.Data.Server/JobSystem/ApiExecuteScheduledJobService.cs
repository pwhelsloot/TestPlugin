using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.JobSystem;

namespace AMCS.Data.Server.JobSystem
{
  public class ApiExecuteScheduledJobService : EntityObjectService<ApiExecuteScheduledJob>
  {
    public ApiExecuteScheduledJobService(IEntityObjectAccess<ApiExecuteScheduledJob> dataAccess)
      : base(dataAccess)
    {
    }

    public override int? Save(ISessionToken userId, ApiExecuteScheduledJob entity, IDataSession existingDataSession = null)
    {
      if (entity == null)
        throw new ArgumentNullException(nameof(entity));

      var jobSystemService = DataServices.Resolve<IJobSystemService>();

      jobSystemService.ExecuteScheduledJob(userId, entity.ScheduledJobId);

      return null;
    }
  }
}
