using System;
using System.Collections.Generic;
using System.Linq;

namespace AMCS.Data.Indexing.DelimitedData
{
  public static class Utils
  {
    /// <summary>
    /// Splits String by string delimeter (Required for Compact Framework 3.5 compatability)
    /// </summary>
    /// <param name="s">String</param>
    /// <param name="delimeter">String Delimiter</param>
    /// <param name="removeEmptyEntries">Remove entries where string is empty</param>
    /// <returns>Split String</returns>
    public static string[] SplitString(string s, string delimeter, bool removeEmptyEntries)
    {
      if (s == null)
        throw new ArgumentNullException("stringToBeSplitted is null.");
      if (delimeter == null)
        throw new ArgumentNullException("delimeter is null.");

      int dsum = 0;
      int ssum = 0;
      int dl = delimeter.Length;
      int sl = s.Length;

      if (dl == 0 || sl == 0 || sl < dl)
        return new string[] { s };

      char[] cd = delimeter.ToCharArray();
      char[] cs = s.ToCharArray();
      List<string> retlist = new List<string>();

      for (int i = 0; i < dl; i++)
      {
        dsum += cd[i];
        ssum += cs[i];
      }

      int start = 0;
      for (int i = start; i < sl - dl; i++)
      {
        if (i >= start && dsum == ssum && s.Substring(i, dl) == delimeter)
        {
          retlist.Add(s.Substring(start, i - start));
          start = i + dl;
        }

        ssum += cs[i + dl] - cs[i];
      }

      if (dsum == ssum && s.Substring(sl - dl, dl) == delimeter)
      {
        retlist.Add(s.Substring(start, sl - dl - start));
        retlist.Add("");
      }
      else
      {
        retlist.Add(s.Substring(start, sl - start));
      }
      if (removeEmptyEntries)
      {
        return retlist.Where(r => !string.IsNullOrEmpty(r)).ToArray();
      }
      else
      {
        return retlist.ToArray();
      }
    }
  }
}
