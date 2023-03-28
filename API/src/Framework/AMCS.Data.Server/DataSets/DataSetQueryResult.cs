using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Server.DataSets
{
  [JsonConverter(typeof(DataSetQueryResultConverter))]
  public class DataSetQueryResult
  {
    public DataSetTable Table { get; }

    public DataSetQueryCursor Cursor { get; }

    public DataSetQueryResult(DataSetTable table, DataSetQueryCursor cursor)
    {
      Table = table;
      Cursor = cursor;
    }
  }
}
