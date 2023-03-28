using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public interface IEntityLoadedValuesService
  {
    bool HasPropertyChanged(EntityObject entity, object[] loadedValues, string propertyName);

    bool HasAnyPropertyChanged(EntityObject entity, object[] loadedValues);
  }
}
