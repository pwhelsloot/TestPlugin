using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  public class SQLQueryParameter
  {
    public string Name { get; }
    
    public object Value { get; set; }

    public SQLQueryParameter(string name)
      : this(name, null)
    {
    }

    public SQLQueryParameter(string name, object value)
    {
      Name = name;
      Value = value;
    }
  }
}
