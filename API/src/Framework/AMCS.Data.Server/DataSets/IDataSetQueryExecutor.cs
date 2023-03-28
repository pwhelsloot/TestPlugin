using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets
{
  public interface IDataSetQueryExecutor
  {
    DataSetQueryResult Query(ISessionToken userId, DataSetQuery query, DataSetQueryCursor cursor, int? maxResults, IDataSession session);
  }
}
