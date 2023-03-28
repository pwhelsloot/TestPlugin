namespace AMCS.Data.Server.UserDefinedField
{
  using System;
  using System.Collections.Generic;

  public static class ListExtensionMethods
  {
    public static bool ContainsCaseInsensitive(this IList<string> list, string compareTo)
    {
      foreach (var item in list)
      {
        if (string.Equals(item, compareTo, StringComparison.OrdinalIgnoreCase))
          return true;
      }

      return false;
    }
  }
}