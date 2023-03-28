using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.Settings
{
  public interface ITranslationMappingOverrideService : IEntityObjectService<TranslationMappingOverrideEntity>
  {
    IList<TranslationMappingOverrideEntity> GetAllByProjectIdentifier(string projectIdentifier, IDataSession existingDataSession = null);
  }
}
