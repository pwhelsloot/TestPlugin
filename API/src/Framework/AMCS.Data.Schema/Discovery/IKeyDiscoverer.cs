using System.Collections.Generic;

namespace AMCS.Data.Schema.Discovery
{
  public interface IKeyDiscoverer
  {
    IList<IKey> Discover(string schema, string table, string column, IEnumerable<KeyType> restrictToKeyTypes = null);
    IList<IKey> Discover(string schema, string table, IEnumerable<KeyType> restrictToKeyTypes = null);
  }
}
