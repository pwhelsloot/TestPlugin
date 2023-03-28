using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server.Services;

namespace AMCS.Data.Server.Settings
{
  public class TranslationMappingOverrideService : EntityObjectService<TranslationMappingOverrideEntity>, ITranslationMappingOverrideService
  {
    public TranslationMappingOverrideService(IEntityObjectAccess<TranslationMappingOverrideEntity> dataAccess)
      : base(dataAccess)
    {
    }

    public IList<TranslationMappingOverrideEntity> GetAllByProjectIdentifier(string projectIdentifier, IDataSession existingDataSession = null)
    {
      IDataSession dataSession = GetDataSession(DataServices.Resolve<IUserService>().SystemUserSessionKey, existingDataSession);
      try
      {
        return GetAllByTemplate(DataServices.Resolve<IUserService>().SystemUserSessionKey, new TranslationMappingOverrideEntity { ProjectIdentifier = projectIdentifier }, false, dataSession);
      }
      finally
      {
        DisposeDataSession(dataSession, existingDataSession);
      }
    }
  }
}