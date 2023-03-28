namespace AMCS.Data.Util.String
{
  using System;
  using System.Text.RegularExpressions;

  public static class StringUtils
  {
    public static string FormatAddress(string[] addressLines)
    {
      string address = string.Empty;

      foreach (string addressLine in addressLines)
      {
        if (!string.IsNullOrWhiteSpace(addressLine))
        {
          if (!string.IsNullOrWhiteSpace(address))
          {
            address = string.Format("{0}, {1}", address, addressLine);
          }
          else
          {
            address = addressLine;
          }
        }
      }

      return address;
    }

    public static bool NumericOnly(string stringValue)
    {
      Regex regex = new Regex("^[0-9]+$");
      if (regex.IsMatch(stringValue))
        return true;
      else
        return false;
    }


    /// <summary>
    /// Return the first 100 characters of the first line of the given string (with an ellipsis
    /// if shortened) for display in gridviews etc.
    /// </summary>
    public static string GetFirstLine(this string text)
    {
      string firstLine = text ?? "";
      var multiLine = !string.IsNullOrEmpty(text) && text.Contains("\r\n");

      if (multiLine)
      {
        var lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length > 0)
        {
          firstLine = lines[0];
        }
      }

      bool truncated = false;
      if (firstLine.Length > 100)
      {
        firstLine = firstLine.Substring(0, 100);
        truncated = true;
      }

      if (truncated || multiLine)
      {
        firstLine = firstLine + " ...";
      }

      return firstLine;
    }
  }
}
