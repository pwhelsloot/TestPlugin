using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity.Validation;
using AMCS.Data.Server;
using AMCS.Data.Server.Settings.Data;
using AMCS.Data.Server.SQL;

namespace AMCS.PlatformFramework.Server.Settings.Data.SQL
{
  public class SQLEntityPropertyValidationAccess : SQLEntityObjectAccess<EntityPropertyValidationEntity>, IEntityPropertyValidationAccess
  {
    public IList<EntityPropertyValidationEntity> GetAllByClassNameAndPropertyName(IDataSession dataSession, string className = null, string propertyName = null)
    {
      return new List<EntityPropertyValidationEntity>();
    }

    public IList<EntityPropertyValidationEntity> GetAllByValidationContext(IDataSession dataSession, int validationContextId)
    {
      return new List<EntityPropertyValidationEntity>();
    }
  }
}
