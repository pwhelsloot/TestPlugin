using System.Collections.Generic;

namespace AMCS.Data.Schema.Discovery
{
  public interface IForeignKeyDiscoverer
  {
    IList<IForeignKey> Discover(string schema, string table, string column);
    IList<IForeignKey> Discover(string schema, string table);
  }
}
