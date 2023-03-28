using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public class SQLQueryParameterCollection : Collection<SQLQueryParameter>
  {
    public object this[string name]
    {
      get => FindParameter(name)?.Value;

      set
      {
        var parameter = FindParameter(name);
        if (parameter != null)
          parameter.Value = value;
        else
          Add(new SQLQueryParameter(name, value));
      }
    }

    private SQLQueryParameter FindParameter(string name)
    {
      foreach (var parameter in this)
      {
        if (string.Equals(name, parameter.Name, StringComparison.OrdinalIgnoreCase))
          return parameter;
      }

      return null;
    }

    public void Add(string name, object value)
    {
      this[name] = value;
    }
  }
}
