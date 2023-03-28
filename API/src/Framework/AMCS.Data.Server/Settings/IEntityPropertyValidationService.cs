using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Entity.Validation;

namespace AMCS.Data.Server.Settings
{
  public interface IEntityPropertyValidationService : IEntityObjectService<EntityPropertyValidationEntity>
  {
    /// <summary>
    /// Returns all EntityPropertyValidationEntity for a specified ClassName and PropertyName
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="className">If not null, limits returned results to those that match provided ClassName</param>
    /// <param name="propertyName">If not null, limits returned results to those that match provided PropertyName</param>
    /// <param name="existingDataSession">Data Session to use. Will create new one if null.</param>
    /// <returns>Collection of EntityPropertyValidationEntity</returns>
    IList<EntityPropertyValidationEntity> GetAllByClassNameAndPropertyName(string className = null, string propertyName = null, IDataSession existingDataSession = null);

    IList<EntityPropertyValidationEntity> GetAllByValidationContext(ISessionToken userId, int validationContextId, string entity, IDataSession existingDataSession = null);
  }
}
