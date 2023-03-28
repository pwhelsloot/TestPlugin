using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Support
{
  internal static class NamingUtils
  {
    public static string CamelCase(string name)
    {
      int firstLowerCase = -1;

      for (int i = 0; i < name.Length; i++)
      {
        if (!char.IsUpper(name[i]))
        {
          firstLowerCase = i;
          break;
        }
      }

      // All upper case?
      if (firstLowerCase == -1)
        return name.ToLowerInvariant();
      // No upper case?
      if (firstLowerCase == 0)
        return name;

      // If the first lower case character is at 1, we have a normal name
      // like "Customer". If it's over one, we have something like GLExport.
      // In that case, we split before the last upper case letter, 2 in this case.
      // The result is lower case before the split and upper after the split.

      int split = 1;
      if (firstLowerCase > 1)
        split = firstLowerCase - 1;

      return name.Substring(0, split).ToLowerInvariant() + name.Substring(split);
    }
  }
}
