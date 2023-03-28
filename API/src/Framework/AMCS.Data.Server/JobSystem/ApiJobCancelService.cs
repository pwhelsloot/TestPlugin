using AMCS.Data.Entity.JobSystem;
using System;

namespace AMCS.Data.Server.JobSystem
{
  public class ApiJobCancelService : EntityObjectService<ApiJobCancel>
  {
    public override int? Save(ISessionToken userId, ApiJobCancel entity, IDataSession existingDataSession = null)
    {
      if (entity == null)
        throw new ArgumentNullException(nameof(entity));

      var jobSystemService = DataServices.Resolve<IJobSystemService>();

      jobSystemService.Client.CancelJob(entity.JobId);

      return null;
    }
  }
}
