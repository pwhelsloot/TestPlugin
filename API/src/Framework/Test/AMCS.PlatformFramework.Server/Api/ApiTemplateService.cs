using AMCS.Data;
using AMCS.Data.Server;
using AMCS.PlatformFramework.Entity.Api;
using System;

namespace AMCS.PlatformFramework.Server.Api
{
  public class ApiTemplateService : EntityObjectService<ApiTemplateEntity>
  {
    public ApiTemplateService(IEntityObjectAccess<ApiTemplateEntity> dataAccess)
  : base(dataAccess)
    {
    }

    public override ApiTemplateEntity GetById(ISessionToken userId, int id, IDataSession existingDataSession = null)
    {
      return new ApiTemplateEntity
      {
        TemplateName = $"Template-{id}",
        TemplateDate = DateTime.Now
      };
    }
  }
}