
using System;

namespace AMCS.Data.Schema.Discovery.Sql
{
  public class SqlDiscoverer
  {
    protected string SanitiseObjectName(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof(name));

      return name.Replace("[", "").Replace("]", "");
    }
  }
}
