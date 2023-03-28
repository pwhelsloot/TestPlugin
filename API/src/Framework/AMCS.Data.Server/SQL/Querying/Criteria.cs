using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public static class Criteria
  {
    public static ICriteria For(Type entityType)
    {
      return new CriteriaBuilder(entityType);
    }
  }
}
