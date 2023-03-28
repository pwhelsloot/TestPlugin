using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.PlatformFramework.Entity.Api.JobSystem;

namespace AMCS.PlatformFramework.Server.Api.JobSystem
{
  public class ApiJobCancelService : EntityObjectService<ApiJobCancel>
  {
    public ApiJobCancelService(IEntityObjectAccess<ApiJobCancel> dataAccess)
      : base(dataAccess)
    {
    }

    public override int? Save(ISessionToken userId, ApiJobCancel entity, IDataSession existingDataSession = null)
    {
      Jobs.Client.CancelJob(entity.JobId);

      return null;
    }
  }
}
