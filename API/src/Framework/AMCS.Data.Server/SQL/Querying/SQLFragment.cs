using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL.Querying
{
  internal partial class SQLFragment
  {
    public static SQLFragment Parse(string text)
    {
      string part = null;
      var parts = new List<string>();
      var parameters = new List<SQLFragmentParameter>();

      foreach (object fragment in ParseParts(text))
      {
        if (fragment == ParameterPlaceholder)
        {
          parts.Add(part ?? string.Empty);
          part = null;
          parameters.Add(new SQLFragmentParameter());
        }
        else
        {
          if (part == null)
            part = (string)fragment;
          else
            part += (string)fragment;
        }
      }

      parts.Add(part ?? string.Empty);

      return new SQLFragment(parts, new ReadOnlyCollection<SQLFragmentParameter>(parameters));
    }

    private readonly List<string> parts;

    public IList<SQLFragmentParameter> Parameters { get; }

    private SQLFragment(List<string> parts, IList<SQLFragmentParameter> parameters)
    {
      this.parts = parts;
      Parameters = parameters;
    }

    public override string ToString()
    {
      var sb = new StringBuilder();

      for (int i = 0; i < parts.Count - 1; i++)
      {
        sb.Append(parts[i]);
        sb.Append(Parameters[i].Name ?? "(unknown parameter)");
      }

      sb.Append(parts[parts.Count - 1]);

      return sb.ToString();
    }
  }
}
