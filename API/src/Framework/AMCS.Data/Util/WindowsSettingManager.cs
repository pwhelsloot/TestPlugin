namespace AMCS.Data.Util
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public static class WindowsSettingManager
  {
    public static string GetDefaultListSeperator()
    {
      Type t = typeof(System.Globalization.CultureInfo);
      Object[] s = t.GetCustomAttributes(true);
      foreach (Object ob in s)
      {

        Console.WriteLine(ob.ToString());
      }
      return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
    }

    public static string GetDefaultListSeperator(string culture)
    {
      try
      {
        return System.Globalization.CultureInfo.GetCultureInfo(culture).TextInfo.ListSeparator;
      }
      catch
      {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
      }
    }
  }
}
