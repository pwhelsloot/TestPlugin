using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AMCS.Data.Server.Services;

namespace AMCS.Data.Server.SQL
{
  internal static class SQLAzureCompatibility
  {
    private static bool? IsAzureCompatibilityEnabled;

    internal static void SetAzureCompatibilityEnabled(bool enabled)
    {
      Debug.Assert(!IsAzureCompatibilityEnabled.HasValue);
      IsAzureCompatibilityEnabled = enabled;
    }

    private static readonly RegExTransformer[] Transformers =
    {
      // These are taken from the UpdateAzureSQLProject project.
      new RegExTransformer("group_concat_d", "([\\[]?dbo[\\]]?\\.)?([\\[]?group_concat_d[\\]]?)", "STRING_AGG"),
      new RegExTransformer("getdate", @"(GETDATE\(\))", "dbo.GETLOCALDATE()")
    };

    public static string Translate(string sql)
    {
      if (!IsAzureCompatibilityEnabled.Value)
        return sql;

      foreach (var transformer in Transformers)
      {
        sql = transformer.Replace(sql);
      }

      return sql;
    }

    private class RegExTransformer
    {
      private readonly string quickMatch;
      private readonly Regex pattern;
      private readonly string replacement;

      public RegExTransformer(string quickMatch, string pattern, string replacement)
      {
        this.quickMatch = quickMatch;
        this.pattern = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        this.replacement = replacement;
      }

      public string Replace(string input)
      {
        // We do a quick contains match before we run the regex to ensure we don't go into
        // the regex engine if we don't need to.
        if (input.IndexOf(quickMatch, StringComparison.OrdinalIgnoreCase) != -1)
          return pattern.Replace(input, replacement);

        return input;
      }
    }
  }
}
