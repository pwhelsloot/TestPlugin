using System.Collections.Generic;

namespace AMCS.Data.Schema.Discovery
{
  public interface ITableSchemaDiscoverer: IQueryableObjectSchemaDiscoverer
  {
    new ITable Discover(string schema, string tableName);
    new IList<ITable> Discover(string schema);
  }
}
