using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public class FieldMapBuilder
  {
    public static FieldMapBuilder Create()
    {
      return new FieldMapBuilder();
    }

    private readonly Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private FieldMapBuilder()
    {
    }

    public FieldMapBuilder Add(string source, string target)
    {
      map.Add(source, target);

      return this;
    }

    public IFieldMap Build()
    {
      return new FieldMap(map);
    }

    private class FieldMap : IFieldMap
    {
      private readonly Dictionary<string, string> map;

      public FieldMap(Dictionary<string, string> map)
      {
        this.map = map;
      }

      public bool TryMap(string source, out string target)
      {
        return map.TryGetValue(source, out target);
      }
    }
  }
}
