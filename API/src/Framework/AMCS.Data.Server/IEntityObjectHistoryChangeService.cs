using AMCS.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server
{
  public interface IEntityObjectHistoryChangeService<T> : IEntityObjectService
  {
    void AddEntityHistoryChanges(T entity, EntityHistoryBuilder builder);
  }
}
